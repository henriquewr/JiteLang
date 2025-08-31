using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class IfStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public IfStatementSyntax(ExpressionSyntax condition, BlockStatement<SyntaxNode> body, ElseStatementSyntax? @else = null) : base()
        {
            Condition = condition;
            Body = body;
            Else = @else;
        }

        public ExpressionSyntax Condition
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public BlockStatement<SyntaxNode> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public ElseStatementSyntax? Else
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}