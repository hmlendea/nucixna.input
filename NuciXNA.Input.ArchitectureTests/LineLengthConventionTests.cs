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
    /// Line-splitting and width checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class LineLengthConventionTests
    {
        private const int MaxConditionalWidth = 72;
        private const int MaxSignatureWidth = 96;

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingConditionalStatements_ThenNoSingleLineConditionalExceedsTheLimit(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                foreach (IfStatementSyntax ifStatement in tree.GetRoot().DescendantNodes().OfType<IfStatementSyntax>())
                {
                    FileLinePositionSpan span = ifStatement.GetLocation().GetLineSpan();
                    int lineIndex = span.StartLinePosition.Line;

                    if (lineIndex != span.EndLinePosition.Line)
                    {
                        continue;
                    }

                    string line = GetSourceLine(sourcePrefix, source, lineIndex);

                    Assert.That(
                        line.Length,
                        Is.LessThanOrEqualTo(MaxConditionalWidth),
                        $"Violated convention in {source}:{lineIndex + 1}: a single-line conditional must not exceed {MaxConditionalWidth} characters.");
                }
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingInvocationArgumentLists_ThenNoInlineListExceedsTheLimit(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                foreach (ArgumentListSyntax argumentList in tree.GetRoot().DescendantNodes().OfType<ArgumentListSyntax>())
                {
                    if (argumentList.Arguments.Count < 2)
                    {
                        continue;
                    }

                    if (IsNestedInList(argumentList))
                    {
                        continue;
                    }

                    AssertInlineListFits(
                        sourcePrefix,
                        source,
                        argumentList.OpenParenToken,
                        argumentList.CloseParenToken,
                        argumentList.GetLocation().GetLineSpan(),
                        "argument list");

                    AssertMultiLineListIsOneItemPerLine(
                        sourcePrefix,
                        source,
                        argumentList,
                        "argument list");
                }
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingDeclarationParameterLists_ThenNoInlineListExceedsTheLimit(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                foreach (ParameterListSyntax parameterList in tree.GetRoot().DescendantNodes().OfType<ParameterListSyntax>())
                {
                    if (parameterList.Parameters.Count < 2)
                    {
                        continue;
                    }

                    if (IsNestedInList(parameterList))
                    {
                        continue;
                    }

                    AssertInlineListFits(
                        sourcePrefix,
                        source,
                        parameterList.OpenParenToken,
                        parameterList.CloseParenToken,
                        parameterList.GetLocation().GetLineSpan(),
                        "parameter list");

                    AssertMultiLineListIsOneItemPerLine(
                        sourcePrefix,
                        source,
                        parameterList,
                        "parameter list");
                }
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingExpressionBodies_ThenTheArrowIsPlacedOnItsOwnLineWhenNeeded(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);

                foreach (MemberDeclarationSyntax member in tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>())
                {
                    ArrowExpressionClauseSyntax? arrow = member switch
                    {
                        MethodDeclarationSyntax method => method.ExpressionBody,
                        PropertyDeclarationSyntax property => property.ExpressionBody,
                        OperatorDeclarationSyntax operatorDeclaration => operatorDeclaration.ExpressionBody,
                        ConversionOperatorDeclarationSyntax conversion => conversion.ExpressionBody,
                        _ => null
                    };

                    if (arrow is null)
                    {
                        continue;
                    }

                    FileLinePositionSpan signatureSpan = arrow.GetLocation().GetLineSpan();
                    int arrowLine = signatureSpan.StartLinePosition.Line;

                    if (arrowLine != signatureSpan.EndLinePosition.Line)
                    {
                        continue;
                    }

                    string line = GetSourceLine(sourcePrefix, source, arrowLine);
                    int signatureLength = line.IndexOf("=>", StringComparison.Ordinal);

                    if (signatureLength < 0 || line.Length <= MaxSignatureWidth)
                    {
                        continue;
                    }

                    Assert.That(
                        line.Length,
                        Is.LessThanOrEqualTo(MaxSignatureWidth),
                        $"Violated convention in {source}:{arrowLine + 1}: the '=>' token must be moved to the next line when the signature exceeds {MaxSignatureWidth} characters.");
                }
            }
        }

        private static bool IsNestedInList(SyntaxNode node)
            => node.Ancestors().Any(ancestor =>
                ancestor is ArgumentListSyntax || ancestor is ParameterListSyntax);

        private static void AssertInlineListFits(
            string sourcePrefix,
            string source,
            SyntaxToken openToken,
            SyntaxToken closeToken,
            FileLinePositionSpan span,
            string listDescription)
        {
            if (span.StartLinePosition.Line != span.EndLinePosition.Line)
            {
                return;
            }

            if (openToken.IsMissing || closeToken.IsMissing)
            {
                return;
            }

            string line = GetSourceLine(sourcePrefix, source, span.StartLinePosition.Line);

            Assert.That(
                line.Length,
                Is.LessThanOrEqualTo(MaxSignatureWidth),
                $"Violated convention in {source}:{span.StartLinePosition.Line + 1}: the inline {listDescription} must be split when its line exceeds {MaxSignatureWidth} characters.");
        }

        private static void AssertMultiLineListIsOneItemPerLine(
            string sourcePrefix,
            string source,
            SyntaxNode list,
            string listDescription)
        {
            FileLinePositionSpan span = list.GetLocation().GetLineSpan();

            if (span.StartLinePosition.Line == span.EndLinePosition.Line)
            {
                return;
            }

            string[] lines = ReadSource(sourcePrefix, source)
                .Replace("\r", string.Empty)
                .Split('\n');

            List<int> itemLines = GetItemLines(list);
            bool hasMultipleItemsOnOneLine = itemLines
                .GroupBy(line => line)
                .Any(group => group.Count() > 1);

            Assert.That(
                hasMultipleItemsOnOneLine,
                Is.False,
                $"Violated convention in {source}:{span.StartLinePosition.Line + 1}: every {listDescription} item must occupy its own line when wrapped.");
            Assert.That(
                lines.Length,
                Is.GreaterThan(0),
                $"Violated convention in {source}: the source must be readable.");
        }

        private static List<int> GetItemLines(SyntaxNode list)
        {
            List<SyntaxNode> items = list switch
            {
                ArgumentListSyntax arguments => arguments.Arguments.Cast<SyntaxNode>().ToList(),
                ParameterListSyntax parameters => parameters.Parameters.Cast<SyntaxNode>().ToList(),
                _ => []
            };

            return items
                .Select(item => item.GetLocation().GetLineSpan().StartLinePosition.Line)
                .ToList();
        }

        private static string GetSourceLine(string sourcePrefix, string source, int lineIndex)
            => ReadSource(sourcePrefix, source)
                .Replace("\r", string.Empty)
                .Split('\n')[lineIndex];

        private static string ReadSource(string sourcePrefix, string source)
            => ArchitectureResources.ReadResource(sourcePrefix, source);

        private static SyntaxTree ParseTree(string sourcePrefix, string source)
            => CSharpSyntaxTree.ParseText(ReadSource(sourcePrefix, source), path: source);

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
