using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.LangParser.Types
{
    internal abstract class TypeSyntax
    {
        public TypeSyntax(string text)
        {
            Text = text;
        }

        public abstract bool IsPreDefined { get; }
        public string Text { get; set; }
    }
}
