using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class IdentifierExpressionSyntax : ExpressionSyntax
    {
        public IdentifierExpressionSyntax(string text, SyntaxPosition position)
        {
            Text = text;
            Position = position;
        }

        public override SyntaxKind Kind => SyntaxKind.IdentifierExpression;
        public string Text { get; set; }
    }
}
