using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundLiteralExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.LiteralExpression;
        public override TypeSymbol Type { get; set; }

        public BoundLiteralExpression(BoundNode? parent, ConstantValue value) : base(parent)
        {
            Value = value;
            Type = value.Type;
        }
        public ConstantValue Value { set; get; }

        public override void SetParent()
        {
        }

        public override void SetParentRecursive()
        {
        }
    }
}