using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeVariable
    {
        public CodeVariable(int stackLocation, TypeSymbol type, bool stackLocationIsPositive = default)
        {
            InScopeStackLocation = stackLocation;
            Type = type;
            StackLocationIsPositive = stackLocationIsPositive;
        }
        
        public bool StackLocationIsPositive { get; set; }
        public int InScopeStackLocation { get; set; }
        public TypeSymbol Type { get; set; }
    }
}
