using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal abstract class VariableDeclarationSyntax : DeclarationSyntax
    { 
        public VariableDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type) : base(identifier)
        {
            Type = type;
        }
        
        public TypeSyntax Type { get; set; }
        public abstract ExpressionSyntax? InitialValue { get; set; }
    }
}