using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Built.Expressions;

namespace JiteLang.Main.Built.Statements.Declaration
{
    internal class BuiltClassDeclaration : BuiltDeclaration
    {
        public override BuiltKind Kind => BuiltKind.ClassDeclaration;
        public BuiltClassDeclaration(BuiltIdentifierExpression identifier) : base(identifier)
        {
        }

        public BuiltBlockStatement Body { get; set; } = new BuiltBlockStatement();
    }
}
