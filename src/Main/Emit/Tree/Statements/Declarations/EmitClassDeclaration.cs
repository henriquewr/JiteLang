using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Shared.Type;
using System.Linq;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitClassDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.ClassDeclaration;

        public EmitClassDeclaration(EmitNode parent, TypeSymbol type, string name) : base(parent, name)
        {
            Body = new(this);
            Type = type;
        }

        public EmitMethodDeclaration Initializer
        {
            get
            {
                return Body.Members.OfType<EmitMethodDeclaration>().First(x => x.IsInitializer);
            }
        }

        public EmitBlockStatement<EmitNode, CodeField> Body { get; set; }
        public TypeSymbol Type { get; set; }

        public string GetFullName(char separator = '.')
        {
            var parentNamespace = (EmitNamespaceDeclaration)Parent.Parent!;
            return $"{parentNamespace.Name}{separator}{Name}";
        }
    }
}