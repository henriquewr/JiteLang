using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal abstract class BoundExpression : BoundNode
    {
        protected BoundExpression(BoundNode? parent) : base(parent)
        {
        }

        public abstract TypeSymbol Type { get; set; }
    }
}