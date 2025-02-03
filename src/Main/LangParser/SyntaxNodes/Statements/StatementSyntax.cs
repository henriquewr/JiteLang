namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal abstract class StatementSyntax : SyntaxNode
    {
        protected StatementSyntax(SyntaxNode parent) : base(parent)
        {
        }
    }
}
