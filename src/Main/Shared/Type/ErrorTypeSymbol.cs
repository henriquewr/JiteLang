using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Shared.Type
{
    internal class ErrorTypeSymbol : TypeSymbol
    {
        private ErrorTypeSymbol() : base("?", "?", false)
        {
        }

        public static readonly ErrorTypeSymbol Instance = new();

        public override int Size => 0;
    }
}