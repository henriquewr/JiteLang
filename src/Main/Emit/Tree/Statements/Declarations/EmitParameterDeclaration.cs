
namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitParameterDeclaration : EmitVariableDeclaration
    {
        public override EmitKind Kind => EmitKind.ParameterDeclaration;
        public EmitParameterDeclaration(EmitNode? parent, string name) : base(parent, name)
        {
        }
    }
}