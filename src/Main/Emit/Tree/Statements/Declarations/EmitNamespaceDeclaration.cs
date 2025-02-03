using JiteLang.Main.AsmBuilder.Scope;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitNamespaceDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.NamespaceDeclaration;

        public EmitNamespaceDeclaration(EmitNode parent, string name) : base(parent, name)
        {
            Body = new(this);
        }

        public EmitBlockStatement<EmitClassDeclaration, CodeVariable> Body { get; set; }
    }
}