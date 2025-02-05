﻿using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class BinaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public BinaryExpressionSyntax(SyntaxNode parent, ExpressionSyntax left, BinaryOperatorKind operation, ExpressionSyntax right) : base(parent)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public ExpressionSyntax Left { get; set; }
        public BinaryOperatorKind Operation { get; set; }
        public ExpressionSyntax Right { get; set; }
    }
}
