using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class MemberExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MemberExpression;

        public MemberExpressionSyntax(ExpressionSyntax left, IdentifierExpressionSyntax right) : base()
        {
            Left = left;
            Right = right;
        }

        public ExpressionSyntax Left { get; set; }
        public IdentifierExpressionSyntax Right { get; set; }

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