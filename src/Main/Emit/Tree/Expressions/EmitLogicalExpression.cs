using JiteLang.Main.Bound.TypeResolvers;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitLogicalExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.LogicalExpression;
        public override TypeSymbol Type => LogicalExprTypeResolver.Resolve(Left.Type, Operation, Right.Type);

        public EmitLogicalExpression(EmitNode parent, EmitExpression left, LogicalOperatorKind operation, EmitExpression right) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public LogicalOperatorKind Operation { get; set; }
        public EmitExpression Right { get; set; }
    }
}