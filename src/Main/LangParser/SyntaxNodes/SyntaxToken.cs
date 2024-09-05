using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes
{
    internal class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, string text, SyntaxPosition position) 
        {
            Kind = kind;
            Text = text;
            Position = position;
        }
        public SyntaxKind Kind { get; set; }
        public string Text { get; set; }
        public SyntaxPosition Position { get; set; }
    }
}
