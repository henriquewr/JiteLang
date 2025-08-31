using JiteLang.Main.Shared.Type;
using System;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitMemberExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.MemberExpression;
        public override TypeSymbol Type { get; set; }

        public EmitMemberExpression(EmitNode? parent, EmitExpression left, EmitIdentifierExpression right, TypeSymbol type) : base(parent)
        {
            Left = left;
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

        public EmitIdentifierExpression Right
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