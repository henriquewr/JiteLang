using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitBinaryExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.BinaryExpression;
        public override TypeSymbol Type { get; set; }
        public EmitBinaryExpression(EmitNode? parent, EmitExpression left, BinaryOperatorKind operation, EmitExpression right, TypeSymbol type) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
            Type = type;
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
        public BinaryOperatorKind Operation { get; set; }
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