using System;
using System.Collections.Generic;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class CallExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public CallExpressionSyntax(ExpressionSyntax caller, List<ExpressionSyntax> args)
        {
            Caller = caller;
            Args = args;
        }

        public CallExpressionSyntax(ExpressionSyntax caller)
        {
            Caller = caller;
            Args = new List<ExpressionSyntax>();
        }

        public ExpressionSyntax Caller { get; set; }
        public List<ExpressionSyntax> Args { get; set; }
    }
}
