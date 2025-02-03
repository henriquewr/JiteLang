using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class MethodDeclarationSyntax : DeclarationSyntax
    {
        public MethodDeclarationSyntax(SyntaxNode parent, IdentifierExpressionSyntax identifier, TypeSyntax returnType, List<SyntaxToken> modifiers) : base(parent, identifier)
        {
            ReturnType = returnType;
            Modifiers = modifiers;
            Params = new();
            Body = new(this);
        }

        public MethodDeclarationSyntax(SyntaxNode parent, IdentifierExpressionSyntax identifier, TypeSyntax returnType) : this(parent, identifier, returnType, new())
        {
        } 
     

        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public TypeSyntax ReturnType { get; set; }

        public List<ParameterDeclarationSyntax> Params { get; set; }

        public List<SyntaxToken> Modifiers { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }
    }
}
