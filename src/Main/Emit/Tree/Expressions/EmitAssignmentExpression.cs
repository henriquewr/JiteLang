using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitAssignmentExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.AssignmentExpression;
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

        public EmitAssignmentExpression(EmitNode? parent, EmitExpression left, EmitExpression right) : base(parent)
        {
            Left = left;
            Right = right;
        }

        public EmitExpression Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public EmitExpression Right
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