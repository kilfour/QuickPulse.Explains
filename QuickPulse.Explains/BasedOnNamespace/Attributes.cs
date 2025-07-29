namespace QuickPulse.Explains.BasedOnNamespace;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileAttribute : Attribute { }

public abstract class DocMethodAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DocHeaderAttribute : DocMethodAttribute
{
    public string Header { get; }

    public DocHeaderAttribute(string header)
    {
        Header = header;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DocContentAttribute : DocMethodAttribute
{
    public string Content { get; }

    public DocContentAttribute(string content)
    {
        Content = content;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DocIncludeAttribute : DocMethodAttribute
{
    public Type IncludedDoc { get; }

    public DocIncludeAttribute(Type includedDoc)
    {
        IncludedDoc = includedDoc;
    }
}


