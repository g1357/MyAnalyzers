using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AsyncMethodNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0007";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Async methods must end with 'Async'",
            messageFormat: "Async method '{0}' does not end with 'Async'. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Async methods returning Task, Task<T>, ValueTask, or ValueTask<T> must end with 'Async'. Event handlers are excluded.",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0007");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;

            if (method.MethodKind != MethodKind.Ordinary &&
                method.MethodKind != MethodKind.ExplicitInterfaceImplementation)
                return;

            if (!ReturnsAwaitableType(method)) return;

            // Exclude event handlers: void-returning methods with (object sender, EventArgs e) pattern
            if (IsEventHandler(method)) return;

            var name = method.Name;
            if (name.EndsWith("Async", System.StringComparison.Ordinal)) return;

            var suggested = name + "Async";
            var diagnostic = Diagnostic.Create(Rule, method.Locations[0], name, suggested);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool ReturnsAwaitableType(IMethodSymbol method)
        {
            var returnType = method.ReturnType;
            if (returnType == null) return false;

            var fullName = returnType.OriginalDefinition?.ToDisplayString() ?? returnType.ToDisplayString();
            return fullName == "System.Threading.Tasks.Task" ||
                   fullName == "System.Threading.Tasks.Task<TResult>" ||
                   fullName == "System.Threading.Tasks.ValueTask" ||
                   fullName == "System.Threading.Tasks.ValueTask<TResult>";
        }

        private static bool IsEventHandler(IMethodSymbol method)
        {
            // Event handler pattern: (object sender, System.EventArgs e) or derived EventArgs
            if (method.Parameters.Length != 2) return false;
            var first = method.Parameters[0].Type;
            var second = method.Parameters[1].Type;

            if (first.SpecialType != SpecialType.System_Object) return false;

            // Check if second param is EventArgs or derives from it
            var t = second;
            while (t != null)
            {
                if (t.ToDisplayString() == "System.EventArgs") return true;
                t = t.BaseType;
            }
            return false;
        }
    }
}
