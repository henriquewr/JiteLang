using JiteLang.Syntax;

namespace JiteLang.Main.Bound
{
    internal abstract class BoundNode
    {
        public BoundNode(BoundNode parent)
        {
            Parent = parent;
        }

        public BoundNode? Parent { get; set; }
        public abstract BoundKind Kind { get; }
        public SyntaxPosition Position { get; set; }
    }
}
