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
    /// Test-source convention checks over the handwritten test sources.
    /// </summary>
    [TestFixture]
    public sealed class TestSourceConventionTests
    {
        [Test]
        public void GivenTestSources_WhenCheckingFixtures_ThenEachUsesASubjectNameAndTheFixtureAttribute()
        {
            ClassDeclarationSyntax[] invalidFixtures = CollectTestClasses()
                .Where(HasTestMethods)
                .Where(declaration => !declaration.Identifier.ValueText.EndsWith("Tests", StringComparison.Ordinal) ||
                    !HasAttribute(declaration.AttributeLists, "TestFixture"))
                .ToArray();

            Assert.That(
                invalidFixtures,
                Is.Empty,
                "Violated convention: every test fixture needs a subject-based 'Tests' name and the TestFixture attribute.");
        }

        [Test]
        public void GivenUnitTestSources_WhenCheckingMethodNames_ThenTheyFollowGivenWhenThen()
        {
            string[] invalidNames = CollectTestClasses(ArchitectureResources.UnitTestSourcePrefix)
                .SelectMany(declaration => declaration.Members.OfType<MethodDeclarationSyntax>())
                .Where(IsTestMethod)
                .Select(method => method.Identifier.ValueText)
                .Where(name => !HasGivenWhenThenName(name))
                .ToArray();

            Assert.That(
                invalidNames,
                Is.Empty,
                $"Violated convention: every unit test needs a Given..._When..._Then... name ({string.Join(", ", invalidNames)}).");
        }

        [Test]
        public void GivenTestSources_WhenCheckingParameterisedTests_ThenEachDeclaresAtLeastTwoCases()
        {
            MethodDeclarationSyntax[] singleCaseTests = CollectTestClasses()
                .SelectMany(declaration => declaration.Members.OfType<MethodDeclarationSyntax>())
                .Where(method => CountAttribute(method.AttributeLists, "TestCase") == 1)
                .ToArray();

            Assert.That(
                singleCaseTests,
                Is.Empty,
                "Violated convention: a parameterised test must declare at least two cases.");
        }

        [Test]
        public void GivenTestSources_WhenCheckingAssertions_ThenEveryBusinessTestAsserts()
        {
            MethodDeclarationSyntax[] assertionFreeTests = CollectTestClasses(ArchitectureResources.UnitTestSourcePrefix)
                .SelectMany(declaration => declaration.Members.OfType<MethodDeclarationSyntax>())
                .Where(IsTestMethod)
                .Where(method => !ContainsAssertion(method))
                .ToArray();

            Assert.That(
                assertionFreeTests,
                Is.Empty,
                "Violated convention: every business unit test needs an assertion or a verification.");
        }

        [Test]
        public void GivenTestSources_WhenCheckingAssertions_ThenNoneUseLegacyAssertions()
        {
            string[] legacyAssertions = CollectTestClasses()
                .SelectMany(declaration => declaration.DescendantNodes().OfType<InvocationExpressionSyntax>())
                .Where(invocation => invocation.Expression is MemberAccessExpressionSyntax)
                .Select(invocation => invocation.Expression.ToString())
                .Where(name => name.StartsWith("Assert.", StringComparison.Ordinal))
                .Where(name => name is "Assert.AreEqual" or "Assert.IsTrue" or "Assert.IsFalse" or
                    "Assert.IsNotNull" or "Assert.IsNull" or "Assert.AreNotEqual")
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            Assert.That(
                legacyAssertions,
                Is.Empty,
                $"Violated convention: the NUnit constraint model must be used ({string.Join(", ", legacyAssertions)}).");
        }

        [Test]
        public void GivenTestSources_WhenCheckingReflection_ThenNoNonPublicMembersAreInspected()
        {
            FieldDeclarationSyntax[] reflectionFields = CollectTestClasses()
                .SelectMany(declaration => declaration.DescendantNodes().OfType<FieldDeclarationSyntax>())
                .Where(field => field.Declaration.Type is IdentifierNameSyntax identifier &&
                    string.Equals(identifier.Identifier.ValueText, "BindingFlags", StringComparison.Ordinal))
                .ToArray();

            Assert.That(
                reflectionFields,
                Is.Empty,
                "Violated convention: unit tests must not use reflection to test non-public code.");
        }

        [Test]
        public void GivenTestSources_WhenCheckingHelpers_ThenNoDummyTypesAreNestedInFixtures()
        {
            TypeDeclarationSyntax[] nestedTypes = CollectTestClasses()
                .SelectMany(declaration => declaration.DescendantNodes().OfType<TypeDeclarationSyntax>())
                .ToArray();

            Assert.That(
                nestedTypes,
                Is.Empty,
                "Violated convention: dummy implementations must live in separate top-level files.");
        }

        [Test]
        public void GivenTestSources_WhenCheckingFixtureAccessibility_ThenEachFixtureIsPublicAndSealedInArchitectureTests()
        {
            ClassDeclarationSyntax[] invalidArchitectureFixtures = CollectTestClasses(
                    ArchitectureResources.ArchitectureTestSourcePrefix)
                .Where(HasTestMethods)
                .Where(declaration => !declaration.Modifiers.Any(SyntaxKind.PublicKeyword) ||
                    !declaration.Modifiers.Any(SyntaxKind.SealedKeyword))
                .ToArray();

            Assert.That(
                invalidArchitectureFixtures,
                Is.Empty,
                "Violated convention: architecture test fixtures must be public and sealed.");
        }

        private static IEnumerable<ClassDeclarationSyntax> CollectTestClasses()
            => CollectTestClasses(ArchitectureResources.UnitTestSourcePrefix)
                .Concat(CollectTestClasses(ArchitectureResources.ArchitectureTestSourcePrefix));

        private static IEnumerable<ClassDeclarationSyntax> CollectTestClasses(string sourcePrefix)
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(sourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                $"Violated convention: no source files were found under '{sourcePrefix}'.");

            return sources.SelectMany(source => ParseTree(sourcePrefix, source)
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>());
        }

        private static SyntaxTree ParseTree(string sourcePrefix, string source)
            => CSharpSyntaxTree.ParseText(
                ArchitectureResources.ReadResource(sourcePrefix, source),
                path: source);

        private static bool HasTestMethods(ClassDeclarationSyntax declaration)
            => declaration.Members.OfType<MethodDeclarationSyntax>().Any(IsTestMethod);

        private static bool IsTestMethod(MethodDeclarationSyntax method)
            => HasAttribute(method.AttributeLists, "Test") ||
                HasAttribute(method.AttributeLists, "TestCase") ||
                HasAttribute(method.AttributeLists, "TestCaseSource");

        private static bool ContainsAssertion(MethodDeclarationSyntax method)
            => method.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(invocation => invocation.Expression is MemberAccessExpressionSyntax)
                .Select(invocation => invocation.Expression.ToString())
                .Any(name => name.StartsWith("Assert.", StringComparison.Ordinal) ||
                    name.StartsWith("Mock.", StringComparison.Ordinal));

        private static bool HasAttribute(
            SyntaxList<AttributeListSyntax> attributeLists,
            string name)
            => attributeLists
                .SelectMany(list => list.Attributes)
                .Any(attribute => IsAttribute(attribute, name));

        private static int CountAttribute(
            SyntaxList<AttributeListSyntax> attributeLists,
            string name)
            => attributeLists
                .SelectMany(list => list.Attributes)
                .Count(attribute => IsAttribute(attribute, name));

        private static bool IsAttribute(AttributeSyntax attribute, string name)
            => string.Equals(
                attribute.Name.ToString(),
                name,
                StringComparison.Ordinal) ||
                string.Equals(
                    attribute.Name.ToString(),
                    name + "Attribute",
                    StringComparison.Ordinal);

        private static bool HasGivenWhenThenName(string name)
        {
            int whenPosition = name.IndexOf("_When", StringComparison.Ordinal);
            int thenPosition = name.IndexOf("_Then", StringComparison.Ordinal);

            return name.StartsWith("Given", StringComparison.Ordinal) &&
                whenPosition > 5 &&
                thenPosition > whenPosition + 5 &&
                thenPosition + 5 < name.Length;
        }
    }
}
