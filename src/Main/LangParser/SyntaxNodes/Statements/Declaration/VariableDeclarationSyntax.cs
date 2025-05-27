using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal abstract class VariableDeclarationSyntax : DeclarationSyntax
    { 
        public VariableDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax? initialValue = null) : base(identifier)
        {
            Type = type;
            InitialValue = initialValue;
        }
        
        public TypeSyntax Type { get; set; }
        public ExpressionSyntax? InitialValue { get; set; }
    }
}