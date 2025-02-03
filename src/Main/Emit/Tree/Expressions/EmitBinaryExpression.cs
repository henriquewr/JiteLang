using JiteLang.Main.Bound.TypeResolvers;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitBinaryExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.BinaryExpression;
        public override TypeSymbol Type => BinaryExprTypeResolver.Resolve(Left.Type, Operation, Right.Type);
        public EmitBinaryExpression(EmitNode parent, EmitExpression left, BinaryOperatorKind operation, EmitExpression right) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public BinaryOperatorKind Operation { get; set; }
        public EmitExpression Right { get; set; }
    }
}