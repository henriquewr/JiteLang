using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming
{
    internal class SimpleNameExpressionSyntax : NameExpressionSyntax
    {
        public override NameKind Kind => NameKind.SimpleName;

        public SimpleNameExpressionSyntax(string text, SyntaxPosition position) : base()
        {
            Text = text;
            Position = position;
        }

        public string Text { get; set; }
    }
}
