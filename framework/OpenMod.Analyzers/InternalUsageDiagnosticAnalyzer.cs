using System;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace OpenMod.Analyzers
{
    [UsedImplicitly]
    [DiagnosticAnalyzer("CSharp")]
    public class InternalUsageDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string Id = "OM1001";

        public const string MessageFormat  = "{0} is an internal API that supports the OpenMod infrastructure and is not meant to be used by plugins.";


        protected const string DefaultTitle = "Internal OpenMod API usage.";
        protected const string Category = "Usage";

        private static readonly int s_OMLen = "OpenMod".Length;

        private static readonly DiagnosticDescriptor m_Descriptor
            = new DiagnosticDescriptor(
                Id,
                title: DefaultTitle,
                messageFormat: MessageFormat,
                category: Category,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(m_Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(
                AnalyzeNode,
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.ObjectCreationExpression,
                SyntaxKind.ClassDeclaration);
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case MemberAccessExpressionSyntax memberAccessSyntax:
                    {
                        if (context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken).Symbol is ISymbol symbol
                            && !Equals(symbol.ContainingAssembly, context.Compilation.Assembly))
                        {
                            var containingType = symbol.ContainingType;

                            if (HasInternalAttribute(symbol))
                            {
                                context.ReportDiagnostic(
                                    Diagnostic.Create(m_Descriptor, memberAccessSyntax.Name.GetLocation(), $"{containingType}.{symbol.Name}"));
                                return;
                            }

                            if (IsInInternalNamespace(containingType)
                                || HasInternalAttribute(containingType))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(m_Descriptor, memberAccessSyntax.Name.GetLocation(), containingType));
                                return;
                            }
                        }

                        return;
                    }

                case ObjectCreationExpressionSyntax creationSyntax:
                    {
                        if (context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken).Symbol is ISymbol symbol
                            && !Equals(symbol.ContainingAssembly, context.Compilation.Assembly))
                        {
                            var containingType = symbol.ContainingType;

                            if (HasInternalAttribute(symbol))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(m_Descriptor, creationSyntax.GetLocation(), containingType));
                                return;
                            }

                            if (IsInInternalNamespace(containingType)
                                || HasInternalAttribute(containingType))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(m_Descriptor, creationSyntax.Type.GetLocation(), containingType));
                                return;
                            }
                        }

                        return;
                    }

                case ClassDeclarationSyntax declarationSyntax:
                    {
                        var classSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, declarationSyntax) as INamedTypeSymbol;
                        
                        if (classSymbol?.BaseType is ISymbol symbol
                            && !Equals(symbol.ContainingAssembly, context.Compilation.Assembly)
                            && (IsInInternalNamespace(symbol) || HasInternalAttribute(symbol))
                            && declarationSyntax.BaseList?.Types.Count > 0)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(m_Descriptor, declarationSyntax.BaseList.Types[0].GetLocation(), symbol));
                        }

                        return;
                    }
            }
        }

        private static bool HasInternalAttribute(ISymbol symbol)
        {
            return symbol != null &&
                   symbol.GetAttributes().Any(a => a.AttributeClass.Name == "OpenModInternalAttribute");
        }

        private static bool IsInInternalNamespace(ISymbol symbol)
        {
            if (!(symbol?.ContainingNamespace?.ToDisplayString() is string ns))
            {
                return false;
            }

            var i = ns.IndexOf("OpenMod", StringComparison.Ordinal);

            return
                i != -1 &&
                (i == 0 || ns[i - 1] == '.') &&
                i + s_OMLen < ns.Length && ns[i + s_OMLen] == '.';
        }
    }
}
