using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.AsmBuilder.Scope
{
    internal class CodeLocal : CodeVariable
    {
        public CodeLocal(int stackLocation, TypeSymbol type) : base(type)
        {
            InScopeStackLocation = stackLocation;
        }

        public int InScopeStackLocation { get; set; }
    }
}
