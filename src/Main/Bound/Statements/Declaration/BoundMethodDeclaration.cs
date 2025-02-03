using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundMethodDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.MethodDeclaration;

        public BoundMethodDeclaration(BoundNode parent,
           BoundIdentifierExpression identifierExpression,
           TypeSymbol returnType,
           List<SyntaxToken> modifiers,
           List<BoundParameterDeclaration> @params
           ) : base(parent, identifierExpression)
        {
            Body = new(this);
            Modifiers = modifiers;
            Params = @params;
            ReturnType = returnType;
        }

        public BoundMethodDeclaration(BoundNode parent,
            BoundIdentifierExpression identifierExpression,
            TypeSymbol returnType,
            BoundBlockStatement<BoundNode> body,
            List<SyntaxToken> modifiers,
            List<BoundParameterDeclaration> @params
            ) : base(parent, identifierExpression)
        {
            Body = body;
            Modifiers = modifiers;
            Params = @params;
            ReturnType = returnType;
        }

        public List<SyntaxToken> Modifiers { get; set; }
        public List<BoundParameterDeclaration> Params { get; set; }
        public BoundBlockStatement<BoundNode> Body { get; set; }
        public TypeSymbol ReturnType { get; set; }
        public bool IsInitializer { get; set; }
    }
}
