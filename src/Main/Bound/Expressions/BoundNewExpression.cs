using JiteLang.Main.Shared.Type;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundNewExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.NewExpression;
        public override TypeSymbol Type { get; set; }
        public BoundNewExpression(BoundNode? parent, TypeSymbol type, NotifyAddList<BoundExpression> args) : base(parent)
        {
            Type = type;
            Args = args;
        }

        protected void OnAdd(BoundExpression item)
        {
            item.Parent = this;
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