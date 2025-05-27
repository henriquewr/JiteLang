using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Visitor.Type.Scope;
using System.Linq;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundClassDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ClassDeclaration;

        public BoundClassDeclaration(BoundNode? parent, ClassTypeSymbol type, BoundIdentifierExpression identifier, BoundBlockStatement<BoundNode, TypeField> body) : base(parent, identifier)
        {
            Body = body;
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
                new(1)
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
            Body.SetParentRecursive();
        }

        public override void SetParent()
        {
            Body.Parent = this;
            Identifier.Parent = this;
        }

        public override void SetParentRecursive()
        {
            SetParent();
            Body.SetParentRecursive();
            Identifier.SetParentRecursive();
        }

        public ClassTypeSymbol Type { get; set; }

        public BoundMethodDeclaration Initializer => Body.Members.OfType<BoundMethodDeclaration>().First(x => x.IsInitializer);

        //public List<BoundMethodDeclaration> Constructors { get; set; }

        public BoundBlockStatement<BoundNode, TypeField> Body { get; set; }

        public string GetFullName(char separator = '.')
        {
            var parentNamespace = (BoundNamespaceDeclaration)Parent.Parent!;
            return $"{parentNamespace.Identifier.Text}{separator}{Identifier.Text}";
        }
    }
}