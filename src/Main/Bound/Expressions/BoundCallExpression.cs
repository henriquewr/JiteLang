using System.Collections.Generic;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundCallExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.CallExpression;
        public BoundCallExpression(BoundExpression caller, List<BoundExpression> args)
        {
            Caller = caller;
            Args = args;
            Position = caller.Position;
        }

        public BoundCallExpression(BoundExpression caller) : this(caller, new())
        {
        }

        public BoundExpression Caller { get; set; }
        public List<BoundExpression> Args { get; set; }

    }
}
