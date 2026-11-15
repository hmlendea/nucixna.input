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
    /// XML documentation checks over the production sources that form the NuGet surface.
    /// </summary>
    [TestFixture]
    public sealed class DocumentationConventionTests
    {
        [Test]
        public void GivenProductionTypes_WhenCheckingDocumentation_ThenEachPublicMemberHasAnXmlSummary()
        {
            MemberDeclarationSyntax[] undocumentedMembers = CollectProductionParseTrees()
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>())
                .Where(IsPubliclyDocumentableMember)
                .Where(member => member.Parent is not InterfaceDeclarationSyntax)
                .Where(member => DeclaresPublicAccessibility(member))
                .Where(member => !HasDocumentation(member))
                .ToArray();

            Assert.That(
                undocumentedMembers,
                Is.Empty,
                "Violated convention: every public NuGet member must carry XML documentation.");
        }

        [Test]
        public void GivenProductionInterfaces_WhenCheckingDocumentation_ThenEveryMemberHasAnXmlSummary()
        {
            MemberDeclarationSyntax[] undocumentedInterfaceMembers = CollectProductionParseTrees()
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                .SelectMany(declaration => declaration.Members)
                .Where(member => !HasDocumentation(member))
                .ToArray();

            Assert.That(
                undocumentedInterfaceMembers,
                Is.Empty,
                "Violated convention: every interface member must carry XML documentation.");
        }

        [Test]
        public void GivenProductionXmlDocumentation_WhenCheckingParameters_ThenEachParamTagNamesAParameter()
        {
            foreach (SyntaxTree tree in CollectProductionParseTrees())
            {
                foreach (MethodDeclarationSyntax method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    string[] documentedNames = GetDocumentedParameterNames(method);
                    string[] declaredNames = method.ParameterList.Parameters
                        .Select(parameter => parameter.Identifier.ValueText)
                        .ToArray();

                    Assert.That(
                        documentedNames.Except(declaredNames, StringComparer.Ordinal),
                        Is.Empty,
                        $"Violated convention: a <param> tag names no declared parameter of {method.Identifier.ValueText}.");
                }
            }
        }

        [Test]
        public void GivenProductionXmlDocumentation_WhenCheckingParameterTags_ThenNoneAreEmpty()
        {
            foreach (SyntaxTree tree in CollectProductionParseTrees())
            {
                foreach (MethodDeclarationSyntax method in tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    foreach (XmlElementSyntax parameterTag in GetParameterTags(method))
                    {
                        string description = parameterTag.Content.ToFullString().Trim();

                        Assert.That(
                            description,
                            Is.Not.Empty,
                            $"Violated convention: a <param> tag of {method.Identifier.ValueText} is empty.");
                    }
                }
            }
        }

        [Test]
        public void GivenProductionXmlDocumentation_WhenCheckingValueTags_ThenEveryGetterOnlyPropertyDocumentsItsValue()
        {
            PropertyDeclarationSyntax[] undocumentedValues = CollectProductionParseTrees()
                .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>())
                .Where(property => DeclaresPublicAccessibility(property))
                .Where(property => property.AccessorList is not null)
                .Where(property => property.AccessorList!.Accessors.Count == 1)
                .Where(property => !HasValueTag(property))
                .ToArray();

            Assert.That(
                undocumentedValues,
                Is.Empty,
                "Violated convention: every read-only property must document its value with a <value> tag.");
        }

        private static IEnumerable<SyntaxTree> CollectProductionParseTrees()
        {
            IReadOnlyList<string> sources = ArchitectureResources.GetResourcePaths(ArchitectureResources.ProductionSourcePrefix);

            Assert.That(
                sources,
                Is.Not.Empty,
                "Violated convention: no production source files were found.");

            return sources
                .Select(source => CSharpSyntaxTree.ParseText(
                    ArchitectureResources.ReadResource(
                        ArchitectureResources.ProductionSourcePrefix,
                        source),
                    path: source));
        }

        private static bool IsPubliclyDocumentableMember(MemberDeclarationSyntax member)
            => member is MethodDeclarationSyntax ||
                member is PropertyDeclarationSyntax ||
                member is ConstructorDeclarationSyntax ||
                member is EventFieldDeclarationSyntax ||
                member is ClassDeclarationSyntax ||
                member is StructDeclarationSyntax ||
                member is DelegateDeclarationSyntax;

        private static bool DeclaresPublicAccessibility(MemberDeclarationSyntax member)
        {
            SyntaxTokenList modifiers = member switch
            {
                MethodDeclarationSyntax method => method.Modifiers,
                PropertyDeclarationSyntax property => property.Modifiers,
                ConstructorDeclarationSyntax constructor => constructor.Modifiers,
                EventFieldDeclarationSyntax eventField => eventField.Modifiers,
                ClassDeclarationSyntax classDeclaration => classDeclaration.Modifiers,
                StructDeclarationSyntax structDeclaration => structDeclaration.Modifiers,
                DelegateDeclarationSyntax delegateDeclaration => delegateDeclaration.Modifiers,
                _ => default
            };

            return modifiers.Any(SyntaxKind.PublicKeyword);
        }

        private static bool HasDocumentation(MemberDeclarationSyntax member)
            => member.GetLeadingTrivia().Any(trivia =>
                trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));

        private static bool HasValueTag(PropertyDeclarationSyntax property)
            => property.GetLeadingTrivia()
                .Select(trivia => trivia.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .SelectMany(documentation => documentation.Content.OfType<XmlElementSyntax>())
                .Any(element => HasTagName(element, "value"));

        private static string[] GetDocumentedParameterNames(MethodDeclarationSyntax method)
            => GetParameterTags(method)
                .Select(tag => tag.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault())
                .Where(attribute => attribute is not null)
                .Select(attribute => attribute!.Identifier.Identifier.ValueText)
                .ToArray();

        private static IEnumerable<XmlElementSyntax> GetParameterTags(MethodDeclarationSyntax method)
            => method.GetLeadingTrivia()
                .Select(trivia => trivia.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .SelectMany(documentation => documentation.Content.OfType<XmlElementSyntax>())
                .Where(element => HasTagName(element, "param"));

        private static bool HasTagName(XmlElementSyntax element, string tagName)
            => element.StartTag.Name.LocalName.ValueText.Equals(
                tagName,
                StringComparison.Ordinal);
    }
}
