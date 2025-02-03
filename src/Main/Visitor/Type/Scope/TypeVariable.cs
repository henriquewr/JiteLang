using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeVariable : TypeIdentifier
    {
        public TypeVariable(TypeSymbol varType, string name) : base(varType, name)
        {
        }
    }
}
