﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class IdentifierExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.IdentifierExpression;

        public IdentifierExpressionSyntax(SyntaxNode parent, string text, SyntaxPosition position) : base(parent)
        {
            Text = text;
            Position = position;
        }

        public string Text { get; set; }
    }
}
