using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Shared;
using System.Diagnostics;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitLocalDeclaration : EmitVariableDeclaration
    {
        public override EmitKind Kind => EmitKind.LocalDeclaration;

        public EmitLocalDeclaration(EmitNode? parent, string name, EmitExpression? initialValue = null) : base(parent, name)
        {
            InitialValue = initialValue;
        }

        public EmitExpression? InitialValue { get; set; }

        public CodeLocal GetVariable()
        {
            return GetVariable(Parent, Name);
        }

        private static CodeLocal GetVariable(EmitNode node, string name)
        {
            var current = GetNearestVarDeclarable(node, out var currentDeclarableNode);

            while (current != null)
            {
                if (current.Variables.TryGetValue(name, out var value))
                {
                    return value;
                }

                current = GetNearestVarDeclarable(currentDeclarableNode.Parent, out currentDeclarableNode);
            }

            throw new UnreachableException();
        }

        private static IVarDeclarable<CodeLocal> GetNearestVarDeclarable(EmitNode node, out EmitNode declarableNode)
        {
            declarableNode = node;

            while (declarableNode != null)
            {
                if (declarableNode is IVarDeclarable<CodeLocal> declarable)
                {
                    return declarable;
                }

                declarableNode = declarableNode.Parent;
            }

            throw new UnreachableException();
        }

        public override void SetParent()
        {
            if (InitialValue is not null)
            {
                InitialValue.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            if (InitialValue is not null)
            {
                InitialValue.Parent = this;
                InitialValue.SetParentRecursive();
            }
        }
    }
}