using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Built.Expressions;

namespace JiteLang.Main.Built.Statements.Declaration
{
    internal class BuiltNamespaceDeclaration : BuiltDeclaration
    {
        public override BuiltKind Kind => BuiltKind.NamespaceDeclaration;

        public BuiltNamespaceDeclaration(BuiltIdentifierExpression identifier) : base(identifier)
        {

        }

        public BuiltBlockStatement Body { get; set; } = new BuiltBlockStatement();
    }
}
