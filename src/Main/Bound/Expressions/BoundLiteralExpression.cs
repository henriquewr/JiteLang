using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.Shared;
using System;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundLiteralExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.LiteralExpression;
        public BoundLiteralExpression(ConstantValue value)
        {
            Value = value;
            Position = value.Position;
        }
        public ConstantValue Value { set; get; }
    }
}
