﻿using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitUnaryExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.UnaryExpression;

        public override TypeSymbol Type { get; }

        public EmitUnaryExpression(EmitNode parent, TypeSymbol type) : base(parent)
        {
            Type = type;
        }
    }
}