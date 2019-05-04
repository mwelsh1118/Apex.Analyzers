﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Apex.Analyzers.Immutable.Rules
{
    internal static class IMM003
    {
        public const string DiagnosticId = "IMM003";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.IMM003Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.IMM003MessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.IMM003Description), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Architecture";

        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
        internal static void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (IFieldSymbol)context.Symbol;
            var containingType = symbol.ContainingType;
            if(containingType == null)
            {
                return;
            }

            if(Helper.HasImmutableAttribute(containingType)
                && Helper.ShouldCheckMemberTypeForImmutability(symbol)
                && !Helper.IsImmutableType(symbol.Type, context.Compilation))
            {
                var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
