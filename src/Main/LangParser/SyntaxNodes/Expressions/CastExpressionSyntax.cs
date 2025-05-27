using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class CastExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CastExpression;

        public CastExpressionSyntax(ExpressionSyntax value, TypeSyntax toType) : base()
        {
            Value = value;
            ToType = toType;
        }

        public ExpressionSyntax Value { get; set; }
        public TypeSyntax ToType { get; set; }

        public override void SetParent()
        {
            Value.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Value.Parent = this;
            Value.SetParentRecursive();
        }
    }
}