using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class VariableDeclarationSyntax : DeclarationSyntax
    { 
        //TODO make this class abstract and create a LocalDeclarationSyntax
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;

        public VariableDeclarationSyntax(SyntaxNode parent, IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax? initialValue = null) : base(parent, identifier)
        {
            Type = type;
            InitialValue = initialValue;
        }
        
        public TypeSyntax Type { get; set; }
        public ExpressionSyntax? InitialValue { get; set; }
    }
}