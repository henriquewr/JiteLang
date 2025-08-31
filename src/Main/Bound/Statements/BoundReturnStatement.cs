using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundReturnStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.ReturnStatement;

        public BoundReturnStatement(BoundNode? parent, BoundExpression? returnValue = null) : base(parent)
        {
            ReturnValue = returnValue;
        }

        public BoundExpression? ReturnValue
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