using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciXNA.Input.ArchitectureTests
{
    /// <summary>
    /// Code style checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class CodeStyleConventionTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingAsyncMembers_ThenNoAsyncOrTaskIsUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                SyntaxToken[] asyncModifiers = tree.GetRoot()
                    .DescendantTokens()
                    .Where(token => token.IsKind(SyntaxKind.AsyncKeyword))
                    .ToArray();

                Assert.That(
                    asyncModifiers,
                    Is.Empty,
                    $"Violated convention in {sourcePrefix}: the 'async' modifier is prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingTaskTypes_ThenNoTaskIsReferenced(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                IdentifierNameSyntax[] taskIdentifiers = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Where(identifier => identifier.Identifier.ValueText.Equals("Task", StringComparison.Ordinal))
                    .ToArray();

                Assert.That(
                    taskIdentifiers,
                    Is.Empty,
                    $"Violated convention in {sourcePrefix}: Task types are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingCatchClauses_ThenNoneAreEmpty(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                CatchClauseSyntax[] emptyCatchClauses = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<CatchClauseSyntax>()
                    .Where(clause => clause.Block.Statements.Count == 0)
                    .ToArray();

                Assert.That(
                    emptyCatchClauses,
                    Is.Empty,
                    $"Violated convention in {sourcePrefix}: empty catch blocks are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingCommentStyle_ThenEveryCommentBlockEndsWithAPeriod(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                string[] lines = ReadSource(sourcePrefix, source)
                    .Replace("\r", string.Empty)
                    .Split('\n');

                for (int index = 0; index < lines.Length; index++)
                {
                    string trimmed = lines[index].TrimStart();

                    if (!IsPlainComment(trimmed))
                    {
                        continue;
                    }

                    bool isLastCommentLine = index == lines.Length - 1 ||
                        !IsPlainComment(lines[index + 1].TrimStart());

                    if (!isLastCommentLine)
                    {
                        continue;
                    }

                    Assert.That(
                        trimmed.TrimEnd(),
                        Does.EndWith("."),
                        $"Violated convention in {source}: every comment block must end with a period.");
                }
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingIncrementOperators_ThenNoCompoundSelfAssignmentsAreUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                SyntaxNode[] selfAssignments = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<AssignmentExpressionSyntax>()
                    .Where(assignment => assignment.IsKind(SyntaxKind.AddExpression) ||
                        assignment.IsKind(SyntaxKind.SubtractExpression))
                    .ToArray();

                Assert.That(
                    selfAssignments,
                    Is.Empty,
                    $"Violated convention in {sourcePrefix}: compound self-assignments are prohibited.");
            }
        }

        private static bool IsPlainComment(string line)
        {
            if (line.StartsWith("//", StringComparison.Ordinal) &&
                !line.StartsWith("///", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        private static IEnumerable<SyntaxTree> CollectParseTrees(string sourcePrefix)
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(sourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                $"Violated convention: no source files were found under '{sourcePrefix}'.");

            return sources.Select(source => CSharpSyntaxTree.ParseText(
                ArchitectureResources.ReadResource(sourcePrefix, source),
                path: source));
        }

        private static IEnumerable<string> CollectSources(string sourcePrefix)
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(sourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                $"Violated convention: no source files were found under '{sourcePrefix}'.");

            return sources;
        }

        private static string ReadSource(string sourcePrefix, string source)
            => ArchitectureResources.ReadResource(sourcePrefix, source);
    }
}
