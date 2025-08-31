using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Statements.Declarations;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitReturnStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.ReturnStatement;
        public EmitReturnStatement(EmitNode? parent, EmitExpression? returnValue = null) : base(parent)
        {
            ReturnValue = returnValue;
        }

        public EmitExpression? ReturnValue
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public EmitMethodDeclaration GetMethod()
        {
            var method = Parent!.GetFirstOrDefaultOfType<EmitMethodDeclaration>()!;

            return method;
        }
    }
}