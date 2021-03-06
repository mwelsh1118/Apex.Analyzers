﻿using Apex.Analyzers.Immutable.Semantics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Apex.Analyzers.Immutable.Rules
{
    internal static class IMM006
    {
        public const string DiagnosticId = "IMM006";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.IMM006Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.IMM006MessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.IMM006Description), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Architecture";

        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
        internal static void Initialize(AnalysisContext context, ImmutableTypes immutableTypes)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(x => AnalyzeSymbol(x, immutableTypes), SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context, ImmutableTypes immutableTypes)
        {
            immutableTypes.Initialize(context.Compilation, context.Options, context.CancellationToken);

            string genericTypeArgument = null;
            var symbol = (INamedTypeSymbol)context.Symbol;
            if (symbol.BaseType != null
                && Helper.HasImmutableAttributeAndShouldVerify(symbol)
                && !immutableTypes.IsImmutableType(symbol.BaseType, ref genericTypeArgument))
            {
                var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
