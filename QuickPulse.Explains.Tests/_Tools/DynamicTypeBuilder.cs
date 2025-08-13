using System.Reflection;
using System.Reflection.Emit;

namespace QuickPulse.Explains.Tests._Tools;

public class DynamicTypeBuilder
{
    private readonly TypeBuilder typeBuilder;

    private DynamicTypeBuilder(TypeBuilder typeBuilder)
    {
        this.typeBuilder = typeBuilder;
    }

    public static DynamicTypeBuilder Create(string typeName, ModuleBuilder? module = null)
    {
        if (module == null)
        {
            var asmName = new AssemblyName("Dynamic_" + Guid.NewGuid().ToString("N"));
            var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            module = asm.DefineDynamicModule("MainModule");
        }

        var tb = module.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
        return new DynamicTypeBuilder(tb);
    }

    public DynamicTypeBuilder WithClassAttribute<TAttr>(params object[]? args)
        where TAttr : Attribute
    {
        ApplyAttribute(typeBuilder.SetCustomAttribute, typeof(TAttr), args);
        return this;
    }

    public DynamicTypeBuilder WithVoidMethod(string methodName)
    {
        var mb = typeBuilder.DefineMethod(methodName,
            MethodAttributes.Public,
            typeof(void),
            Type.EmptyTypes);
        mb.GetILGenerator().Emit(OpCodes.Ret);
        return this;
    }

    public DynamicTypeBuilder WithVoidMethod<TAttr>(
        string methodName,
        params object[]? args)
        where TAttr : Attribute
    {
        var mb = typeBuilder.DefineMethod(methodName,
            MethodAttributes.Public,
            typeof(void),
            Type.EmptyTypes);
        mb.GetILGenerator().Emit(OpCodes.Ret);

        ApplyAttribute(mb.SetCustomAttribute, typeof(TAttr), args);
        return this;
    }

    public Type Build() => typeBuilder.CreateType()!;

    private static void ApplyAttribute(
        Action<CustomAttributeBuilder> setter,
        Type attrType,
        object[]? args)
    {
        var ctorArgs = args ?? [];
        var ctor = attrType.GetConstructors()
            .First(c => c.GetParameters().Length == ctorArgs.Length);
        setter(new CustomAttributeBuilder(ctor, ctorArgs));
    }
}
