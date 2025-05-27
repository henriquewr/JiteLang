
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundUnaryExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.UnaryExpression;

        public override TypeSymbol Type { get; set; }

        public BoundUnaryExpression(BoundNode? parent, TypeSymbol type) : base(parent)
        {
            Type = type;
        }

        public override void SetParent()
        {
        }

        public override void SetParentRecursive()
        {
        }
    }
}