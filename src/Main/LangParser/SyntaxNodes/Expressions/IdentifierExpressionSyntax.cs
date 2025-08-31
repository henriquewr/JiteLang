using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class IdentifierExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IdentifierExpression;

        public IdentifierExpressionSyntax(string text, SyntaxPosition position) : base()
        {
            Text = text;
            Position = position;
        }

        public string Text { get; set; }
    }
}