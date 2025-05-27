using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class BinaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public BinaryExpressionSyntax(ExpressionSyntax left, BinaryOperatorKind operation, ExpressionSyntax right) : base()
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public ExpressionSyntax Left { get; set; }
        public BinaryOperatorKind Operation { get; set; }
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