using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundClassDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ClassDeclaration;

        public BoundClassDeclaration(BoundNode? parent, ClassTypeSymbol type, BoundIdentifierExpression identifier, BoundBlockStatement<BoundNode, TypeField> body) : base(parent)
        {
            Body = body;
            Identifier = identifier;
            //Constructors = new();
            Type = type;
        }

        public void AddInitializer()
        {
            var initializerName = $"init_{GetFullName('_')}";

            var delegateTypeSymbol = DelegateTypeSymbol.Generate(initializerName, Type, Type);

            BoundMethodDeclaration initializer = new
            (
                Body,
                new BoundIdentifierExpression(null, $"init_{GetFullName('_')}", delegateTypeSymbol),
                Type,
                null!,
                new NotifyAddList<BoundParameterDeclaration>(1)
                {
                    new BoundParameterDeclaration
                    (
                        null,
                        new(null,"this", Type),
                        Type
                    )
                }
            )
            {
                IsInitializer = true,
                Modifiers = Modifier.Static
            };

            initializer.Body = new BoundBlockStatement<BoundNode, TypeLocal>(initializer, new(1));
            var returnStmt = new BoundReturnStatement(initializer.Body,
                new BoundIdentifierExpression(null, "this", Type)
            );

            initializer.Body.Members.Add(returnStmt);

            Body.Members.Add(initializer);
        }

        public ClassTypeSymbol Type { get; set; }

        public BoundMethodDeclaration Initializer => Body.Members.OfType<BoundMethodDeclaration>().First(x => x.IsInitializer);

        //public List<BoundMethodDeclaration> Constructors { get; set; }

        public BoundBlockStatement<BoundNode, TypeField> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
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


        public string GetFullName(char separator = '.')
        {
            var parentNamespace = (BoundNamespaceDeclaration)Parent.Parent!;
            return $"{parentNamespace.Identifier.Text}{separator}{Identifier.Text}";
        }
    }
}