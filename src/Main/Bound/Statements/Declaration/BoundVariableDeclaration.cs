using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundVariableDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.VariableDeclaration;

        public BoundVariableDeclaration(BoundNode parent, BoundIdentifierExpression identifier, TypeSymbol type, BoundExpression? initialValue = null) : base(parent, identifier) 
        {
            InitialValue = initialValue;
            Type = type;
            Modifiers = new();
        }

        public TypeSymbol Type { get; set; }
        public BoundExpression? InitialValue { get; set; }
        public List<SyntaxToken> Modifiers { get; set; }

    }
}
