using System.Collections.Generic;

namespace JiteLang.Main.Shared
{
    internal interface IVarDeclarable<T>
    {
        Dictionary<string, T> Variables { get; set; }
    }
}
