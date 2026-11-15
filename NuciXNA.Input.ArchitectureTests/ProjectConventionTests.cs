using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;

namespace NuciXNA.Input.ArchitectureTests
{
    /// <summary>
    /// Project file convention checks.
    /// </summary>
    [TestFixture]
    public sealed class ProjectConventionTests
    {
        private static readonly string ProductionProjectPath = "NuciXNA.Input/NuciXNA.Input.csproj";
        private static readonly string UnitTestProjectPath = "NuciXNA.Input.UnitTests/NuciXNA.Input.UnitTests.csproj";
        private static readonly string ArchitectureTestProjectPath = "NuciXNA.Input.ArchitectureTests/NuciXNA.Input.ArchitectureTests.csproj";

        [TestCase("NuciXNA.Input/NuciXNA.Input.csproj")]
        [TestCase("NuciXNA.Input.UnitTests/NuciXNA.Input.UnitTests.csproj")]
        [TestCase("NuciXNA.Input.ArchitectureTests/NuciXNA.Input.ArchitectureTests.csproj")]
        public void GivenProjects_WhenLoaded_ThenTheRequiredPropertiesAreDeclared(string projectPath)
        {
            XDocument project = LoadProject(projectPath);

            Assert.That(
                GetProperty(project, "TargetFramework"),
                Is.EqualTo("net10.0"),
                $"{projectPath}: the target framework must be net10.0.");

            Assert.That(
                GetProperty(project, "ImplicitUsings"),
                Is.EqualTo("disable"),
                $"{projectPath}: implicit usings must be disabled.");

            Assert.That(
                GetProperty(project, "LangVersion"),
                Is.EqualTo("latest"),
                $"{projectPath}: the latest language version must be used.");

            Assert.That(
                GetProperty(project, "Nullable"),
                Is.EqualTo("enable"),
                $"{projectPath}: nullable references must be explicitly enabled.");
        }

        [TestCase("NuciXNA.Input/NuciXNA.Input.csproj")]
        [TestCase("NuciXNA.Input.UnitTests/NuciXNA.Input.UnitTests.csproj")]
        [TestCase("NuciXNA.Input.ArchitectureTests/NuciXNA.Input.ArchitectureTests.csproj")]
        public void GivenProjects_WhenLoaded_ThenPackageReferencesAreAlphabetical(string projectPath)
        {
            XDocument project = LoadProject(projectPath);

            string[] packages = project.Descendants("PackageReference")
                .Select(reference => GetAttributeValue(reference, "Include"))
                .ToArray();

            Assert.That(
                packages,
                Is.Not.Empty,
                $"{projectPath}: no package references were found.");
            Assert.That(
                packages,
                Is.EqualTo(packages.OrderBy(package => package, StringComparer.Ordinal).ToArray()),
                $"{projectPath}: package references must be ordered alphabetically.");
        }

        [TestCase("NuciXNA.Input.UnitTests/NuciXNA.Input.UnitTests.csproj")]
        [TestCase("NuciXNA.Input.ArchitectureTests/NuciXNA.Input.ArchitectureTests.csproj")]
        public void GivenTestProjects_WhenLoaded_ThenTheTestFlagsAreDeclared(string projectPath)
        {
            XDocument project = LoadProject(projectPath);

            Assert.That(
                GetProperty(project, "IsTestProject"),
                Is.EqualTo("true"),
                $"{projectPath}: the test project flag must be declared.");

            Assert.That(
                GetProperty(project, "ExcludeFromCodeCoverage"),
                Is.EqualTo("true"),
                $"{projectPath}: the code coverage exclusion flag must be declared.");

            Assert.That(
                GetProperty(project, "IsPackable"),
                Is.EqualTo("false"),
                $"{projectPath}: test projects must not be packable.");
        }

        [Test]
        public void GivenProductionProject_WhenLoaded_ThenItIsPackable()
        {
            XDocument project = LoadProject(ProductionProjectPath);

            Assert.That(
                GetProperty(project, "IsPackable"),
                Is.EqualTo("true"),
                "The production project is published as a NuGet package and must be packable.");
        }

        [Test]
        public void GivenProductionProject_WhenLoaded_ThenItDoesNotReferenceTestProjects()
        {
            XDocument project = LoadProject(ProductionProjectPath);

            string[] projectReferences = GetProjectReferences(project);

            Assert.That(
                projectReferences.Where(reference => reference.Contains("Tests", StringComparison.OrdinalIgnoreCase)),
                Is.Empty,
                "The production project must not reference test projects.");
        }

        [Test]
        public void GivenUnitTestProject_WhenLoaded_ThenItReferencesTheProductionProject()
        {
            XDocument project = LoadProject(UnitTestProjectPath);

            string[] projectReferences = GetProjectReferences(project);

            Assert.That(
                projectReferences,
                Has.Some.EqualTo("../NuciXNA.Input/NuciXNA.Input.csproj"),
                "The unit test project must reference the production project.");
        }

        [Test]
        public void GivenArchitectureTestProject_WhenLoaded_ThenItReferencesTheProductionProject()
        {
            XDocument project = LoadProject(ArchitectureTestProjectPath);

            string[] projectReferences = GetProjectReferences(project);

            Assert.That(
                projectReferences,
                Has.Some.EqualTo("../NuciXNA.Input/NuciXNA.Input.csproj"),
                "The architecture test project must reference the production project.");
        }

        private static XDocument LoadProject(string relativePath)
            => XDocument.Parse(
                ArchitectureResources.ReadResource(
                    ArchitectureResources.ProjectFilePrefix,
                    relativePath));

        private static string[] GetProjectReferences(XDocument project)
            => project.Descendants("ProjectReference")
                .Select(reference => GetAttributeValue(reference, "Include"))
                .Select(reference => reference.Replace('\\', '/'))
                .ToArray();

        private static string GetAttributeValue(XElement element, string attributeName)
        {
            XAttribute? attribute = element.Attribute(attributeName);

            if (attribute is null)
            {
                return string.Empty;
            }

            return attribute.Value;
        }

        private static string GetProperty(XDocument project, string name)
        {
            IEnumerable<XElement> elements = project.Descendants()
                .Where(element => string.Equals(element.Name.LocalName, name, StringComparison.Ordinal));
            XElement? element = elements.FirstOrDefault();

            if (element is null)
            {
                return string.Empty;
            }

            return element.Value;
        }
    }
}
