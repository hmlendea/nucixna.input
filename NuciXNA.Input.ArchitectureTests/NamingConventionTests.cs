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
    /// Naming and declaration checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class NamingConventionTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingBuiltInTypes_ThenAliasesAreUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] bclTypeNames = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Select(identifier => identifier.Identifier.ValueText)
                    .Where(IsBclTypeName)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();

                Assert.That(
                    bclTypeNames,
                    Is.Empty,
                    $"Violated convention: built-in type aliases must be used instead of {string.Join(", ", bclTypeNames)}.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingTypeNames_ThenClassesUsePascalCase(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] nonPascalClasses = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Select(declaration => declaration.Identifier.ValueText)
                    .Where(name => !IsPascalCase(name))
                    .ToArray();

                Assert.That(
                    nonPascalClasses,
                    Is.Empty,
                    $"Violated convention: every class must use PascalCase ({string.Join(", ", nonPascalClasses)}).");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingMethodNames_ThenMethodsUsePascalCase(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] nonPascalMethods = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Select(method => method.Identifier.ValueText)
                    .Where(name => !IsPascalCase(name))
                    .ToArray();

                Assert.That(
                    nonPascalMethods,
                    Is.Empty,
                    $"Violated convention: every method must use PascalCase ({string.Join(", ", nonPascalMethods)}).");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingPrivateFields_ThenNoUnderscorePrefixIsUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                VariableDeclaratorSyntax[] prefixedFields = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(field => field.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    .SelectMany(field => field.Declaration.Variables)
                    .Where(variable => variable.Identifier.ValueText.StartsWith('_'))
                    .ToArray();

                Assert.That(
                    prefixedFields,
                    Is.Empty,
                    "Violated convention: private fields must use camelCase without an underscore prefix.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingBooleanNames_ThenPermissiblePrefixesAreUsed(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                string[] forbiddenBooleanNames = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(property => property.Type is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.BoolKeyword))
                    .Select(property => property.Identifier.ValueText)
                    .Where(IsForbiddenBooleanName)
                    .ToArray();

                Assert.That(
                    forbiddenBooleanNames,
                    Is.Empty,
                    "Violated convention: boolean names must use Is/Has/Does/Are forms.");
            }
        }

        [Test]
        public void GivenUnitTestSources_WhenCheckingFixtureNames_ThenEachClassEndsWithTestsSuffix()
        {
            string[] testTypeNames = CollectParseTrees(ArchitectureResources.UnitTestSourcePrefix)
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                .Select(declaration => declaration.Identifier.ValueText)
                .ToArray();

            Assert.That(
                testTypeNames,
                Is.Not.Empty,
                "Violated convention: no unit test classes were found.");
            Assert.That(
                testTypeNames.Where(name => !name.EndsWith("Tests", StringComparison.Ordinal)),
                Is.Empty,
                "Violated convention: every unit test class name must end with the 'Tests' suffix.");
        }

        [Test]
        public void GivenUnitTestSources_WhenCheckingEachClass_ThenAnExportedProductionSubjectExists()
        {
            string[] productionTypeNames = ProductionArchitecture.ProductionAssembly
                .GetExportedTypes()
                .Select(type => type.Name)
                .ToArray();

            foreach (SyntaxTree tree in CollectParseTrees(ArchitectureResources.UnitTestSourcePrefix))
            {
                foreach (ClassDeclarationSyntax classDeclaration in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    string subjectName = classDeclaration.Identifier.ValueText
                        .Replace("Tests", string.Empty, StringComparison.Ordinal);

                    Assert.That(
                        productionTypeNames,
                        Does.Contain(subjectName),
                        $"Violated convention: {classDeclaration.Identifier.ValueText} must test an exported production type.");
                }
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
                .Select(path => CSharpSyntaxTree.ParseText(
                    ArchitectureResources.ReadResource(sourcePrefix, path),
                    path: path));
        }

        private static bool IsBclTypeName(string name)
            => name is "String" or "Int32" or "Boolean" or "Object" or "Byte" or "Char" or
                "Int64" or "Double" or "Single" or "Decimal" or "UInt32" or "UInt64" or
                "Int16" or "UInt16";

        private static bool IsPascalCase(string name)
            => !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);

        private static bool IsForbiddenBooleanName(string name)
        {
            string[] lowercaseForbidden = ["flag", "check", "result"];

            return lowercaseForbidden.Contains(name.ToLowerInvariant(), StringComparer.Ordinal);
        }
    }
}
