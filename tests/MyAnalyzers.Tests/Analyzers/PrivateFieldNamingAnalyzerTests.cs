using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.PrivateFieldNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class PrivateFieldNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenPrivateFieldFollowsConvention()
        {
            var code = "public class C { private int _myField; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenPrivateFieldMissingUnderscore()
        {
            var code = @"public class C { private int {|MYAN0006:myField|}; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenPrivateFieldNotCamelCase()
        {
            var code = @"public class C { private int {|MYAN0006:MyField|}; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenPublicField()
        {
            var code = "public class C { public int MyField; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenConstant()
        {
            var code = "public class C { private const int MyConst = 1; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
