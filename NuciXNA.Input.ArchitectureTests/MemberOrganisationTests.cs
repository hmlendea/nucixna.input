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
    /// Member organisation checks over the handwritten sources.
    /// </summary>
    [TestFixture]
    public sealed class MemberOrganisationTests
    {
        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingAccessibility_ThenEveryMemberDeclaresItExplicitly(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                MemberDeclarationSyntax[] membersWithoutAccessibility = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<MemberDeclarationSyntax>()
                    .Where(IsAccessibilityScopedMember)
                    .Where(member => member.Parent is not InterfaceDeclarationSyntax)
                    .Where(member => !DeclaresAccessibility(member))
                    .ToArray();

                Assert.That(
                    membersWithoutAccessibility,
                    Is.Empty,
                    "Violated convention: every field, property, event, constructor, and method must declare explicit accessibility.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingKindOrder_ThenNoNonMethodFollowsAMethod(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                foreach (TypeDeclarationSyntax typeDeclaration in tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
                {
                    MemberDeclarationSyntax[] orderedMembers = typeDeclaration.Members
                        .Where(IsAccessibilityScopedMember)
                        .ToArray();

                    int lastMethodIndex = FindLastMethodIndex(orderedMembers);
                    int lastNonMethodIndex = FindLastNonMethodIndex(orderedMembers);

                    if (lastMethodIndex < 0 || lastNonMethodIndex < 0)
                    {
                        continue;
                    }

                    Assert.That(
                        lastNonMethodIndex,
                        Is.LessThan(lastMethodIndex),
                        $"Violated convention in {typeDeclaration.Identifier.ValueText}: fields, properties, events, and constructors must appear before methods and operators.");
                }
            }
        }

        private static int FindLastMethodIndex(MemberDeclarationSyntax[] members)
            => Array.FindLastIndex(
                members,
                member => member is MethodDeclarationSyntax);

        private static int FindLastNonMethodIndex(MemberDeclarationSyntax[] members)
            => Array.FindLastIndex(
                members,
                member => member is not MethodDeclarationSyntax);

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingSingleStatementMethods_ThenTheyAreExpressionBodied(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                MethodDeclarationSyntax[] blockBodiedSingleStatementMethods = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Body is not null)
                    .Where(method => method.Body!.Statements.Count == 1)
                    .Where(method => method.Body!.Statements[0] is ReturnStatementSyntax ||
                        method.Body!.Statements[0] is ThrowStatementSyntax)
                    .ToArray();

                Assert.That(
                    blockBodiedSingleStatementMethods,
                    Is.Empty,
                    "Violated convention: every single-statement method must be expression-bodied.");
            }
        }

        [TestCase(ArchitectureResources.ProductionSourcePrefix)]
        [TestCase(ArchitectureResources.UnitTestSourcePrefix)]
        [TestCase(ArchitectureResources.ArchitectureTestSourcePrefix)]
        public void GivenSources_WhenCheckingGetterOnlyProperties_ThenNoneUseABlockBody(string sourcePrefix)
        {
            foreach (SyntaxTree tree in CollectParseTrees(sourcePrefix))
            {
                PropertyDeclarationSyntax[] blockBodiedGetterProperties = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(property => property.ExpressionBody is null)
                    .Where(property => property.AccessorList is not null)
                    .Where(property => property.AccessorList!.Accessors.Count == 1)
                    .Where(property => property.AccessorList!.Accessors[0].Kind() == SyntaxKind.GetAccessorDeclaration)
                    .Where(property => property.AccessorList!.Accessors[0].Body is not null)
                    .ToArray();

                Assert.That(
                    blockBodiedGetterProperties,
                    Is.Empty,
                    "Violated convention: computed read-only properties must be expression-bodied.");
            }
        }

        [Test]
        public void GivenProductionSources_WhenCheckingConstants_ThenNoneAreDeclared()
        {
            foreach (SyntaxTree tree in CollectParseTrees(ArchitectureResources.ProductionSourcePrefix))
            {
                FieldDeclarationSyntax[] constFields = tree.GetRoot()
                    .DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(field => field.Modifiers.Any(SyntaxKind.ConstKeyword))
                    .ToArray();

                Assert.That(
                    constFields,
                    Is.Empty,
                    "Violated convention: production constants must be expressed as 'static' members, never const.");
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

        private static bool IsAccessibilityScopedMember(MemberDeclarationSyntax member)
            => member is FieldDeclarationSyntax ||
                member is PropertyDeclarationSyntax ||
                member is EventFieldDeclarationSyntax ||
                member is ConstructorDeclarationSyntax ||
                member is MethodDeclarationSyntax;

        // Narrowly scoped exception: interface members omit accessibility modifiers by
        // language convention, so the explicit-accessibility rule cannot apply to them.

        private static bool DeclaresAccessibility(MemberDeclarationSyntax member)
        {
            SyntaxTokenList modifiers = member switch
            {
                FieldDeclarationSyntax field => field.Modifiers,
                PropertyDeclarationSyntax property => property.Modifiers,
                EventFieldDeclarationSyntax eventField => eventField.Modifiers,
                ConstructorDeclarationSyntax constructor => constructor.Modifiers,
                MethodDeclarationSyntax method => method.Modifiers,
                _ => default
            };

            return modifiers.Any(modifier =>
                modifier.IsKind(SyntaxKind.PublicKeyword) ||
                modifier.IsKind(SyntaxKind.ProtectedKeyword) ||
                modifier.IsKind(SyntaxKind.InternalKeyword) ||
                modifier.IsKind(SyntaxKind.PrivateKeyword));
        }
    }
}
