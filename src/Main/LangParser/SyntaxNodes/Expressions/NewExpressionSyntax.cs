using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class NewExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NewExpression;

        public NewExpressionSyntax(TypeSyntax type, List<ExpressionSyntax> args) : base()
        {
            Type = type;
            Args = args;
        }

        public TypeSyntax Type { get; set; }
        public List<ExpressionSyntax> Args { get; set; }

        public override void SetParent()
        {
            foreach (var arg in Args)
            {
                arg.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            foreach (var arg in Args)
            {
                arg.Parent = this;
                arg.SetParentRecursive();
            }
        }
    }
}