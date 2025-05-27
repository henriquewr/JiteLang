using JiteLang.Main.AsmBuilder.Scope;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitNamespaceDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.NamespaceDeclaration;

        public EmitNamespaceDeclaration(EmitNode? parent, string name, EmitBlockStatement<EmitClassDeclaration, CodeVariable> body) : base(parent, name)
        {
            Body = body;
        }

        public EmitBlockStatement<EmitClassDeclaration, CodeVariable> Body { get; set; }

        public override void SetParent()
        {
            Body.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Body.Parent = this;
            Body.SetParentRecursive();
        }
    }
}