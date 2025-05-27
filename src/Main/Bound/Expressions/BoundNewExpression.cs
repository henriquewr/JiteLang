using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundNewExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.NewExpression;
        public override TypeSymbol Type { get; set; }
        public BoundNewExpression(BoundNode? parent, TypeSymbol type, List<BoundExpression> args) : base(parent)
        {
            Type = type;
            Args = args;
        }

        public override void SetParent()
        {
            foreach (var arg in Args)
            {
                arg.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            foreach (var arg in Args)
            {
                arg.Parent = this;
                arg.SetParentRecursive();
            }
        }

        public List<BoundExpression> Args { get; set; }
    }
}