using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ParameterNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0004";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Parameter names must be camelCase",
            messageFormat: "Parameter '{0}' does not follow camelCase naming convention. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Parameter names must be camelCase.",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0004");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Parameter);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (IParameterSymbol)context.Symbol;

            // Skip parameters that are part of overrides or explicit interface implementations
            if (symbol.ContainingSymbol is IMethodSymbol method)
            {
                if (method.IsOverride || method.ExplicitInterfaceImplementations.Length > 0)
                    return;
            }

            var name = symbol.Name;
            if (string.IsNullOrEmpty(name)) return;
            if (NamingHelper.IsCamelCase(name)) return;

            var suggested = NamingHelper.ToCamelCase(name);
            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], name, suggested);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
