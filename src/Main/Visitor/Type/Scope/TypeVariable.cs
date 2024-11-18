using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeVariable
    {
        public TypeVariable(TypeSymbol varType)
        {
            Type = varType;
        }
        public TypeSymbol Type { get; set; }
    }
}
