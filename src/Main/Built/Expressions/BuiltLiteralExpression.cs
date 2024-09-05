using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes;

namespace JiteLang.Main.Built.Expressions
{
    internal class BuiltLiteralExpression : BuiltExpression
    {
        public override BuiltKind Kind => BuiltKind.LiteralExpression;

        public BuiltLiteralExpression(SyntaxToken value)
        {
            Value = value;
        }

        public SyntaxToken Value { set; get; }
    }
}
