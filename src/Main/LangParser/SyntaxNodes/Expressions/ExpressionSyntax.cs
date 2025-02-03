using JiteLang.Main.LangLexer.Token;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal abstract class ExpressionSyntax : SyntaxNode
    {
        protected ExpressionSyntax(SyntaxNode parent) : base(parent)
        {
        }
    }
}
