using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundFieldDeclaration : BoundVariableDeclaration
    {
        public override BoundKind Kind => BoundKind.FieldDeclaration;

        public BoundFieldDeclaration(BoundNode parent, BoundIdentifierExpression identifierExpression, TypeSymbol type) : this(parent, identifierExpression, type, new())
        {
        }

        public BoundFieldDeclaration(BoundNode parent, BoundIdentifierExpression identifierExpression, TypeSymbol type, List<SyntaxToken> modifiers) : base(parent, identifierExpression, type)
        {
            Modifiers = modifiers;
        }

        public List<SyntaxToken> Modifiers { get; set; }
    }
}