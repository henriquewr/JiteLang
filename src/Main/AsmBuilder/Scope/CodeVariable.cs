using JiteLang.Main.LangParser.Types;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeVariable
    {
        public CodeVariable(int stackLocation, TypeSyntax type)
        {
            InScopeStackLocation = stackLocation;
            Type = type;
        }

        public CodeVariable(int stackLocation, TypeSyntax type, bool stackLocationIsPositive)
        {
            InScopeStackLocation = stackLocation;
            Type = type;
            StackLocationIsPositive = stackLocationIsPositive;
        }

        public bool StackLocationIsPositive { get; set; }
        public int InScopeStackLocation { get; set; }
        public TypeSyntax Type { get; set; }
    }
}
