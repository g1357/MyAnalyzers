using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.LocalVariableNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class LocalVariableNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenLocalIsCamelCase()
        {
            var code = "public class C { public void M() { int myVar = 0; } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenLocalIsNotCamelCase()
        {
            var code = @"public class C { public void M() { int {|MYAN0005:MyVar|} = 0; } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
