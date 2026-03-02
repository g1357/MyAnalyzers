using System.Threading.Tasks;
using Xunit;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyAnalyzers.Analyzers.AsyncMethodNamingAnalyzer>;

namespace MyAnalyzers.Tests.Analyzers
{
    public class AsyncMethodNamingAnalyzerTests
    {
        [Fact]
        public async Task NoDiagnostic_WhenAsyncMethodEndsWithAsync()
        {
            var code = @"
using System.Threading.Tasks;
public class C { public async Task DoWorkAsync() => await Task.CompletedTask; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenAsyncMethodDoesNotEndWithAsync()
        {
            var code = @"
using System.Threading.Tasks;
public class C { public async Task {|MYAN0007:DoWork|}() => await Task.CompletedTask; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenTaskReturningMethodAlreadyEndsWithAsync()
        {
            var code = @"
using System.Threading.Tasks;
public class C { public Task<int> GetValueAsync() => Task.FromResult(1); }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task Diagnostic_WhenTaskReturningMethodDoesNotEndWithAsync()
        {
            var code = @"
using System.Threading.Tasks;
public class C { public Task<int> {|MYAN0007:GetValue|}() => Task.FromResult(1); }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenSyncMethod()
        {
            var code = "public class C { public void DoWork() { } }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }

        [Fact]
        public async Task NoDiagnostic_WhenEventHandler()
        {
            var code = @"
using System;
using System.Threading.Tasks;
public class C { public Task OnButtonClick(object sender, EventArgs e) => Task.CompletedTask; }";
            await VerifyCS.VerifyAnalyzerAsync(code);
        }
    }
}
