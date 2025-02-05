using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class IfStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public IfStatementSyntax(SyntaxNode parent, ExpressionSyntax condition, BlockStatement<SyntaxNode> body, ElseStatementSyntax? @else = null) : base(parent)
        {
            Condition = condition;
            Body = body;
            Else = @else;
        }

        public IfStatementSyntax(SyntaxNode parent, ExpressionSyntax condition) : base(parent)
        {
            Condition = condition;
            Body = new(this);
        }

        public ExpressionSyntax Condition { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }

        public ElseStatementSyntax? Else { get; set; }
    }
}
