using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal abstract class BoundDeclaration : BoundStatement
    {
        public BoundDeclaration(BoundNode parent, BoundIdentifierExpression identifier) : base(parent)
        {
            Identifier = identifier; 
            Position = identifier.Position;
        }

        public BoundIdentifierExpression Identifier { get; init; }
    }
}
