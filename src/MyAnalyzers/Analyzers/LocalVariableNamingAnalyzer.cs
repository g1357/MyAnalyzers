using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyAnalyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class LocalVariableNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MYAN0005";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            title: "Local variable names must be camelCase",
            messageFormat: "Local variable '{0}' does not follow camelCase naming convention. Suggested name: '{1}'",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Local variable names must be camelCase.",
            helpLinkUri: "https://github.com/g1357/MyAnalyzers/blob/main/README.md#MYAN0005");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclarator);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var declarator = (VariableDeclaratorSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(declarator);
            if (symbol is not ILocalSymbol localSymbol) return;

            var name = localSymbol.Name;
            if (string.IsNullOrEmpty(name)) return;
            if (NamingHelper.IsCamelCase(name)) return;

            var suggested = NamingHelper.ToCamelCase(name);
            var diagnostic = Diagnostic.Create(Rule, declarator.Identifier.GetLocation(), name, suggested);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
