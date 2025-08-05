using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileHeaderAttribute : Attribute
{
    public string Header { get; }

    public DocFileHeaderAttribute(string header)
    {
        Header = header;
    }
}

public abstract class DocFragmentAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DocHeaderAttribute : DocFragmentAttribute
{
    public string Header { get; }
    public int Level { get; }

    public DocHeaderAttribute(string header, int level = 0)
    {
        Header = header;
        Level = level;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocContentAttribute : DocFragmentAttribute
{
    public string Content { get; }

    public DocContentAttribute(string content)
    {
        Content = content;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeAttribute : DocFragmentAttribute
{
    public string Code { get; }
    public string Language { get; }

    public DocCodeAttribute(string content, string language = "csharp")
    {
        Code = content;
        Language = language;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocIncludeAttribute : DocFragmentAttribute
{
    public Type Included { get; }

    public DocIncludeAttribute(Type includedDoc)
    {
        Included = includedDoc;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeExampleAttribute : DocFragmentAttribute
{
    public string Name { get; }
    public string[] Replacements { get; }

    public DocCodeExampleAttribute(string name, params string[] replacements)
    {
        Name = name;
        Replacements = replacements;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DocExampleAttribute : Attribute
{
    public string File { get; }
    public int Line { get; }

    public DocExampleAttribute(
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        File = file;
        Line = line;
    }
}
