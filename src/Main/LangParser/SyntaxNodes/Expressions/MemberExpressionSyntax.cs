using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class MemberExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MemberExpression;

        public MemberExpressionSyntax(SyntaxNode parent, ExpressionSyntax left, IdentifierExpressionSyntax right) : base(parent)
        {
            Left = left;
            Right = right;
        }

        public ExpressionSyntax Left { get; set; }
        public IdentifierExpressionSyntax Right { get; set; }
    }
}
