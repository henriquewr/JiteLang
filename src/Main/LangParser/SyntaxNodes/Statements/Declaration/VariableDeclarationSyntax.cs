using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal abstract class VariableDeclarationSyntax : StatementSyntax
    { 
        public VariableDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type) : base()
        {
            Type = type;
            Identifier = identifier;
        }
        
        public TypeSyntax Type { get; set; }
        public abstract IdentifierExpressionSyntax Identifier { get; set; }
        public abstract ExpressionSyntax? InitialValue { get; set; }
    }
}