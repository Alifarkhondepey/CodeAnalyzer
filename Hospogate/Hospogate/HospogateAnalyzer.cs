using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Hospogate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HospogateAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "HospogateAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.NamespaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeInterface, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
            if (!char.IsUpper(namespaceDeclaration.Name.ToString()[0]))
            {
                var diagnostic = Diagnostic.Create(Rule, namespaceDeclaration.Name.GetLocation(), "Namespace should start with an uppercase letter.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            if (!char.IsUpper(classDeclaration.Identifier.ValueText[0]))
            {
                var diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), "Class should start with an uppercase letter.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeInterface(SyntaxNodeAnalysisContext context)
        {
            var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;
            if (!interfaceDeclaration.Identifier.ValueText.StartsWith("I", StringComparison.Ordinal) || !char.IsUpper(interfaceDeclaration.Identifier.ValueText[1]))
            {
                var diagnostic = Diagnostic.Create(Rule, interfaceDeclaration.Identifier.GetLocation(), "Interface should start with 'I' followed by an uppercase letter.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (!char.IsUpper(methodDeclaration.Identifier.ValueText[0]))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), "Method should start with an uppercase letter.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
            if (!char.IsUpper(propertyDeclaration.Identifier.ValueText[0]))
            {
                var diagnostic = Diagnostic.Create(Rule, propertyDeclaration.Identifier.GetLocation(), "Property should start with an uppercase letter.");
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeField(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                if (!char.IsLower(variable.Identifier.ValueText[0]))
                {
                    // Check if the field is a dependency injection field
                    var isDependencyInjectionField = fieldDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword)
                                                     && fieldDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
                                                     && fieldDeclaration.Declaration.Type is SimpleNameSyntax simpleNameSyntax
                                                     && simpleNameSyntax.Identifier.ValueText.StartsWith("I", StringComparison.Ordinal);

                    // If it's a dependency injection field, check if it starts with '_'
                    if (isDependencyInjectionField && !variable.Identifier.ValueText.StartsWith("_", StringComparison.Ordinal))
                    {
                        var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), "Dependency injection field should start with '_' followed by a lowercase letter.");
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
