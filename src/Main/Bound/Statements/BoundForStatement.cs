using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundForStatement : BoundLoopStatement
    {
        public override BoundKind Kind => BoundKind.ForStatement;

        public BoundForStatement(
            BoundNode? parent, BoundNode initializer,
            BoundExpression condition,
            BoundExpression incrementor,
            BoundBlockStatement<BoundNode, TypeLocal> body) : base(parent)
        {
            Initializer = initializer;
            Condition = condition;
            Incrementor = incrementor;
            Body = body;
        }

        public BoundNode Initializer
        {
            get;
            set
            {
                field = value;
                field?.Parent = Body;
            }
        }

        public BoundExpression Condition
        {
            get;
            set
            {
                field = value;
                field?.Parent = Body;
            }
        }

        public BoundExpression Incrementor
        {
            get;
            set
            {
                field = value;
                field?.Parent = Body;
            }
        }

        public BoundBlockStatement<BoundNode, TypeLocal> Body
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
