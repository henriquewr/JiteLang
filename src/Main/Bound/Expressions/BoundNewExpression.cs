using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundNewExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.NewExpression;
        public BoundNewExpression(BoundNode parent, TypeSymbol type, List<BoundExpression> args) : base(parent)
        {
            Type = type;
            Args = args;
        }

        public List<BoundExpression> Args { get; set; }
        public TypeSymbol Type { get; set; }
    }
}