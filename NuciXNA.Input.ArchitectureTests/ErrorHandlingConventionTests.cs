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
    /// Error-handling and concurrency convenience checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class ErrorHandlingConventionTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNullGuards_ThenNoStandardEqualityComparesNull(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                BinaryExpressionSyntax[] nullComparisons = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<BinaryExpressionSyntax>()
                    .Where(expression => expression.IsKind(SyntaxKind.EqualsExpression) ||
                        expression.IsKind(SyntaxKind.NotEqualsExpression))
                    .Where(expression => expression.Left is IdentifierNameSyntax left && IsConstantNull(left))
                    .ToArray();

                Assert.That(
                    nullComparisons,
                    Is.Empty,
                    "Violated convention: the 'is null' or 'is not null' pattern must be used.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingLambdaParameters_ThenNoVarDeclarationsAreUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                SyntaxNode[] lambdaVarDeclarations = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<VariableDeclarationSyntax>()
                    .Where(declaration => declaration.Type.IsVar)
                    .ToArray();

                Assert.That(
                    lambdaVarDeclarations,
                    Is.Empty,
                    $"Violated convention: lambda variable declarations must use explicit types.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingReturnStatements_ThenEveryReturnIsBraced(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                SyntaxNode[] illegalReturns = tree.GetRoot()
                    .DescendantNodes()
                    .Where(node => node is ReturnStatementSyntax && node.Parent is not BlockSyntax)
                    .ToArray();

                Assert.That(
                    illegalReturns,
                    Is.Empty,
                    $"Violated convention: return statements must appear only in method blocks.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingExpressionStatements_ThenNoEmptyStatementsAreUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                SyntaxNode[] emptyStatements = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<EmptyStatementSyntax>()
                    .ToArray();

                Assert.That(
                    emptyStatements,
                    Is.Empty,
                    $"Violated convention: empty statements are prohibited.");
            }
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

        private static bool IsConstantNull(IdentifierNameSyntax identifier)
        {
            string name = identifier.Identifier.ValueText;

            return name.Equals("null", StringComparison.Ordinal) ||
                name.Equals("default", StringComparison.Ordinal);
        }
    }
}
