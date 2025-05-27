using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax() : base()
        {
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override void SetParent()
        {
        }

        public override void SetParentRecursive()
        {
        }
    }
}