using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Statements.Declarations;
using System.Diagnostics;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitReturnStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.ReturnStatement;
        public EmitReturnStatement(EmitNode parent) : base(parent)
        { 
        }

        public EmitExpression? ReturnValue { get; set; }

        public EmitMethodDeclaration GetMethod()
        {
            var current = Parent;

            while (current != null)
            {
                if (current.Kind == EmitKind.MethodDeclaration)
                {
                    var method = (EmitMethodDeclaration)current;
                    return method;
                }

                current = current.Parent;
            }

            throw new UnreachableException();
        }
    }
}
