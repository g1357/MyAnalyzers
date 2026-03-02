using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class InterfaceNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Interface names must be PascalCase and start with 'I'",
            messageFormat: "Interface '{0}' does not follow naming convention. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Interface names must be PascalCase and start with 'I' (e.g., IUserService).",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0001");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;
            if (symbol.TypeKind != TypeKind.Interface) return;

            var name = symbol.Name;
            if (NamingHelper.IsInterfaceCase(name)) return;

            var suggested = NamingHelper.ToInterfaceCase(name);
            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], name, suggested);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
