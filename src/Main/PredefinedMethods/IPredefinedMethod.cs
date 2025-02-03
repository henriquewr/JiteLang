using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.PredefinedExternMethods
{
    internal interface IPredefinedMethod
    {
        string Name { get; }
        TypeSymbol ReturnType { get; }
        List<TypeSymbol> ParamsTypes { get; }
    }
}
