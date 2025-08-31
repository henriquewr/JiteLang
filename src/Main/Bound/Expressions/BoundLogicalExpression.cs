using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundLogicalExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.LogicalExpression;
        public override TypeSymbol Type { get; set; }

        public BoundLogicalExpression(BoundNode? parent, BoundExpression left, LogicalOperatorKind operation, BoundExpression right, TypeSymbol type) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
            Type = type;
        }

        public BoundExpression Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public LogicalOperatorKind Operation { get; set; }
        public BoundExpression Right
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}
