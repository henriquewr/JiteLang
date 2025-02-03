using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal abstract class CodeVariable
    {
        public CodeVariable(TypeSymbol type)
        {
            Type = type;
        }
        
        public TypeSymbol Type { get; set; }
    }
}
