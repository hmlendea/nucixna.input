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
    /// Indentation and formatting checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class IndentationConventionTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingIndentation_ThenTabsAreNeverUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                Assert.That(
                    ReadSource(sourcePrefix, source),
                    Does.Not.Contain('\t'),
                    $"Violated convention in {source}: indentation must use four spaces, never tabs.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingTrailingWhitespace_ThenNoneExists(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                Assert.That(
                    ReadSource(sourcePrefix, source),
                    Does.Not.Match(@"[ \t]+(\r?\n)"),
                    $"Violated convention in {source}: trailing whitespace is not allowed.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingConsecutiveBlankLines_ThenAtMostOneExists(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                Assert.That(
                    ReadSource(sourcePrefix, source).Replace("\r", string.Empty),
                    Does.Not.Contain("\n\n\n"),
                    $"Violated convention in {source}: two or more consecutive blank lines are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingClosingDelimiters_ThenNoBlankLinePrecedesThem(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                Assert.That(
                    ReadSource(sourcePrefix, source),
                    Does.Not.Match(@"\n[ \t]*\n[ \t]*[}\])]"),
                    $"Violated convention in {source}: no blank line may immediately precede a closing delimiter.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingFileEndings_ThenEachEndsWithAnEmptyLine(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                Assert.That(
                    ReadSource(sourcePrefix, source),
                    Does.EndWith(Environment.NewLine),
                    $"Violated convention in {source}: every file must end with an empty line.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNamespaceSpacing_ThenExactlyOneBlankLinePrecedesIt(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                NamespaceDeclarationSyntax? namespaceDeclaration = root.DescendantNodes()
                    .OfType<NamespaceDeclarationSyntax>()
                    .FirstOrDefault();

                if (namespaceDeclaration is null || root.Usings.Count == 0)
                {
                    continue;
                }

                int namespaceStartLine = namespaceDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line;
                int lastUsingLine = root.Usings.Last().GetLocation().GetLineSpan().EndLinePosition.Line;

                Assert.That(
                    namespaceStartLine - lastUsingLine - 1,
                    Is.EqualTo(1),
                    $"Violated convention in {namespaceDeclaration.Name}: exactly one blank line must separate the using directives from the namespace.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingUsingPlacement_ThenAllUsingsAreOutsideTheNamespace(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                UsingDirectiveSyntax[] usingsInsideNamespace = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<NamespaceDeclarationSyntax>()
                    .SelectMany(namespaceDeclaration => namespaceDeclaration.Usings)
                    .ToArray();

                Assert.That(
                    usingsInsideNamespace,
                    Is.Empty,
                    "Violated convention: all using directives must be placed above and outside the namespace.");
            }
        }

        private static string ReadSource(string sourcePrefix, string source)
            => ArchitectureResources.ReadResource(sourcePrefix, source);

        private static IEnumerable<string> CollectSources(string sourcePrefix)
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(sourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                $"Violated convention: no source files were found under '{sourcePrefix}'.");

            return sources;
        }

        private static IEnumerable<SyntaxTree> CollectParseTrees(string sourcePrefix)
            => CollectSources(sourcePrefix)
                .Select(source =>
                    CSharpSyntaxTree.ParseText(
                        ReadSource(sourcePrefix, source),
                        path: source));
    }
}
