using System.Reflection;
using System.Reflection.Emit;

namespace QuickPulse.Explains.Tests._Tools;

public static class DynamicModuleBuilder
{
    public static DynamicModuleScope Create()
    {
        var asmName = new AssemblyName("Dynamic_" + Guid.NewGuid().ToString("N"));
        var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        return new DynamicModuleScope(asm, asm.DefineDynamicModule("Main"));
    }
}

public sealed class DynamicModuleScope : IDisposable
{
    private readonly AssemblyBuilder assembly;
    private readonly ResolveEventHandler resolver;
    private bool disposed;

    internal DynamicModuleScope(AssemblyBuilder assembly, ModuleBuilder module)
    {
        this.assembly = assembly;
        Module = module;
        resolver = ResolveDynamicAssembly;
        AppDomain.CurrentDomain.AssemblyResolve += resolver;
    }

    public ModuleBuilder Module { get; }

    public static implicit operator ModuleBuilder(DynamicModuleScope scope) => scope.Module;

    public void Dispose()
    {
        if (disposed)
            return;

        AppDomain.CurrentDomain.AssemblyResolve -= resolver;
        disposed = true;
    }

    private Assembly? ResolveDynamicAssembly(object? sender, ResolveEventArgs args)
    {
        var requestedAssembly = new AssemblyName(args.Name);
        return AssemblyName.ReferenceMatchesDefinition(requestedAssembly, assembly.GetName())
            ? assembly
            : null;
    }
}
