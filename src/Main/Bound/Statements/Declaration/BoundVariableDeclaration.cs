using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal abstract class BoundVariableDeclaration : BoundDeclaration
    {
        public BoundVariableDeclaration(BoundNode? parent, BoundIdentifierExpression identifier, TypeSymbol type) : base(parent, identifier) 
        {
            Type = type;
        }

        public TypeSymbol Type { get; set; }
    }
}