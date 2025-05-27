using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;
using System.Diagnostics;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitIdentifierExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.IdentifierExpression;
        public override TypeSymbol Type { get; set; }
        public EmitIdentifierExpression(EmitNode? parent, string text, TypeSymbol type) : base(parent)
        {
            Text = text;
            Type = type;
        }
        public string Text { get; set; }

        public CodeLocal GetLocal()
        {
            return GetLocal(Parent, Text);
        }

        private static CodeLocal GetLocal(EmitNode node, string name)
        {
            var current = GetNearestLocalDeclarable(node, out var currentDeclarableNode);

            while (current != null)
            {
                if (current.Variables.TryGetValue(name, out var value))
                {
                    return value;
                }

                current = GetNearestLocalDeclarable(currentDeclarableNode.Parent, out currentDeclarableNode);
            }

            throw new UnreachableException();
        }

        private static IVarDeclarable<CodeLocal>? GetNearestLocalDeclarable(EmitNode node, out EmitNode declarableNode)
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

            return null;
        }

        public override void SetParent()
        {
        }

        public override void SetParentRecursive()
        {
        }
    }
}