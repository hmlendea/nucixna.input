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
    /// Source-file convention checks over the handwritten sources of NuciXNA.Input.
    /// </summary>
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        private const int MaxParameterCount = 7;

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNamespaces_ThenEveryFileUsesABlockNamespace(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>(),
                    Is.Not.Empty,
                    $"Violated convention in {source}: every file must declare a block namespace.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNamespaceForm_ThenNoneIsFileScoped(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<FileScopedNamespaceDeclarationSyntax>(),
                    Is.Empty,
                    $"Violated convention in {source}: file-scoped namespaces are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNullableDirectives_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.NullableDirectiveTrivia)).ToArray(),
                    Is.Empty,
                    $"Violated convention in {source}: the '#nullable' directive is prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingRegions_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantTrivia().Where(trivia =>
                        trivia.IsKind(SyntaxKind.RegionDirectiveTrivia) ||
                        trivia.IsKind(SyntaxKind.EndRegionDirectiveTrivia)).ToArray(),
                    Is.Empty,
                    $"Violated convention in {source}: regions are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingExplicitTypes_ThenNoVarKeywordIsUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                VariableDeclarationSyntax[] varDeclarations = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<VariableDeclarationSyntax>()
                    .Where(declaration => declaration.Type.IsVar)
                    .ToArray();

                Assert.That(
                    varDeclarations,
                    Is.Empty,
                    $"Violated convention in {source}: the 'var' keyword must not be used.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingConditionalExpressions_ThenNoTernariesAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<ConditionalExpressionSyntax>(),
                    Is.Empty,
                    $"Violated convention in {source}: ternary expressions are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNullCoalescing_ThenNoneIsUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                BinaryExpressionSyntax[] coalescingExpressions = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<BinaryExpressionSyntax>()
                    .Where(expression => expression.IsKind(SyntaxKind.CoalesceExpression))
                    .ToArray();

                Assert.That(
                    coalescingExpressions,
                    Is.Empty,
                    $"Violated convention in {source}: the '??' operator is prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingTuples_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                SyntaxNode[] tupleNodes = tree.GetRoot()
                    .DescendantNodes()
                    .Where(node => node.IsKind(SyntaxKind.TupleType) ||
                        node.IsKind(SyntaxKind.TupleExpression))
                    .ToArray();

                Assert.That(
                    tupleNodes,
                    Is.Empty,
                    $"Violated convention in {source}: tuples are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingParameterModifiers_ThenNoRefOrOutIsUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                SyntaxNode[] refParameters = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .Where(parameter => parameter.Modifiers.Any(modifier =>
                        modifier.IsKind(SyntaxKind.RefKeyword) ||
                        modifier.IsKind(SyntaxKind.OutKeyword)))
                    .ToArray();

                Assert.That(
                    refParameters,
                    Is.Empty,
                    $"Violated convention in {source}: ref and out parameters are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingParameterDefaults_ThenNoOptionalParametersAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                SyntaxNode[] optionalParameters = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<ParameterSyntax>()
                    .Where(parameter => parameter.Default is not null)
                    .ToArray();

                Assert.That(
                    optionalParameters,
                    Is.Empty,
                    $"Violated convention in {source}: optional parameters are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingRecordDeclarations_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>(),
                    Is.Empty,
                    $"Violated convention in {source}: record types are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingTopLevelStatements_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<GlobalStatementSyntax>(),
                    Is.Empty,
                    $"Violated convention in {source}: top-level statements are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingGlobalUsings_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                Assert.That(
                    tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().Where(usingDirective =>
                        usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)).ToArray(),
                    Is.Empty,
                    $"Violated convention in {source}: global using directives are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingControlFlowBodies_ThenEveryBodyUsesBraces(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                SyntaxNode[] unbracedControlFlowBodies = tree.GetRoot()
                    .DescendantNodes()
                    .Where(node => node is IfStatementSyntax ifStatement && ifStatement.Statement is not BlockSyntax)
                    .ToArray();

                Assert.That(
                    unbracedControlFlowBodies,
                    Is.Empty,
                    $"Violated convention in {source}: every control-flow body must use braces.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingMethodParameters_ThenAtMostSevenAreUsed(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                SyntaxNode[] oversizedParameterLists = tree.GetRoot()
                    .DescendantNodes()
                    .Where(node => node is ParameterListSyntax parameterList && parameterList.Parameters.Count > MaxParameterCount)
                    .ToArray();

                Assert.That(
                    oversizedParameterLists,
                    Is.Empty,
                    $"Violated convention in {source}: methods must not exceed seven parameters.");
            }
        }

        private static SyntaxTree ParseTree(string sourcePrefix, string source)
            => CSharpSyntaxTree.ParseText(
                ArchitectureResources.ReadResource(sourcePrefix, source),
                path: source);

        private static IEnumerable<string> CollectSources(string sourcePrefix)
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(sourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                $"Violated convention: no source files were found under '{sourcePrefix}'.");

            return sources;
        }
    }
}
