using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes
{
    internal abstract class SyntaxNode : Parented<SyntaxNode>
    {
        public SyntaxNode(SyntaxNode parent) : base(parent)
        {
        }

        public abstract SyntaxKind Kind { get; }
        public SyntaxPosition Position { get; set; }
    }
}