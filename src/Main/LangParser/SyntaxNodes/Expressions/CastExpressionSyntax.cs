using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class CastExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CastExpression;

        public CastExpressionSyntax(SyntaxNode parent, ExpressionSyntax value, TypeSyntax toType) : base(parent)
        {
            Value = value;
            ToType = toType;
        }

        public ExpressionSyntax Value { get; set; }
        public TypeSyntax ToType { get; set; }
    }
}
