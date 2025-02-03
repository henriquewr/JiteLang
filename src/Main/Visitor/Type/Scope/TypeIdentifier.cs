using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal abstract class TypeIdentifier
    {
        public TypeIdentifier(TypeSymbol type, string name)
        {
            Type = type;
            Name = name;
        }
        public string Name { get; set; }
        public TypeSymbol Type { get; set; }
    }
}