using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.Built.Expressions
{
    internal class BuiltBinaryExpression : BuiltExpression
    {
        public override BuiltKind Kind => BuiltKind.BinaryExpression;
        public BuiltBinaryExpression(BuiltExpression left, SyntaxKind operation, BuiltExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public BuiltExpression Left { get; set; }
        public SyntaxKind Operation { get; set; }
        public BuiltExpression Right { get; set; }
    }
}
