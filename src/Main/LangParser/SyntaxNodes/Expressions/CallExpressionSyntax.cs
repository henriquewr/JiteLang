using System;
using System.Collections.Generic;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class CallExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public CallExpressionSyntax(SyntaxNode parent, ExpressionSyntax caller, List<ExpressionSyntax> args) : base(parent)
        {
            Caller = caller;
            Args = args;
            Position = caller.Position;
        }

        public CallExpressionSyntax(SyntaxNode parent, ExpressionSyntax caller) : base(parent)
        {
            Caller = caller;
            Args = new List<ExpressionSyntax>();
        }

        public ExpressionSyntax Caller { get; set; }
        public List<ExpressionSyntax> Args { get; set; }
    }
}
