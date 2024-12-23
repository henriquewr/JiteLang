﻿using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.PredefinedExternMethods
{
    internal interface IPredefinedExternMethod
    {
        string Name { get; }
        TypeSymbol ReturnType { get; }
        List<TypeSymbol> ParamsTypes { get; }
    }
}
