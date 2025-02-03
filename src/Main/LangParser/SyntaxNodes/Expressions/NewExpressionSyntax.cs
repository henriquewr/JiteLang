using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class NewExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NewExpression;

        public NewExpressionSyntax(SyntaxNode parent, TypeSyntax type, List<ExpressionSyntax> args) : base(parent)
        {
            Type = type;
            Args = args;
        }

        public TypeSyntax Type { get; set; }
        public List<ExpressionSyntax> Args { get; set; }
    }
}
