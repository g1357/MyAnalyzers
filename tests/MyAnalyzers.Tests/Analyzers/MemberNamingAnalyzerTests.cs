using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.MemberNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class MemberNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenMethodIsPascalCase()
        {
            var code = "public class C { public void DoWork() { } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenMethodIsNotPascalCase()
        {
            var code = @"public class C { public void {|MYAN0003:doWork|}() { } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenPropertyIsPascalCase()
        {
            var code = "public class C { public int MyProp { get; set; } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenPropertyIsNotPascalCase()
        {
            var code = @"public class C { public int {|MYAN0003:myProp|} { get; set; } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenConstructor()
        {
            var code = "public class MyClass { public MyClass() { } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
