using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Built.Expressions
{
    internal class BuiltIdentifierExpression : BuiltExpression
    {
        public override BuiltKind Kind => BuiltKind.IdentifierExpression;

        public BuiltIdentifierExpression(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
