using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class IfStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public IfStatementSyntax(ExpressionSyntax condition, BlockStatement<SyntaxNode> body, StatementSyntax? @else)
        {
            Condition = condition;
            Body = body;
            Else = @else;
        }

        public IfStatementSyntax(ExpressionSyntax condition, BlockStatement<SyntaxNode> body) : this(condition, body, null)
        {
        }

        public IfStatementSyntax(ExpressionSyntax condition) : this(condition, new(), null)
        {
        }

        public ExpressionSyntax Condition { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }

        public StatementSyntax? Else { get; set; }
    }
}
