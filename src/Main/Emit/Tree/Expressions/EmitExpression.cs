using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal abstract class EmitExpression : EmitNode
    {
        protected EmitExpression(EmitNode? parent) : base(parent)
        {
        }

        public abstract TypeSymbol Type { get; set; }
    }
}