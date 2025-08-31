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

        public ExpressionSyntax Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public IdentifierExpressionSyntax Right
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}