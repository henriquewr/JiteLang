using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Shared;
using System.Diagnostics;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitVariableDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.VariableDeclaration;
        public EmitVariableDeclaration(EmitNode parent, string text) : base(parent)
        {
            Text = text;
        }

        public string Text { get; set; }

        public EmitExpression? InitialValue { get; set; }

        public CodeVariable GetVariable()
        {
            return GetVariable(Parent, Text);
        }

        private static CodeVariable GetVariable(EmitNode node, string name)
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

        private static IVarDeclarable<CodeVariable> GetNearestVarDeclarable(EmitNode node, out EmitNode declarableNode)
        {
            declarableNode = node;

            while (declarableNode != null)
            {
                if (declarableNode is IVarDeclarable<CodeVariable> declarable)
                {
                    return declarable;
                }

                declarableNode = declarableNode.Parent;
            }

            throw new UnreachableException();
        }
    }
}
