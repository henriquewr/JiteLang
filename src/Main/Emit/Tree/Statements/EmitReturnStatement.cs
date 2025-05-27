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

        public EmitExpression? ReturnValue { get; set; }

        public EmitMethodDeclaration GetMethod()
        {
            var method = Parent!.GetFirstOrDefaultOfType<EmitMethodDeclaration>()!;

            return method;
        }

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