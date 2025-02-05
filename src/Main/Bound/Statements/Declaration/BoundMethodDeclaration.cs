using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.Shared.Modifiers;
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
           List<BoundParameterDeclaration> @params
           ) : base(parent, identifierExpression)
        {
            Body = new(this);
            Params = @params;
            ReturnType = returnType;
        }

        public BoundMethodDeclaration(BoundNode parent,
            BoundIdentifierExpression identifierExpression,
            TypeSymbol returnType,
            BoundBlockStatement<BoundNode> body,
            List<BoundParameterDeclaration> @params
            ) : base(parent, identifierExpression)
        {
            Body = body;
            Params = @params;
            ReturnType = returnType;
        }

        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }
        public List<BoundParameterDeclaration> Params { get; set; }
        public BoundBlockStatement<BoundNode> Body { get; set; }
        public TypeSymbol ReturnType { get; set; }
        public bool IsInitializer { get; set; }
    }
}