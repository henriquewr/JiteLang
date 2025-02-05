﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxNode parent) : base(parent)
        {
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
    }
}
