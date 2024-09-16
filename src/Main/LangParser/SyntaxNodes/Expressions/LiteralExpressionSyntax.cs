using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken value) 
        {
            Value = value;
            Position = value.Position;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken Value { set; get; }
    }
}
