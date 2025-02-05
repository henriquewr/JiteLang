using System;

namespace JiteLang.Main.Shared.Modifiers
{
    [Flags]
    internal enum AccessModifier
    {
        None    = 0b0000_0000,
        Public  = 0b0000_0001,
        Private = 0b0000_0010,
    }
}
