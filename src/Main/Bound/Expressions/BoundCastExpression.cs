using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundCastExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.CastExpression;
        public BoundCastExpression(BoundExpression value, TypeSymbol toType)
        {
            Value = value;
            ToType = toType;
        }
        public BoundExpression Value { get; set; }
        public TypeSymbol ToType { get; set; }
    }
}
