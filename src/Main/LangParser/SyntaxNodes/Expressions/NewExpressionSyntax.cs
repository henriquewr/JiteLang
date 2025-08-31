using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class NewExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NewExpression;

        public NewExpressionSyntax(TypeSyntax type, IEnumerable<ExpressionSyntax> args) : base()
        {
            Type = type;
            Args = new(args);
        }

        public TypeSyntax Type { get; set; }
        protected void OnAdd(ExpressionSyntax item)
        {
            item.Parent = this;
        }

        public NotifyAddList<ExpressionSyntax> Args
        {
            get;
            set
            {
                field?.OnAdd -= OnAdd;
                field = value;

                if (field is not null)
                {
                    field.OnAdd += OnAdd;

                    foreach (var member in Args)
                    {
                        OnAdd(member);
                    }
                }
            }
        }
    }
}