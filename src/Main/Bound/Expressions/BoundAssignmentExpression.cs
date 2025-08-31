
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.AssignmentExpression;

        public override TypeSymbol Type
        {
            get
            {
                return Left.Type;
            }
            set
            {
                Left.Type = value;
            }
        }

        public BoundAssignmentExpression(BoundNode? parent, BoundExpression left, BoundKind @operator, BoundExpression right) : base(parent)
        {
            Right = right;
            Operator = @operator;
            Left = left;
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

        public BoundKind Operator { get; set; }

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