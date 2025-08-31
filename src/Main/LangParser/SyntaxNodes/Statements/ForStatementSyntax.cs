using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class ForStatementSyntax : LoopStatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public ForStatementSyntax(SyntaxNode initializer,
            ExpressionSyntax condition,
            ExpressionSyntax incrementor,
            BlockStatement<SyntaxNode> body) : base()
        {
            Initializer = initializer;
            Condition = condition;
            Incrementor = incrementor;
            Body = body;
        }

        public SyntaxNode Initializer
        {
            get;
            set
            {
                field = value;
                field?.Parent = Body;
            }
        }

        public ExpressionSyntax Condition
        {
            get;
            set
            {
                field = value;
                field?.Parent = Body;
            }
        }

        public ExpressionSyntax Incrementor
        {
            get;
            set
            {
                field = value;
                field?.Parent = Body;
            }
        }

        public BlockStatement<SyntaxNode> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;

                Initializer?.Parent = field;
                Condition?.Parent = field;
                Incrementor?.Parent = field;
            }
        }
    }
}
