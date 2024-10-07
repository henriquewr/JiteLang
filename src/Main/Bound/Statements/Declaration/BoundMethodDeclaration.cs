using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.PredefinedExternMethods;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundMethodDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.MethodDeclaration;

        public BoundMethodDeclaration(BoundIdentifierExpression identifierExpression,
            TypeSymbol returnType,
            BoundBlockStatement<BoundNode> body,
            List<SyntaxToken> modifiers,
            List<BoundParameterDeclaration> @params
            ) : base(identifierExpression)
        {
            Body = body;
            Modifiers = modifiers;
            Params = @params;
            ReturnType = returnType;
        }

        public BoundMethodDeclaration(BoundIdentifierExpression identifierExpression,
            TypeSymbol returnType,
            BoundBlockStatement<BoundNode> body
            ) : this(identifierExpression, returnType, body, new(), new())
        {
        }

        public BoundMethodDeclaration(BoundIdentifierExpression identifierExpression,
            BoundBlockStatement<BoundNode> body
            ) : this(identifierExpression, PredefinedTypeSymbol.Void, body)
        {
        }

        public BoundMethodDeclaration(BoundIdentifierExpression identifierExpression)
            : this(identifierExpression, PredefinedTypeSymbol.Void, new())
        {
        }

        public List<SyntaxToken> Modifiers { get; set; }
        public List<BoundParameterDeclaration> Params { get; set; }
        public BoundBlockStatement<BoundNode> Body { get; set; }
        public TypeSymbol ReturnType { get; set; }
    }
}
