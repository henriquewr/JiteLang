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

        public EmitExpression Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public LogicalOperatorKind Operation { get; set; }
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