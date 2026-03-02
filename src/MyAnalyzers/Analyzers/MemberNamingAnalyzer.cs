using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MemberNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0003";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Member names must be PascalCase",
            messageFormat: "Member '{0}' does not follow PascalCase naming convention. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Method, property, and event names must be PascalCase.",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0003");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;

            // Skip overrides, explicit interface implementations, operators
            if (symbol is IMethodSymbol method)
            {
                if (method.MethodKind == MethodKind.Constructor ||
                    method.MethodKind == MethodKind.Destructor ||
                    method.MethodKind == MethodKind.UserDefinedOperator ||
                    method.MethodKind == MethodKind.Conversion ||
                    method.MethodKind == MethodKind.PropertyGet ||
                    method.MethodKind == MethodKind.PropertySet ||
                    method.MethodKind == MethodKind.EventAdd ||
                    method.MethodKind == MethodKind.EventRemove ||
                    method.ExplicitInterfaceImplementations.Length > 0 ||
                    method.IsOverride)
                    return;
            }

            if (symbol is IPropertySymbol property)
            {
                if (property.ExplicitInterfaceImplementations.Length > 0 || property.IsOverride)
                    return;
            }

            if (symbol is IEventSymbol @event)
            {
                if (@event.ExplicitInterfaceImplementations.Length > 0 || @event.IsOverride)
                    return;
            }

            var name = symbol.Name;
            if (NamingHelper.IsPascalCase(name)) return;

            var suggested = NamingHelper.ToPascalCase(name);
            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], name, suggested);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
