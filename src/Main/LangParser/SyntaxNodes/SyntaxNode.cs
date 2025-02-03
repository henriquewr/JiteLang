using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes
{
    internal abstract class SyntaxNode
    {
        public SyntaxNode(SyntaxNode parent)
        {
            Parent = parent;
        }

        public abstract SyntaxKind Kind { get; }
        public SyntaxPosition Position { get; set; }
        public SyntaxNode Parent { get; set; }
    }
}