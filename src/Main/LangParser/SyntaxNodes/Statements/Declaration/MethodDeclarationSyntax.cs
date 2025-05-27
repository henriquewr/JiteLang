using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class MethodDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public MethodDeclarationSyntax(IdentifierExpressionSyntax identifier, 
            TypeSyntax returnType, 
            BlockStatement<SyntaxNode> body, 
            List<ParameterDeclarationSyntax> @params, 
            List<SyntaxToken> modifiers) : base(identifier)
        {
            ReturnType = returnType;
            Modifiers = modifiers;
            Params = @params;
            Body = body;
        }
     
        public TypeSyntax ReturnType { get; set; }

        public List<ParameterDeclarationSyntax> Params { get; set; }

        public List<SyntaxToken> Modifiers { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }


        public override void SetParent()
        {
            Body.Parent = this;
            Identifier.Parent = this;

            foreach (var param in Params)
            {
                param.Parent = Body;
            }
        }

        public override void SetParentRecursive()
        {
            Body.Parent = this;
            Identifier.Parent = this;

            Body.SetParentRecursive();
            Identifier.SetParentRecursive();

            foreach (var param in Params)
            {
                param.Parent = Body;
                param.SetParentRecursive();
            }
        }
    }
}
