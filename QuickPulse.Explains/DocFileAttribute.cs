using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileAttribute : Attribute
{
    // Optional: Add Title or Order if you need them later
}

public abstract class DocMethodAttribute : Attribute
{
    // Marker base class for method-level doc attributes
}

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
