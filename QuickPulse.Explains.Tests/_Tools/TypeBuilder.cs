using System.Reflection;
using System.Reflection.Emit;

namespace QuickPulse.Explains.Tests._Tools;

public class TypeBuilder
{
    private static readonly AssemblyBuilder Asm;
    private static readonly ModuleBuilder Mod;

    static TypeBuilder()
    {
        var asmName = new AssemblyName("DynamicTypesForTests");
        Asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        Mod = Asm.DefineDynamicModule("MainModule");
    }

    private readonly string _typeName;
    private readonly System.Reflection.Emit.TypeBuilder _tb;

    private TypeBuilder(string? typeName)
    {
        _typeName = typeName ?? $"Dynamic_{Guid.NewGuid():N}";
        _tb = Mod.DefineType(_typeName, TypeAttributes.Public | TypeAttributes.Class);
    }

    public static TypeBuilder Create(string? typeName)
        => new TypeBuilder(typeName);

    public TypeBuilder WithClassAttribute<TAttr>(params object[]? args)
        where TAttr : Attribute
    {
        ApplyAttribute(_tb.SetCustomAttribute, typeof(TAttr), args);
        return this;
    }

    public TypeBuilder WithVoidMethod(string methodName)
    {
        var mb = _tb.DefineMethod(methodName,
            MethodAttributes.Public,
            typeof(void),
            Type.EmptyTypes);

        var il = mb.GetILGenerator();
        il.Emit(OpCodes.Ret);

        return this;
    }

    public TypeBuilder WithVoidMethod<TAttr>(
        string methodName,
        params object[]? args)
        where TAttr : Attribute
    {
        var mb = _tb.DefineMethod(methodName,
            MethodAttributes.Public,
            typeof(void),
            Type.EmptyTypes);

        var il = mb.GetILGenerator();
        il.Emit(OpCodes.Ret);

        ApplyAttribute(mb.SetCustomAttribute, typeof(TAttr), args);

        return this;
    }

    public Type Build() => _tb.CreateType()!;

    private static void ApplyAttribute(
        Action<CustomAttributeBuilder> setter,
        Type attrType,
        object[]? args)
    {
        var ctorArgs = args ?? Array.Empty<object>();
        var ctor = attrType.GetConstructors()
            .First(c => c.GetParameters().Length == ctorArgs.Length);
        setter(new CustomAttributeBuilder(ctor, ctorArgs));
    }
}
