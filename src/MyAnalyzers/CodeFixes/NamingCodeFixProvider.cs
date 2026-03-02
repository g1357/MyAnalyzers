using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;
using MyAnalyzers.Analyzers;

namespace MyAnalyzers.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamingCodeFixProvider)), Shared]
    public sealed class NamingCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(
                InterfaceNamingAnalyzer.DiagnosticId,
                TypeNamingAnalyzer.DiagnosticId,
                MemberNamingAnalyzer.DiagnosticId,
                ParameterNamingAnalyzer.DiagnosticId,
                LocalVariableNamingAnalyzer.DiagnosticId,
                PrivateFieldNamingAnalyzer.DiagnosticId,
                AsyncMethodNamingAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics[0];
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null) return;

            var token = root.FindToken(diagnosticSpan.Start);
            if (token == default) return;

            // Extract suggested name from diagnostic message
            var message = diagnostic.GetMessage();
            var suggestedName = ExtractSuggestedName(message);
            if (string.IsNullOrEmpty(suggestedName)) return;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Rename to '{suggestedName}'",
                    createChangedSolution: ct => RenameSymbolAsync(context.Document, token, suggestedName!, ct),
                    equivalenceKey: diagnostic.Id + "_" + suggestedName),
                diagnostic);
        }

        private static string? ExtractSuggestedName(string message)
        {
            // Message format: "... Suggested name: 'SuggestedName'"
            const string marker = "Suggested name: '";
            var idx = message.LastIndexOf(marker, System.StringComparison.Ordinal);
            if (idx < 0) return null;
            var start = idx + marker.Length;
            var end = message.IndexOf('\'', start);
            if (end < 0) return null;
            return message.Substring(start, end - start);
        }

        private static async Task<Solution> RenameSymbolAsync(
            Document document,
            SyntaxToken token,
            string newName,
            CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null) return document.Project.Solution;

            var symbol = semanticModel.GetDeclaredSymbol(token.Parent!, cancellationToken)
                      ?? semanticModel.GetSymbolInfo(token.Parent!, cancellationToken).Symbol;

            if (symbol == null) return document.Project.Solution;

            var solution = document.Project.Solution;
            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, new SymbolRenameOptions(), newName, cancellationToken).ConfigureAwait(false);
            return newSolution;
        }
    }
}
