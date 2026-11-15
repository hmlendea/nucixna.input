using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuciXNA.Input.ArchitectureTests
{
    internal static class ArchitectureResources
    {
        internal const string ProductionSourcePrefix = "ProductionSources/";

        internal const string UnitTestSourcePrefix = "UnitTestSources/";

        internal const string ArchitectureTestSourcePrefix = "ArchitectureTestSources/";

        internal const string ProjectFilePrefix = "ProjectFiles/";

        internal static IReadOnlyList<string> GetResourcePaths(string prefix)
            => typeof(ArchitectureResources).Assembly
                .GetManifestResourceNames()
                .Select(Normalise)
                .Where(name => name.StartsWith(prefix, StringComparison.Ordinal))
                .Select(name => name.Substring(prefix.Length))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

        internal static string ReadResource(string prefix, string relativePath)
        {
            string expectedName = Normalise(prefix + relativePath);

            foreach (string resourceName in typeof(ArchitectureResources).Assembly.GetManifestResourceNames())
            {
                if (!string.Equals(
                    Normalise(resourceName),
                    expectedName,
                    StringComparison.Ordinal))
                {
                    continue;
                }

                using Stream? stream = typeof(ArchitectureResources).Assembly
                    .GetManifestResourceStream(resourceName);

                if (stream is null)
                {
                    throw new InvalidOperationException(
                        $"The embedded resource '{resourceName}' could not be opened.");
                }

                using StreamReader reader = new(stream);

                return reader.ReadToEnd();
            }

            throw new InvalidOperationException($"The embedded resource '{expectedName}' was not found.");
        }

        private static string Normalise(string resourceName)
            => resourceName.Replace('\\', '/');
    }
}
