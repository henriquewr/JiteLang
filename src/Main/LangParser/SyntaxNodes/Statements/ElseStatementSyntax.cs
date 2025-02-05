using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class ElseStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public ElseStatementSyntax(SyntaxNode parent, StatementSyntax @else) : base(parent)
        {
            Else = @else;
        }

        public StatementSyntax Else { get; set; }
    }
}