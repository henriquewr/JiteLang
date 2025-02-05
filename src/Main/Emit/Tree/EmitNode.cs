using JiteLang.Main.Shared;

namespace JiteLang.Main.Emit.Tree
{
    internal abstract class EmitNode : Parented<EmitNode>
    {
        public EmitNode(EmitNode parent) : base(parent) 
        {
        }

        public abstract EmitKind Kind { get; }
    }
}