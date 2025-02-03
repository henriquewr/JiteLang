using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class WhileStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.WhileStatement;

        public WhileStatementSyntax(SyntaxNode parent, ExpressionSyntax condition, BlockStatement<SyntaxNode> body) : base(parent)
        {
            Condition = condition;
            Body = body;
        }

        public WhileStatementSyntax(SyntaxNode parent, ExpressionSyntax condition) : base(parent)
        {
            Condition = condition;
            Body = new(this);
        }

        public ExpressionSyntax Condition { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }
    }
}
