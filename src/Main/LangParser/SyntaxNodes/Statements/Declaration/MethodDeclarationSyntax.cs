using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class MethodDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public MethodDeclarationSyntax(IdentifierExpressionSyntax identifier, 
            TypeSyntax returnType, 
            BlockStatement<SyntaxNode> body, 
            IEnumerable<ParameterDeclarationSyntax> @params, 
            List<SyntaxToken> modifiers) : base(identifier)
        {
            Identifier = identifier;
            ReturnType = returnType;
            Modifiers = modifiers;
            Params = new(@params);
            Body = body;
        }
     
        public TypeSyntax ReturnType { get; set; }

        protected void OnAdd(ParameterDeclarationSyntax item)
        {
            item.Parent = Body;
        }
        public NotifyAddList<ParameterDeclarationSyntax> Params
        {
            get;
            set
            {
                field?.OnAdd -= OnAdd;
                field = value;

                if (field is not null)
                {
                    field.OnAdd += OnAdd;

                    foreach (var member in Params)
                    {
                        OnAdd(member);
                    }
                }
            }
        }

        public List<SyntaxToken> Modifiers { get; set; }

        public BlockStatement<SyntaxNode> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;

                if (Params is not null)
                {
                    foreach (var param in Params)
                    {
                        OnAdd(param);
                    }
                }
            }
        }

        public override IdentifierExpressionSyntax Identifier
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
