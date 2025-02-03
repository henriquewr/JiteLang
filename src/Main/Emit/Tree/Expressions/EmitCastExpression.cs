using JiteLang.Main.Shared.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitCastExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.CastExpression;
        public override TypeSymbol Type { get; }
        public EmitCastExpression(EmitNode parent, TypeSymbol type) : base(parent)
        {
            Type = type;
        }
    }
}