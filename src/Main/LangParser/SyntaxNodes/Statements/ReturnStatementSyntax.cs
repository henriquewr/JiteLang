using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class ReturnStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;

        public ReturnStatementSyntax(SyntaxNode parent) : base(parent)
        {
        }

        public ReturnStatementSyntax(SyntaxNode parent, ExpressionSyntax returnValue) : base(parent)
        {
            ReturnValue = returnValue;
        }

        public ExpressionSyntax? ReturnValue { get; set; }
    }
}
