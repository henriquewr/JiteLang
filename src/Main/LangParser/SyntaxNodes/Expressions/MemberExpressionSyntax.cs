using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class MemberExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MemberExpression;
    }
}
