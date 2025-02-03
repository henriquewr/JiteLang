using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitCallExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.CallExpression;

        public EmitCallExpression(EmitNode parent, EmitExpression caller, List<EmitExpression> args) : base(parent)
        {
            Caller = caller;
            Args = args;
        }

        public override TypeSymbol Type { get; }

        public EmitCallExpression(EmitNode parent, EmitExpression caller) : this(parent, caller, new())
        {
        }

        public EmitExpression Caller { get; set; }
        public List<EmitExpression> Args { get; set; }
    }
}