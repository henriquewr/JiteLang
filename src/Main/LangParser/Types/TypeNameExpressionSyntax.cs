using JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming;

namespace JiteLang.Main.LangParser.Types
{
    internal class TypeNameExpressionSyntax : TypeSyntax
    {
        public TypeNameExpressionSyntax(NameExpressionSyntax nameExpression)
        {
            NameExpression = nameExpression;
        }

        public NameExpressionSyntax NameExpression { get; set; }
    }
}
