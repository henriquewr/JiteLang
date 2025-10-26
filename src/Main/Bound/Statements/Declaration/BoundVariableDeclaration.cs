using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal abstract class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(BoundNode? parent, BoundIdentifierExpression identifier, TypeSymbol type) : base(parent) 
        {
            Identifier = identifier;
            Type = type;
        }

        public abstract BoundIdentifierExpression Identifier { get; set; }

        public TypeSymbol Type { get; set; }
    }
}