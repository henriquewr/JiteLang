using JiteLang.Main.Builder;
using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundReturnStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.ReturnStatement;

        public BoundReturnStatement(BoundExpression? returnValue = null)
        {
            ReturnValue = returnValue;
        }

        public BoundExpression? ReturnValue { get; set; }
    }
}
