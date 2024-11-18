using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundIdentifierExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.IdentifierExpression;
        public BoundIdentifierExpression(BoundNode parent, string text, SyntaxPosition position) : base(parent)
        {
            Text = text;
            Position = position;
        }

        public string Text { get; set; }
    }
}
