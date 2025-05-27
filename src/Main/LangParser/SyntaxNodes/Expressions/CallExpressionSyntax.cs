using System.Collections.Generic;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class CallExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public CallExpressionSyntax(ExpressionSyntax caller, List<ExpressionSyntax> args) : base()
        {
            Caller = caller;
            Args = args;
            Position = caller.Position;
        }

        public ExpressionSyntax Caller { get; set; }
        public List<ExpressionSyntax> Args { get; set; }

        public override void SetParent()
        {
            Caller.Parent = this;

            foreach (var arg in Args)
            {
                arg.Parent = this;
            }
        }
        public override void SetParentRecursive()
        {
            Caller.Parent = this;
            Caller.SetParentRecursive();

            foreach (var arg in Args)
            {
                arg.Parent = this;
                arg.SetParentRecursive();
            }
        }
    }
}