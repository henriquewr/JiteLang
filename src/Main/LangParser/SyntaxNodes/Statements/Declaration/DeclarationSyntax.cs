using JiteLang.Main.LangParser.SyntaxNodes.Expressions;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal abstract class DeclarationSyntax : StatementSyntax
    {
        public DeclarationSyntax(IdentifierExpressionSyntax identifier) : base()
        {
            Identifier = identifier;
            Position = identifier.Position;
        }
        
        public IdentifierExpressionSyntax Identifier { get; init; }
    }
}
