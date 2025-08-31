using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal abstract class BoundDeclaration : BoundStatement
    {
        public BoundDeclaration(BoundNode? parent) : base(parent)
        {
        }

        public abstract BoundIdentifierExpression Identifier { get; set; }
    }
}