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
    /// Namespace and using-directive checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class NamespaceConventionTests
    {
        private static readonly string[] UsingGroupOrder =
        [
            "System",
            "Microsoft",
            "NuciXNA"
        ];

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingUsings_ThenNoGlobalUsingsAreDeclared(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                UsingDirectiveSyntax[] globalUsings = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<UsingDirectiveSyntax>()
                    .Where(usingDirective => usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
                    .ToArray();

                Assert.That(
                    globalUsings,
                    Is.Empty,
                    "Violated convention: global using directives are prohibited.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingUsings_ThenEveryDirectiveIsAlphabetisedInItsGroup(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                List<UsingDirectiveSyntax> currentGroup = [];
                List<string> violations = [];

                for (int index = 0; index < root.Usings.Count; index++)
                {
                    UsingDirectiveSyntax current = root.Usings[index];
                    currentGroup.Add(current);

                    bool isLastUsing = IsLastUsing(root, index);
                    bool isGroupBreak = !isLastUsing && StartsNewGroup(root, index);

                    if (!isLastUsing && !isGroupBreak)
                    {
                        continue;
                    }

                    violations.AddRange(GetOrderingViolations(currentGroup));
                    currentGroup = [];
                }

                Assert.That(
                    violations,
                    Is.Empty,
                    $"Violated convention: using directives must be alphabetised within each group ({string.Join(", ", violations)}).");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingUsings_ThenGroupsFollowTheDocumentedOrder(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                List<UsingDirectiveSyntax> currentGroup = [];
                List<int> groupRanks = [];

                for (int index = 0; index < root.Usings.Count; index++)
                {
                    UsingDirectiveSyntax current = root.Usings[index];
                    currentGroup.Add(current);

                    bool isLastUsing = IsLastUsing(root, index);
                    bool isGroupBreak = !isLastUsing && StartsNewGroup(root, index);

                    if (!isLastUsing && !isGroupBreak)
                    {
                        continue;
                    }

                    groupRanks.Add(GetUsingRank(currentGroup[0].Name.ToString()));
                    currentGroup = [];
                }

                int[] orderedGroupRanks = groupRanks
                    .Where(rank => rank >= 0)
                    .ToArray();

                Assert.That(
                    orderedGroupRanks,
                    Is.Ordered,
                    "Violated convention: using groups must follow the System, Microsoft, then solution order.");            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingFullyQualifiedNames_ThenNoInlineBclTypeIsUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] qualifiedBclNames = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<QualifiedNameSyntax>()
                    .Where(IsBclTypeUsage)
                    .Select(name => name.ToString())
                    .ToArray();

                Assert.That(
                    qualifiedBclNames,
                    Is.Empty,
                    $"Violated convention: built-in type aliases must be used instead of {string.Join(", ", qualifiedBclNames)}.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingNamespaces_ThenEveryFileDeclaresExactlyOne(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                SyntaxNode[] namespaceDeclarations = tree.GetRoot()
                    .DescendantNodes()
                    .Where(node => node is BaseNamespaceDeclarationSyntax)
                    .ToArray();

                Assert.That(
                    namespaceDeclarations,
                    Has.Length.EqualTo(1),
                    "Violated convention: every file must declare exactly one namespace.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingUsings_ThenNoneAreDuplicated(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] duplicateUsings = tree.GetCompilationUnitRoot()
                    .Usings
                    .Select(usingDirective => usingDirective.Name.ToString())
                    .GroupBy(name => name, StringComparer.Ordinal)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToArray();

                Assert.That(
                    duplicateUsings,
                    Is.Empty,
                    $"Violated convention: duplicate using directives are prohibited ({string.Join(", ", duplicateUsings)}).");
            }
        }

        private static IEnumerable<string> GetOrderingViolations(IEnumerable<UsingDirectiveSyntax> group)
        {
            string[] names = group
                .Select(usingDirective => usingDirective.Name.ToString())
                .ToArray();

            string[] orderedNames = names
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(name => name, StringComparer.Ordinal)
                .ToArray();

            for (int index = 0; index < names.Length; index++)
            {
                if (!string.Equals(names[index], orderedNames[index], StringComparison.Ordinal))
                {
                    yield return $"{names[index]} must follow {orderedNames[index]}";
                }
            }
        }

        private static bool HasBlankLineBetween(SyntaxNode first, SyntaxNode second)
        {
            bool firstIsStatic = first is UsingDirectiveSyntax firstUsing &&
                firstUsing.StaticKeyword.IsKind(SyntaxKind.StaticKeyword);

            bool secondIsStatic = second is UsingDirectiveSyntax secondUsing &&
                secondUsing.StaticKeyword.IsKind(SyntaxKind.StaticKeyword);

            if (firstIsStatic || secondIsStatic)
            {
                return true;
            }

            int firstEndLine = first.GetLocation().GetLineSpan().EndLinePosition.Line;
            int secondStartLine = second.GetLocation().GetLineSpan().StartLinePosition.Line;

            return secondStartLine - firstEndLine > 1;
        }

        private static bool IsLastUsing(CompilationUnitSyntax root, int index)
            => index == root.Usings.Count - 1;

        private static bool StartsNewGroup(CompilationUnitSyntax root, int index)
            => HasBlankLineBetween(root.Usings[index], root.Usings[index + 1]);

        private static int GetUsingRank(string name)
        {
            for (int index = 0; index < UsingGroupOrder.Length; index++)
            {
                if (name.Equals(UsingGroupOrder[index], StringComparison.Ordinal) ||
                    name.StartsWith(UsingGroupOrder[index] + ".", StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return -1;
        }

        private static bool IsBclTypeUsage(QualifiedNameSyntax name)
        {
            string left = name.Left.ToString();

            if (name.Ancestors().OfType<UsingDirectiveSyntax>().Any())
            {
                return false;
            }

            return left.StartsWith("System", StringComparison.Ordinal);
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
    }
}
