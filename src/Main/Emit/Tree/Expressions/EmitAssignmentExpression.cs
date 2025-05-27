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

        public override void SetParent()
        {
            Right.Parent = this;
            Left.Parent = this;
        }

        public override void SetParentRecursive()
        {
            SetParent();

            Right.SetParentRecursive();
            Left.SetParentRecursive();
        }

        public EmitExpression Left { get; set; }
        public EmitExpression Right { get; set; }
    }
}