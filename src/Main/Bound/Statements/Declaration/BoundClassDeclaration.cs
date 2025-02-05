using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using System.Collections.Generic;
using System.Linq;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundClassDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ClassDeclaration;

        public BoundClassDeclaration(BoundNode parent, TypeSymbol type, BoundIdentifierExpression identifier, BoundBlockStatement<BoundNode> body) : base(parent, identifier)
        {
            Body = body;
            Constructors = new();
            Type = type;

            AddInitializer();
        }

        public BoundClassDeclaration(BoundNode parent, TypeSymbol type, BoundIdentifierExpression identifier) : base(parent, identifier)
        {
            Body = new(this);
            Constructors = new();
            Type = type;

            AddInitializer();
        }
        protected void AddInitializer()
        {
            BoundMethodDeclaration initializer = new(Body, new BoundIdentifierExpression(null!, $"init_{GetFullName('_')}", default), Type, new());
            initializer.Identifier.Parent = initializer;
            initializer.IsInitializer = true;
            initializer.Modifiers = Modifier.Static;

            BoundParameterDeclaration builtParameter = new(
                initializer.Body,
                new(null!, "this", default),
                Type
            );
            builtParameter.Identifier.Parent = builtParameter;
            initializer.Params.Add(builtParameter);

            var returnStmt = new BoundReturnStatement(initializer.Body);

            returnStmt.ReturnValue = new BoundIdentifierExpression(returnStmt, builtParameter.Identifier.Text, default);

            initializer.Body.Members.Add(returnStmt);

            Body.Members.Add(initializer);
        }

        public TypeSymbol Type { get; set; }

        public BoundMethodDeclaration Initializer => Body.Members.OfType<BoundMethodDeclaration>().First(x => x.IsInitializer);

        public List<BoundMethodDeclaration> Constructors { get; set; }

        public BoundBlockStatement<BoundNode> Body { get; set; }

        public string GetFullName(char separator = '.')
        {
            var parentNamespace = (BoundNamespaceDeclaration)Parent.Parent!;
            return $"{parentNamespace.Identifier.Text}{separator}{Identifier.Text}";
        }
    }
}
