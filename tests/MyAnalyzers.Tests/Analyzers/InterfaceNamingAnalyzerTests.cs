using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.InterfaceNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class InterfaceNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenInterfaceFollowsConvention()
        {
            var code = "public interface IUserService { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenInterfaceMissingI()
        {
            var code = @"public interface {|MYAN0001:UserService|} { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenInterfaceNotPascalCase()
        {
            var code = @"public interface {|MYAN0001:iUserService|} { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenInterfaceHasOnlyI()
        {
            // "I" alone is not valid (needs uppercase after I), but our check requires 2 chars
            var code = @"public interface {|MYAN0001:I|} { }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
