using System;

namespace JiteLang.Main.Shared.Modifiers
{
    [Flags]
    internal enum Modifier
    {
        None   = 0b0000_0000,
        Static = 0b0000_0001,
        Extern = 0b0000_0010,
    }
}