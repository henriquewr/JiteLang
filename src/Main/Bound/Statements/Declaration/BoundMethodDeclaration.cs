using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Shared.Type.Members.Method;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundMethodDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.MethodDeclaration;

        public BoundMethodDeclaration(
            BoundNode? parent,
            BoundIdentifierExpression identifierExpression,
            TypeSymbol returnType,
            BoundBlockStatement<BoundNode, TypeLocal> body,
            List<BoundParameterDeclaration> @params
            ) : base(parent, identifierExpression)
        {
            Body = body;
            Params = @params;
            ReturnType = returnType;
        }

        public override void SetParent()
        {
            Body.Parent = this;
            Identifier.Parent = this;

            foreach (var param in Params)
            {
                param.Parent = Body;
            }
        }

        public override void SetParentRecursive()
        {
            Body.Parent = this;
            Identifier.Parent = this;

            Body.SetParentRecursive();
            Identifier.SetParentRecursive();

            foreach (var param in Params)
            {
                param.Parent = Body;
                param.SetParentRecursive();
            }
        }

        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }
        public List<BoundParameterDeclaration> Params { get; set; }
        public BoundBlockStatement<BoundNode, TypeLocal> Body { get; set; }
        public TypeSymbol ReturnType { get; set; }
        public bool IsInitializer { get; set; }

        public DelegateTypeSymbol GetDelegateType()
        {
            var delegateType = DelegateTypeSymbol.Generate(Identifier.Text, ReturnType, Params.Select(x => x.Type));
            return delegateType;
        }

        public MethodSymbol GetSymbol()
        {
            var delegateType = GetDelegateType();
            var methodSymbol = new MethodSymbol(Identifier.Text, delegateType);
            return methodSymbol;
        }
    }
}