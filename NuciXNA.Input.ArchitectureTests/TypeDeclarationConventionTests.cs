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
    /// Type and interface declaration checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class TypeDeclarationConventionTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingPartialDeclarations_ThenNoneAreUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                TypeDeclarationSyntax[] partialTypes = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<TypeDeclarationSyntax>()
                    .Where(declaration => declaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword)))
                    .ToArray();

                Assert.That(
                    partialTypes,
                    Is.Empty,
                    "Violated convention: partial types are prohibited unless a framework contract requires them.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenCheckingSealing_ThenEveryClassIsSealedOrStatic()
        {
            foreach (SyntaxTree tree in CollectParseTrees(ArchitectureResources.ProductionSourcePrefix))
            {
                ClassDeclarationSyntax[] nonSealedClasses = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(declaration => !declaration.Modifiers.Any(modifier =>
                        modifier.IsKind(SyntaxKind.SealedKeyword) ||
                        modifier.IsKind(SyntaxKind.StaticKeyword) ||
                        modifier.IsKind(SyntaxKind.AbstractKeyword)))
                    .ToArray();

                Assert.That(
                    nonSealedClasses,
                    Is.Empty,
                    "Violated convention: every non-static class must be sealed.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenCheckingInterfaces_ThenEligibleClassesHaveAnEquivalentInterface()
        {
            HashSet<string> interfaceNames = CollectParseTrees(ArchitectureResources.ProductionSourcePrefix)
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                .Select(declaration => declaration.Identifier.ValueText)
                .ToHashSet(StringComparer.Ordinal);

            string[] missingInterfaces = CollectParseTrees(ArchitectureResources.ProductionSourcePrefix)
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .Where(declaration => declaration.Modifiers.Any(SyntaxKind.PublicKeyword))
                .Where(declaration => !declaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                .Where(declaration => !declaration.BaseList?.Types.Any() == true || declaration.BaseList is null)
                .Where(declaration => declaration.Members.OfType<MethodDeclarationSyntax>()
                    .Any(method => method.Modifiers.Any(SyntaxKind.PublicKeyword)))
                .Select(declaration => declaration.Identifier.ValueText)
                .Where(name => !interfaceNames.Contains($"I{name}"))
                .ToArray();

            Assert.That(
                missingInterfaces,
                Is.Empty,
                $"Violated convention: each eligible class must have an equivalent interface ({string.Join(", ", missingInterfaces)}).");
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingInterfaces_ThenEachUsesTheIPrefix(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] misnamedInterfaces = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .Where(IsMisnamedInterface)
                    .ToArray();

                Assert.That(
                    misnamedInterfaces,
                    Is.Empty,
                    "Violated convention: each interface must use the 'I' prefix followed by PascalCase.");
            }
        }

        private static IEnumerable<SyntaxTree> CollectParseTrees(string sourcePrefix)
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(sourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                $"Violated convention: no source files were found under '{sourcePrefix}'.");

            return sources
                .Select(source => CSharpSyntaxTree.ParseText(
                    ArchitectureResources.ReadResource(sourcePrefix, source),
                    path: source));
        }

        private static bool IsMisnamedInterface(string name)
            => name.Length < 2 || name[0] != 'I' || !char.IsUpper(name[1]);
    }
}
