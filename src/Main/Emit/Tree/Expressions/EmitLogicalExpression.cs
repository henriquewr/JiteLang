using JiteLang.Main.Bound.TypeResolvers;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitLogicalExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.LogicalExpression;
        public override TypeSymbol Type { get; set; }

        public EmitLogicalExpression(EmitNode? parent, EmitExpression left, LogicalOperatorKind operation, EmitExpression right, TypeSymbol type) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
            Type = type;
        }

        public EmitExpression Left { get; set; }
        public LogicalOperatorKind Operation { get; set; }
        public EmitExpression Right { get; set; }

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
    }
}