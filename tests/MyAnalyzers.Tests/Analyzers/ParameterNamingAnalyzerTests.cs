using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.ParameterNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class ParameterNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenParameterIsCamelCase()
        {
            var code = "public class C { public void M(int myParam) { } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenParameterIsNotCamelCase()
        {
            var code = @"public class C { public void M(int {|MYAN0004:MyParam|}) { } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
