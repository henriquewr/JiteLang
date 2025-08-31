using JiteLang.Syntax;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class CallExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public CallExpressionSyntax(ExpressionSyntax caller, IEnumerable<ExpressionSyntax> args) : base()
        {
            Caller = caller;
            Args = new(args);
            Position = caller.Position;
        }

        public ExpressionSyntax Caller
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

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