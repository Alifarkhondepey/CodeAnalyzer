using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer.CodeAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnalyzerCodeFixProvider))]
    [Shared]
    public class AnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AnalyzerAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var node = root.FindNode(diagnosticSpan);

            switch (node)
            {
                case NamespaceDeclarationSyntax namespaceDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create("Fix namespace naming", c => FixNamespaceAsync(context.Document, namespaceDeclaration, c)),
                        diagnostic);
                    break;
                case ClassDeclarationSyntax classDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create("Fix class naming", c => FixClassAsync(context.Document, classDeclaration, c)),
                        diagnostic);
                    break;
                case InterfaceDeclarationSyntax interfaceDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create("Fix interface naming", c => FixInterfaceAsync(context.Document, interfaceDeclaration, c)),
                        diagnostic);
                    break;
                case MethodDeclarationSyntax methodDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create("Fix method naming", c => FixMethodAsync(context.Document, methodDeclaration, c)),
                        diagnostic);
                    break;
                case PropertyDeclarationSyntax propertyDeclaration:
                    context.RegisterCodeFix(
                        CodeAction.Create("Fix property naming", c => FixPropertyAsync(context.Document, propertyDeclaration, c)),
                        diagnostic);
                    break;
                case VariableDeclaratorSyntax variableDeclarator:
                    context.RegisterCodeFix(
                        CodeAction.Create("Fix field naming", c => FixFieldAsync(context.Document, variableDeclarator, c)),
                        diagnostic);
                    break;
            }
        }

        private async Task<Document> FixNamespaceAsync(Document document, NamespaceDeclarationSyntax namespaceDeclaration, CancellationToken cancellationToken)
        {
            var newName = char.ToUpper(namespaceDeclaration.Name.ToString()[0]) + namespaceDeclaration.Name.ToString().Substring(1);
            var newNamespaceDeclaration = namespaceDeclaration.WithName(SyntaxFactory.ParseName(newName));
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(namespaceDeclaration, newNamespaceDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixClassAsync(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var newName = char.ToUpper(classDeclaration.Identifier.ValueText[0]) + classDeclaration.Identifier.ValueText.Substring(1);
            var newClassDeclaration = classDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName));
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixInterfaceAsync(Document document, InterfaceDeclarationSyntax interfaceDeclaration, CancellationToken cancellationToken)
        {
            var newName = "I" + char.ToUpper(interfaceDeclaration.Identifier.ValueText[0]) + interfaceDeclaration.Identifier.ValueText.Substring(1);
            var newInterfaceDeclaration = interfaceDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName));
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(interfaceDeclaration, newInterfaceDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixMethodAsync(Document document, MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
        {
            var newName = char.ToUpper(methodDeclaration.Identifier.ValueText[0]) + methodDeclaration.Identifier.ValueText.Substring(1);
            var newMethodDeclaration = methodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName));
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(methodDeclaration, newMethodDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixPropertyAsync(Document document, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
        {
            var newName = char.ToUpper(propertyDeclaration.Identifier.ValueText[0]) + propertyDeclaration.Identifier.ValueText.Substring(1);
            var newPropertyDeclaration = propertyDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName));
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(propertyDeclaration, newPropertyDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> FixFieldAsync(Document document, VariableDeclaratorSyntax variableDeclarator, CancellationToken cancellationToken)
        {
            var newName = "_" + variableDeclarator.Identifier.ValueText;
            var newVariableDeclarator = variableDeclarator.WithIdentifier(SyntaxFactory.Identifier(newName));
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(variableDeclarator, newVariableDeclarator);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
