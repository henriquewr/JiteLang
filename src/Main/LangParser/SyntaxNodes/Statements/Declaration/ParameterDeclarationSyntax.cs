using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ParameterDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ParameterDeclaration;

        public ParameterDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type) : base(identifier)
        {
            Type = type;
        }

        public TypeSyntax Type { get; set; }
    }
}
