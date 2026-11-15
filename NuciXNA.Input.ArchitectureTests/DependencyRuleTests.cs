using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;

using NUnit.Framework;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace NuciXNA.Input.ArchitectureTests
{
    /// <summary>
    /// Dependency and boundary checks over the loaded production architecture.
    /// </summary>
    [TestFixture]
    public sealed class DependencyRuleTests
    {
        private static readonly string[] ExpectedPublicTypes =
        [
            "NuciXNA.Input.ButtonState",
            "NuciXNA.Input.GamepadButtonEventArgs",
            "NuciXNA.Input.GamepadButtonEventHandler",
            "NuciXNA.Input.IInputManager",
            "NuciXNA.Input.InputManager",
            "NuciXNA.Input.KeyboardKeyEventArgs",
            "NuciXNA.Input.KeyboardKeyEventHandler",
            "NuciXNA.Input.MouseButton",
            "NuciXNA.Input.MouseButtonEventArgs",
            "NuciXNA.Input.MouseButtonEventHandler",
            "NuciXNA.Input.MouseEventArgs",
            "NuciXNA.Input.MouseEventHandler"
        ];

        private static readonly string[] AllowedReferences =
        [
            "MonoGame.Framework",
            "NuciXNA.Primitives",
            "System.Collections",
            "System.Linq",
            "System.Runtime",
            "System.Threading"
        ];

        private static readonly string[] EnumerationClassTypes =
        [
            "NuciXNA.Input.ButtonState",
            "NuciXNA.Input.MouseButton"
        ];

        [Test]
        public void GivenProductionArchitecture_WhenLoaded_ThenOnlyTheProductionAssemblyIsModelled()
        {
            string[] modelledAssemblies = ProductionArchitecture.Model.Assemblies
                .Select(assembly => assembly.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            Assert.That(
                modelledAssemblies,
                Is.EquivalentTo([ProductionArchitecture.ProductionAssembly.GetName().Name]),
                "Violated boundary: only the production assembly must be modelled.");
        }

        [Test]
        public void GivenProductionAssembly_WhenInspectingExportedTypes_ThenThePublicSurfaceMatchesTheDocumentation()
        {
            string[] exportedTypes = GetExportedTypeNames();

            Assert.That(
                exportedTypes,
                Is.EquivalentTo(ExpectedPublicTypes),
                "Violated boundary: the public surface must match the documented API.");
        }

        [Test]
        public void GivenProductionTypes_WhenCheckingNamespacePlacement_ThenAllResideInTheDocumentedNamespace()
        {
            string[] unexpectedTypes = GetProductionTypeNames()
                .Where(name => !name.StartsWith("NuciXNA.Input.", StringComparison.Ordinal))
                .ToArray();

            Assert.That(
                unexpectedTypes,
                Is.Empty,
                "Violated boundary: every production type must reside in the NuciXNA.Input namespace.");
        }

        [Test]
        public void GivenProductionAssembly_WhenCheckingReferences_ThenOnlyTheAllowedAssembliesAreReferenced()
        {
            string[] references = GetReferencedAssemblyNames();
            string[] unexpectedReferences = references
                .Where(reference => !IsAllowedReference(reference))
                .ToArray();

            AssertReferencesAreInspectable(references);
            Assert.That(
                unexpectedReferences,
                Is.Empty,
                "Violated boundary: the production assembly must only" +
                " reference the documented dependencies.");
        }

        [TestCase("NUnit")]
        [TestCase("Moq")]
        [TestCase("ArchUnit")]
        [TestCase("Microsoft.AspNetCore")]
        [TestCase("Microsoft.EntityFrameworkCore")]
        [TestCase("Amazon.DynamoDBv2")]
        [TestCase("Dapper")]
        [TestCase("MediatR")]
        [TestCase("System.Net")]
        [TestCase("System.IO")]
        public void GivenProductionAssembly_WhenCheckingReferences_ThenForbiddenAssembliesAreAbsent(
            string forbiddenAssembly)
        {
            string[] references = GetReferencedAssemblyNames();
            string[] matchingReferences = references
                .Where(reference => reference.Contains(
                    forbiddenAssembly,
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();

            AssertReferencesAreInspectable(references);
            Assert.That(
                matchingReferences,
                Is.Empty,
                $"Violated boundary: the production assembly must not reference " +
                $"{forbiddenAssembly}.");
        }

        [Test]
        public void GivenProductionTypes_WhenCheckingExternalDependencies_ThenNoPersistenceOrWebFrameworkIsUsed()
        {
            IArchRule noFrameworkRule = Types()
                .That()
                .Are(ProductionArchitecture.ProductionTypes)
                .Should()
                .NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching(@"Microsoft\.EntityFrameworkCore\..*")
                .AndShould()
                .NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching(@"Microsoft\.AspNetCore\..*")
                .AndShould()
                .NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching(@"Amazon\.DynamoDBv2\..*")
                .AndShould()
                .NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching(@"Dapper\..*")
                .AndShould()
                .NotDependOnAnyTypesThat()
                .ResideInNamespaceMatching(@"MediatR\..*")
                .Because("the library provides input management and must stay framework-agnostic");

            AssertProductionTypesSelected();

            AssertNoViolations(noFrameworkRule);
        }

        [Test]
        public void GivenProductionAssembly_WhenCheckingFriendAssemblies_ThenInternalsAreNotExposed()
        {
            InternalsVisibleToAttribute[] friendAssemblies = ProductionArchitecture.ProductionAssembly
                .GetCustomAttributes<InternalsVisibleToAttribute>()
                .ToArray();

            Assert.That(
                friendAssemblies,
                Is.Empty,
                "Violated boundary: production code must not declare InternalsVisibleTo.");
        }

        [Test]
        public void GivenInputManager_WhenInspectingDeclaration_ThenItIsASealedClassImplementingItsInterface()
        {
            Type inputManagerType = GetProductionType("NuciXNA.Input.InputManager");

            Assert.That(
                inputManagerType.IsClass,
                Is.True,
                "Violated boundary: InputManager must be a class.");
            Assert.That(
                inputManagerType.IsSealed,
                Is.True,
                "Violated boundary: InputManager must be sealed.");
            Assert.That(
                typeof(IInputManager).IsAssignableFrom(inputManagerType),
                Is.True,
                "Violated boundary: InputManager must implement IInputManager.");
        }

        [Test]
        public void GivenEnumerationClassTypes_WhenInspectingDeclarations_ThenEachIsASealedValueEqualityClass()
        {
            foreach (string typeName in EnumerationClassTypes)
            {
                Type enumerationType = GetProductionType(typeName);
                bool implementsEquatable = enumerationType
                    .GetInterfaces()
                    .Any(interfaceType => interfaceType.IsGenericType
                        && interfaceType.GetGenericTypeDefinition() == typeof(IEquatable<>)
                        && interfaceType.GenericTypeArguments[0] == enumerationType);

                Assert.That(
                    enumerationType.IsClass,
                    Is.True,
                    $"Violated boundary: {typeName} must be a class.");
                Assert.That(
                    enumerationType.IsSealed,
                    Is.True,
                    $"Violated boundary: {typeName} must be sealed.");
                Assert.That(
                    implementsEquatable,
                    Is.True,
                    $"Violated boundary: {typeName} must implement IEquatable<{enumerationType.Name}>.");
            }
        }

        [Test]
        public void GivenEventArgumentTypes_WhenInspectingDeclarations_ThenEachIsASealedClass()
        {
            string[] eventArgumentTypes =
            [
                "NuciXNA.Input.GamepadButtonEventArgs",
                "NuciXNA.Input.KeyboardKeyEventArgs",
                "NuciXNA.Input.MouseButtonEventArgs",
                "NuciXNA.Input.MouseEventArgs"
            ];

            foreach (string typeName in eventArgumentTypes)
            {
                Type eventArgumentType = GetProductionType(typeName);

                Assert.That(
                    eventArgumentType.IsClass,
                    Is.True,
                    $"Violated boundary: {typeName} must be a class.");
                Assert.That(
                    eventArgumentType.IsSealed,
                    Is.True,
                    $"Violated boundary: {typeName} must be sealed.");
            }
        }

        [Test]
        public void GivenEventHandlerDelegateTypes_WhenInspectingDeclarations_ThenEachIsPublic()
        {
            string[] handlerTypes =
            [
                "NuciXNA.Input.GamepadButtonEventHandler",
                "NuciXNA.Input.KeyboardKeyEventHandler",
                "NuciXNA.Input.MouseButtonEventHandler",
                "NuciXNA.Input.MouseEventHandler"
            ];

            foreach (string typeName in handlerTypes)
            {
                Type handlerType = GetProductionType(typeName);

                Assert.That(
                    handlerType.IsSubclassOf(typeof(Delegate)),
                    Is.True,
                    $"Violated boundary: {typeName} must be a delegate.");
                Assert.That(
                    handlerType.IsPublic,
                    Is.True,
                    $"Violated boundary: {typeName} must be public.");
            }
        }

        private static void AssertProductionTypesSelected()
            => Assert.That(
                ProductionArchitecture.ProductionTypes.GetObjects(ProductionArchitecture.Model),
                Is.Not.Empty,
                "Vacuous-selector guard: no production types were selected.");

        private static void AssertNoViolations(IArchRule archRule)
        {
            EvaluationResult[] failures = archRule
                .Evaluate(ProductionArchitecture.Model)
                .Where(result => !result.Passed)
                .ToArray();

            Assert.That(
                failures,
                Is.Empty,
                "Violated boundary: " + archRule.Description);
        }

        private static bool IsAllowedReference(string reference)
            => AllowedReferences.Contains(reference, StringComparer.Ordinal);

        private static void AssertReferencesAreInspectable(string[] references)
            => Assert.That(
                references,
                Is.Not.Empty,
                "Violated boundary: the references must be inspectable.");

        private static string[] GetExportedTypeNames()
            => ProductionArchitecture.ProductionAssembly
                .GetExportedTypes()
                .Select(type => type.FullName)
                .OfType<string>()
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

        private static string[] GetReferencedAssemblyNames()
            => ProductionArchitecture.ProductionAssembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .OfType<string>()
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

        private static string[] GetProductionTypeNames()
            => ProductionArchitecture.ProductionAssembly
                .GetTypes()
                .Where(type => !IsCompilerGenerated(type))
                .Select(type => type.FullName)
                .OfType<string>()
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

        private static Type GetProductionType(string fullName)
        {
            Type? productionType = ProductionArchitecture.ProductionAssembly.GetType(fullName);

            Assert.That(
                productionType,
                Is.Not.Null,
                $"Violated boundary: {fullName} is missing.");

            return productionType!;
        }

        private static bool IsCompilerGenerated(Type type)
            => type.Name.StartsWith('<') ||
                type.GetCustomAttribute<CompilerGeneratedAttribute>() is not null;
    }
}
