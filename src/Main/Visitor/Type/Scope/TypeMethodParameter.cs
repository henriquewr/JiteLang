using JiteLang.Main.Bound;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeMethodParameter : TypeVariable
    {
        public TypeMethodParameter(TypeSymbol varType) : base(varType)
        {
        }
    }
}
