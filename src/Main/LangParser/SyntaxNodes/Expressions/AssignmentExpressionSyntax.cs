using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

        public AssignmentExpressionSyntax(ExpressionSyntax left, SyntaxKind @operator, ExpressionSyntax right) : base()
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public ExpressionSyntax Left { get; set; }
        public SyntaxKind Operator { get; set; }
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