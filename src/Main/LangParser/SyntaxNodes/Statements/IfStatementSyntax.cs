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

        public ExpressionSyntax Condition { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }

        public ElseStatementSyntax? Else { get; set; }


        public override void SetParent()
        {
            Condition.Parent = this;
            Body.Parent = this;

            if (Else is not null)
            {
                Else.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            Condition.Parent = this;
            Condition.SetParentRecursive();

            Body.Parent = this;
            Body.SetParentRecursive();

            if (Else is not null)
            {
                Else.Parent = this;
                Else.SetParentRecursive();
            }
        }
    }
}