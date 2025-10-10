# Spike
```csharp
private class Bar
{
    public int MyMethod() { return 42; }
}
```
```markdown
Just Checking
```
```csharp
// just a comment { a { b } }
```
```csharp
private void FullFoo()
{
    // replaced { }
}
```
```csharp
private class Bar
{
    public int MyMethod() { return 42; }
}
```
```sql
one\"withquote
tw,o
three
```
```csharp
private List<string> AnotherList()
{
    [
        "char='{', enter=-1, emit=False, exit=0",
        "char=' ', enter=0, emit=True, exit=0",
        "char='a', enter=0, emit=True, exit=0",
        "char=' ', enter=0, emit=True, exit=0",
        "char='{', enter=0, emit=True, exit=1",
        "char=' ', enter=1, emit=True, exit=1",
        "char='b', enter=1, emit=True, exit=1",
        "char=' ', enter=1, emit=True, exit=1",
        "char='}', enter=1, emit=True, exit=0",
        "char=' ', enter=0, emit=True, exit=0",
        "char='c', enter=0, emit=True, exit=0",
        "char=' ', enter=0, emit=True, exit=0",
        "char='}', enter=0, emit=True, exit=-1"
    ];
}
```
