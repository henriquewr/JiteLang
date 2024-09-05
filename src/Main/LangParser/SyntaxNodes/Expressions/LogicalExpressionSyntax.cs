using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class LogicalExpressionSyntax : ExpressionSyntax
    {
        public LogicalExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, SyntaxKind operation) 
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public override SyntaxKind Kind => SyntaxKind.LogicalExpression;

        public ExpressionSyntax Left { get; set; }
        public ExpressionSyntax Right { get; set; }
        public SyntaxKind Operation { get; set; }
    }
}
