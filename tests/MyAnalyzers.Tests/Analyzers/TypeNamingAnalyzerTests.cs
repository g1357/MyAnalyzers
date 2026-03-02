using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.TypeNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class TypeNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenClassIsPascalCase()
        {
            var code = "public class MyClass { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenClassIsNotPascalCase()
        {
            var code = @"public class {|MYAN0002:myClass|} { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenStructIsPascalCase()
        {
            var code = "public struct MyStruct { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenStructIsNotPascalCase()
        {
            var code = @"public struct {|MYAN0002:myStruct|} { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenEnumIsPascalCase()
        {
            var code = "public enum MyEnum { Value }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenEnumIsNotPascalCase()
        {
            var code = @"public enum {|MYAN0002:myEnum|} { Value }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
