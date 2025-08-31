using JiteLang.Main.Shared.Type;
using JiteLang.Utilities;
using System;
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
                return ((DelegateTypeSymbol)Caller.Type).ReturnType; 
            } 
            set 
            {
                throw new InvalidOperationException();
            } 
        }
             
        public BoundCallExpression(BoundNode? parent, BoundExpression caller, NotifyAddList<BoundExpression> args) : base(parent)
        {
            Caller = caller;
            Args = args;
        }

        protected void OnAdd(BoundExpression item)
        {
            item.Parent = this;
        }

        public BoundExpression Caller
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public NotifyAddList<BoundExpression> Args 
        { 
            get; 
            set
            {
                field?.OnAdd -= OnAdd;
                field = value;

                if (field is not null)
                {
                    field.OnAdd += OnAdd;

                    foreach (var member in Args)
                    {
                        OnAdd(member);
                    }
                }
            }
        }
    }
}
