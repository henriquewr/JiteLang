using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundVariableDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.VariableDeclaration;

        public BoundVariableDeclaration(BoundIdentifierExpression identifier, TypeSymbol type, BoundExpression? initialValue = null) : base(identifier) 
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
