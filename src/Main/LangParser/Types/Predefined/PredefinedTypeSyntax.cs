using JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming;

namespace JiteLang.Main.LangParser.Types.Predefined
{
    internal class PredefinedTypeSyntax : TypeNameExpressionSyntax
    {
        public PredefinedTypeSyntax(NameExpressionSyntax nameExpression) : base(nameExpression)
        {
        }
    }
}
