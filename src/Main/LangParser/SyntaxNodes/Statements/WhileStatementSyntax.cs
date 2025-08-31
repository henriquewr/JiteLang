using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class WhileStatementSyntax : LoopStatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.WhileStatement;

        public WhileStatementSyntax(ExpressionSyntax condition, BlockStatement<SyntaxNode> body) : base()
        {
            Condition = condition;
            Body = body;
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
    }
}
