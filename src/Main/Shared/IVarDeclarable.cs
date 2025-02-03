using System.Collections.Generic;

namespace JiteLang.Main.Shared
{
    internal interface IVarDeclarable<TVar>
    {
        Dictionary<string, TVar> Variables { get; set; }
    }
}