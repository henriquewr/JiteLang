using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitCallExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.CallExpression;
        public override TypeSymbol Type
        {
            get
            {
                return Caller.Type;
            }
            set
            {
                Caller.Type = value;
            }
        }

        public EmitCallExpression(EmitNode? parent, EmitExpression caller, List<EmitExpression> args) : base(parent)
        {
            Caller = caller;
            Args = args;
        }

        public EmitExpression Caller { get; set; }
        public List<EmitExpression> Args { get; set; }

        public override void SetParent()
        {
            Caller.Parent = this;

            foreach (var arg in Args)
            {
                arg.Parent = this;
            }
        }
        public override void SetParentRecursive()
        {
            Caller.Parent = this;
            Caller.SetParentRecursive();

            foreach (var arg in Args)
            {
                arg.Parent = this;
                arg.SetParentRecursive();
            }
        }
    }
}