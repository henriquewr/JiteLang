using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundWhileStatement : BoundLoopStatement
    {
        public override BoundKind Kind => BoundKind.WhileStatement;

        public BoundWhileStatement(
            BoundNode? parent,
            BoundExpression condition,
            BoundBlockStatement<BoundNode, TypeLocal> body) : base(parent)
        {
            Condition = condition;
            Body = body;
        }

        public BoundExpression Condition
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public BoundBlockStatement<BoundNode, TypeLocal> Body
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