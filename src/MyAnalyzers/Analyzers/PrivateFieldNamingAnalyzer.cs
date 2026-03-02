using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PrivateFieldNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0006";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Private field names must be _camelCase",
            messageFormat: "Private field '{0}' does not follow _camelCase naming convention. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Private field names must start with an underscore followed by camelCase (e.g., _myField).",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0006");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (IFieldSymbol)context.Symbol;

            // Only check private fields (not constants, not enum members)
            if (symbol.DeclaredAccessibility != Accessibility.Private) return;
            if (symbol.IsConst) return;
            if (symbol.ContainingType?.TypeKind == TypeKind.Enum) return;

            var name = symbol.Name;
            if (NamingHelper.IsPrivateFieldCase(name)) return;

            var suggested = NamingHelper.ToPrivateFieldCase(name);
            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], name, suggested);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
