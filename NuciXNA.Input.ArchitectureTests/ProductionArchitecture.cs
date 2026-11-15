using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

using Assembly = System.Reflection.Assembly;

using NuciXNA.Input;

namespace NuciXNA.Input.ArchitectureTests
{
    internal static class ProductionArchitecture
    {
        internal static Assembly ProductionAssembly { get; } = typeof(InputManager).Assembly;

        internal static Architecture Model { get; } = new ArchLoader()
            .LoadAssemblies(ProductionAssembly)
            .Build();

        internal static IObjectProvider<IType> ProductionTypes { get; } = Types()
            .That()
            .ResideInAssembly(ProductionAssembly)
            .As("NuciXNA.Input production types");
    }
}
