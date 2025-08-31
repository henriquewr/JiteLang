using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundFieldDeclaration : BoundVariableDeclaration
    {
        public override BoundKind Kind => BoundKind.FieldDeclaration;

        public BoundFieldDeclaration(BoundNode? parent, BoundIdentifierExpression identifierExpression, TypeSymbol type) : base(parent, type)
        {
            Identifier = identifierExpression;
        }

        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }
        public override BoundIdentifierExpression Identifier
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}