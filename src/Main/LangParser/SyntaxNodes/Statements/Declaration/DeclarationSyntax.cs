using JiteLang.Main.LangParser.SyntaxNodes.Expressions;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal abstract class DeclarationSyntax : StatementSyntax
    {
        public DeclarationSyntax(SyntaxNode parent, IdentifierExpressionSyntax identifier) : base(parent)
        {
            Identifier = identifier;
            Position = identifier.Position;
        }
        
        public IdentifierExpressionSyntax Identifier { get; init; }
    }
}
