using JiteLang.Main.Bound;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeVariable
    {
        public CodeVariable(int stackLocation, TypeSymbol type)
        {
            InScopeStackLocation = stackLocation;
            Type = type;
        }

        public CodeVariable(int stackLocation, TypeSymbol type, bool stackLocationIsPositive)
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
