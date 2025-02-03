

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal abstract class EmitVariableDeclaration : EmitDeclaration
    {
        public EmitVariableDeclaration(EmitNode parent, string name) : base(parent, name)
        {
        }
    }
}
