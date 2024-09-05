using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes
{
    internal class SyntaxTokenWithValue<T> : SyntaxToken
    {
        public SyntaxTokenWithValue(SyntaxKind kind, string text, T value, SyntaxPosition position) : base(kind, text, position)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
