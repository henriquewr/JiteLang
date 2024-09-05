using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using JiteLang.Main.Built.Expressions;

namespace JiteLang.Main.Built.Statements.Declaration
{
    internal class BuiltVariableDeclaration : BuiltDeclaration
    {
        public override BuiltKind Kind => BuiltKind.VariableDeclaration;

        public BuiltVariableDeclaration(BuiltIdentifierExpression identifier, TypeSyntax type) : base(identifier)
        {
            Type = type;
        }

        public BuiltVariableDeclaration(BuiltIdentifierExpression identifier, TypeSyntax type, BuiltExpression initialValue) : this(identifier, type)
        {
            InitialValue = initialValue;
        }

        public TypeSyntax Type { get; set; }
        public BuiltExpression? InitialValue { get; set; }
        public List<SyntaxToken> Modifiers { get; set; } = new();

    }
}
