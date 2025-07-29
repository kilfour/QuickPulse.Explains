namespace QuickPulse.Explains.BasedOnNamespace;

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
public class DocIncludeAttribute : DocFragmentAttribute
{
    public Type Included { get; }

    public DocIncludeAttribute(Type includedDoc)
    {
        Included = includedDoc;
    }
}


