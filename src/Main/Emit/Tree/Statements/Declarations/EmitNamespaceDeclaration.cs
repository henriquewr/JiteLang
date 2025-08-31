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

        public EmitBlockStatement<EmitClassDeclaration, CodeVariable> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}