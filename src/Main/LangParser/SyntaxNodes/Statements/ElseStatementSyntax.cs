using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class ElseStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ElseStatement;

        public ElseStatementSyntax(StatementSyntax @else) : base()
        {
            Else = @else;
        }

        public StatementSyntax Else { get; set; }

        public override void SetParent()
        {
            Else.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Else.Parent = this;
            Else.SetParentRecursive();
        }
    }
}