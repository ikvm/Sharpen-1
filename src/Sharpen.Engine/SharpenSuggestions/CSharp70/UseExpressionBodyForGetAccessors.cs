﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpen.Engine.CSharpFeatures;

namespace Sharpen.Engine.SharpenSuggestions.CSharp70
{
    public class UseExpressionBodyForGetAccessors : ISharpenSuggestion, ISingleSyntaxTreeAnalyzer
    {
        private UseExpressionBodyForGetAccessors() { }

        public string MinimumLanguageVersion { get; } = CSharpLanguageVersions.CSharp70;

        public ICSharpFeature LanguageFeature { get; } = ExpressionBodiedMembers.Instance;

        public string FriendlyName { get; } = "Use expression body for get accessors";

        public static readonly UseExpressionBodyForGetAccessors Instance = new UseExpressionBodyForGetAccessors();

        public IEnumerable<AnalysisResult> Analyze(SyntaxTree syntaxTree)
        {
            return syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<AccessorDeclarationSyntax>()
                .Where(accessor => accessor.Keyword.IsKind(SyntaxKind.GetKeyword) && accessor.Body != null && accessor.Body.Statements.Count == 1 && accessor.Body.Statements[0] is ReturnStatementSyntax)
                .Select(accessor => new AnalysisResult(this, syntaxTree.FilePath, accessor.Keyword));
        }
    }
}