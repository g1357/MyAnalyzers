using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class TypeNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0002";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Type names must be PascalCase",
            messageFormat: "Type '{0}' does not follow PascalCase naming convention. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Class, struct, enum, and delegate names must be PascalCase.",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0002");

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
            if (symbol.TypeKind != TypeKind.Class &&
                symbol.TypeKind != TypeKind.Struct &&
                symbol.TypeKind != TypeKind.Enum &&
                symbol.TypeKind != TypeKind.Delegate)
                return;

            var name = symbol.Name;
            if (NamingHelper.IsPascalCase(name)) return;

            var suggested = NamingHelper.ToPascalCase(name);
            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], name, suggested);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
