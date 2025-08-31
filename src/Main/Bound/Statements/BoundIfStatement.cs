using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundIfStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.IfStatement;

        public BoundIfStatement(
            BoundNode? parent,
            BoundExpression condition,
            BoundBlockStatement<BoundNode, TypeLocal> body,
            BoundElseStatement? @else = null) : base(parent)
        {
            Condition = condition;
            Body = body;
            Else = @else;
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

        public BoundElseStatement? Else
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