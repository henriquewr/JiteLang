using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundIfStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.IfStatement;

        public BoundIfStatement(BoundExpression condition, BoundBlockStatement<BoundNode> body, BoundStatement? @else = null)
        {
            Condition = condition;
            Body = body;
            Else = @else;
        }

        public BoundExpression Condition { get; set; }

        public BoundBlockStatement<BoundNode> Body { get; set; }

        public BoundStatement? Else { get; set; }
    }
}
