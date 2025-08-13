// namespace QuickPulse.Explains.Tests;



// // ---- Minimal resolver used by the doc build flow ----
// static class ExampleResolver
// {
//     public static IEnumerable<string> ResolveCode(
//         CodeExampleAttribute code,
//         IEnumerable<CodeReplaceAttribute> replaces,
//         IExampleLocator fs)
//     {
//         IEnumerable<string> lines = fs.ReadAfter(code.File, code.Line);
//         foreach (var rep in replaces)
//             lines = lines.Select(l => l.Replace(rep.From, rep.To));
//         return lines;
//     }
// }

// // ---- The test ----
// public class CodeReplaceTests
// {
//     [Fact]
//     public void CodeReplace_on_CodeExample_makes_snippets_match_DocExample()
//     {
//         var fs = new FakeExampleLocator()
//             .Add("code.cs", 0,
//                 "public static class Demo {",
//                 "    public static void Run() {",
//                 "        Console.WriteLine(\"Hi\");",   // <- will be replaced
//                 "    }",
//                 "}");

//         var codeAttr = new CodeExampleAttribute("code.cs", 0);
//         var replace = new[] { new CodeReplaceAttribute("Hi", "Hello") };

//         // Act
//         var docLines = ExampleResolver.ResolveDoc(docAttr, fs).ToArray();
//         var codeLines = ExampleResolver.ResolveCode(codeAttr, replace, fs).ToArray();

//         // Assert: with replacement, the resolved snippets are identical
//         Assert.Equal(docLines, codeLines);

//         // (Optional) Guard: without replacement they would differ
//         var codeNoReplace = ExampleResolver.ResolveCode(codeAttr, Array.Empty<CodeReplaceAttribute>(), fs).ToArray();
//         Assert.NotEqual(docLines, codeNoReplace);
//     }
// }
