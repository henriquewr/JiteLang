using JiteLang.Main.Shared;
using JiteLang.Syntax;
using System;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class BinaryExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public override SyntaxPosition Position { get => Left?.Position ?? default; set => throw new InvalidOperationException(); }

        public BinaryExpressionSyntax(ExpressionSyntax left, BinaryOperatorKind operation, ExpressionSyntax right) : base()
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public ExpressionSyntax Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public BinaryOperatorKind Operation { get; set; }

        public ExpressionSyntax Right
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}