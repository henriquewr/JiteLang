using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundCallExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.CallExpression;
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
             
        public BoundCallExpression(BoundNode? parent, BoundExpression caller, List<BoundExpression> args) : base(parent)
        {
            Caller = caller;
            Args = args;
        }

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

        public BoundExpression Caller { get; set; }
        public List<BoundExpression> Args { get; set; }
    }
}
