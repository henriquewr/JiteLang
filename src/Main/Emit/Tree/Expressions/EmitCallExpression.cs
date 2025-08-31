using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;
using JiteLang.Utilities;
using System;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitCallExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.CallExpression;
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

        public EmitCallExpression(EmitNode? parent, EmitExpression caller, NotifyAddList<EmitExpression> args) : base(parent)
        {
            Caller = caller;
            Args = args;
        }

        public EmitExpression Caller
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        protected void OnAdd(EmitExpression item)
        {
            item.Parent = this;
        }
        public NotifyAddList<EmitExpression> Args
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