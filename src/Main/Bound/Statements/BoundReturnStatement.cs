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

        public BoundExpression? ReturnValue { get; set; }

        public override void SetParent()
        {
            if (ReturnValue is not null)
            {
                ReturnValue.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            if (ReturnValue is not null)
            {
                ReturnValue.Parent = this;
                ReturnValue.SetParentRecursive();
            }
        }
    }
}