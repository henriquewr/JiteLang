using JiteLang.Main.Shared.Modifiers;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitFieldDeclaration : EmitVariableDeclaration
    {
        public override EmitKind Kind => EmitKind.FieldDeclaration;

        public EmitFieldDeclaration(EmitNode? parent, string name) : base(parent, name)
        {
        }

        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }

        public override void SetParent()
        {
        }

        public override void SetParentRecursive()
        {
        }
    }
}