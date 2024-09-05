using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.Built.Expressions
{
    internal class BuiltAssignmentExpression : BuiltExpression
    {
        public override BuiltKind Kind => BuiltKind.AssignmentExpression;

        public BuiltAssignmentExpression(BuiltExpression left, SyntaxKind @operator, BuiltExpression right)
        {
            Right = right;
            Operator = @operator;
            Left = left;
        }

        public BuiltExpression Left { get; set; }
        public SyntaxKind Operator { get; set; }
        public BuiltExpression Right { get; set; }

    }
}
