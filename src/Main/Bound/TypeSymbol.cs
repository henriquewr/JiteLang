using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound
{
    internal class TypeSymbol
    {
        public TypeSymbol(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        public static TypeSymbol None => new("?");

        public bool IsEqualsNotNone(TypeSymbol? type)
        {
            var isSame = Text == type?.Text;
            var isSameNotNone = isSame && Text != None.Text;
            return isSameNotNone;
        }
    }
}
