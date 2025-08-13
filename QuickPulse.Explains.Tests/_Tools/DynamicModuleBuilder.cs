using System.Reflection;
using System.Reflection.Emit;

namespace QuickPulse.Explains.Tests._Tools;

public class DynamicModuleBuilder
{
    public static ModuleBuilder Create()
    {
        var asmName = new AssemblyName("DocIncludeTestAssembly");
        var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        var module = asm.DefineDynamicModule("Main");
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            if (args.Name.StartsWith(asm.FullName!))
                return asm;
            return null;
        };
        return module;
    }
}
