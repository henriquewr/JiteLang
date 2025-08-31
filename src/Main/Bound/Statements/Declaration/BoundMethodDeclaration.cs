using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Shared.Type.Members.Method;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Syntax;
using JiteLang.Utilities;
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
            NotifyAddList<BoundParameterDeclaration> @params
            ) : base(parent)
        {
            Body = body;
            Identifier = identifierExpression;
            Params = @params;
            ReturnType = returnType;
        }

        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }

        protected void OnAdd(BoundParameterDeclaration item)
        {
            item.Parent = Body;
        }

        public NotifyAddList<BoundParameterDeclaration> Params
        {
            get;
            set
            {
                field?.OnAdd -= OnAdd;
                field = value;

                if (field is not null)
                {
                    field.OnAdd += OnAdd;

                    foreach (var member in field)
                    {
                        OnAdd(member);
                    }
                }
            }
        }

        public BoundBlockStatement<BoundNode, TypeLocal> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;

                if (Params is not null)
                {
                    foreach (var member in Params)
                    {
                        OnAdd(member);
                    }
                }
            }
        }

        public override BoundIdentifierExpression Identifier
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

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