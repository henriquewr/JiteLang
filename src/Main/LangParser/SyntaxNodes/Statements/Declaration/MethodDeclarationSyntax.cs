using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class MethodDeclarationSyntax : DeclarationSyntax
    {
        public MethodDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax returnType) : base(identifier)
        {
            ReturnType = returnType;
        }

        public override SyntaxKind Kind => SyntaxKind.MethodDeclaration;

        public TypeSyntax ReturnType { get; set; }

        public List<ParameterDeclarationSyntax> Params { get; set; } = new List<ParameterDeclarationSyntax>();

        public List<SyntaxToken> Modifiers { get; set; } = new();

        public BlockStatement<SyntaxNode> Body { get; set; } = new();
    }
}
