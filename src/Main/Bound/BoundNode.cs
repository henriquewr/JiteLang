using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.Bound
{
    internal abstract class BoundNode : Parented<BoundNode>
    {
        public BoundNode(BoundNode parent) : base(parent)
        {
        }

        public abstract BoundKind Kind { get; }
        public SyntaxPosition Position { get; set; }
    }
}