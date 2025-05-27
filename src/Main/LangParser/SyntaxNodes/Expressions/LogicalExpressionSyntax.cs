using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class LogicalExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.LogicalExpression;

        public LogicalExpressionSyntax(ExpressionSyntax left, LogicalOperatorKind operation, ExpressionSyntax right) : base()
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public ExpressionSyntax Left { get; set; }
        public LogicalOperatorKind Operation { get; set; }
        public ExpressionSyntax Right { get; set; }

        public override void SetParent()
        {
            Left.Parent = this;
            Right.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Left.Parent = this;
            Right.Parent = this;

            Left.SetParentRecursive();
            Right.SetParentRecursive();
        }
    }
}