using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class WhileStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.WhileStatement;

        public WhileStatementSyntax(ExpressionSyntax condition, BlockStatement<SyntaxNode> body)
        {
            Condition = condition;
            Body = body;
        }

        public WhileStatementSyntax(ExpressionSyntax condition) : this(condition, new())
        {
        }

        public ExpressionSyntax Condition { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }
    }
}
