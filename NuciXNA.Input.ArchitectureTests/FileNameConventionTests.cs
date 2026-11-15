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
    /// Declaration-placement checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class FileNameConventionTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingDeclaredTypes_ThenEachFileDeclaresExactlyOneType(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                List<string> declaredTypes = GetDeclaredTypeNames(tree).ToList();

                Assert.That(
                    declaredTypes,
                    Has.Count.EqualTo(1),
                    $"Violated convention in {source}: expected exactly one declared type, but found {declaredTypes.Count}.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingTypeNames_ThenEachMatchesItsFileNameExactly(string sourcePrefix)
        {
            foreach (string source in CollectSources(sourcePrefix))
            {
                SyntaxTree tree = ParseTree(sourcePrefix, source);
                string fileNameStem = source.Substring(source.LastIndexOf('/') + 1);

                fileNameStem = fileNameStem.Substring(0, fileNameStem.LastIndexOf('.'));

                string[] typeNames = GetDeclaredTypeNames(tree).ToArray();

                foreach (string typeName in typeNames)
                {
                    string ungenericTypeName = RemoveGenericArity(typeName);

                    Assert.That(
                        ungenericTypeName,
                        Is.EqualTo(fileNameStem),
                        $"Violated convention in {source}: declared type '{typeName}' " +
                        $"does not match the file name stem '{fileNameStem}'.");
                }
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

        private static IEnumerable<string> GetDeclaredTypeNames(SyntaxTree tree)
            => tree.GetRoot()
                .DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>()
                .Select(declaration => declaration.Identifier.ValueText)
                .Concat(tree.GetRoot()
                    .DescendantNodes()
                    .OfType<DelegateDeclarationSyntax>()
                    .Select(delegateDeclaration => delegateDeclaration.Identifier.ValueText));

        private static string RemoveGenericArity(string typeName)
        {
            int arityIndex = typeName.IndexOf('`');

            if (arityIndex < 0)
            {
                return typeName;
            }

            return typeName.Substring(0, arityIndex);
        }
    }
}
