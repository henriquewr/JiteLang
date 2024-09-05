using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Built;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Built.Statements.Declaration;
using JiteLang.Main.Built.Statements;
using JiteLang.Main.Built.Expressions;

namespace JiteLang.Main.Builder
{
    internal class BuiltMethodDeclaration : BuiltDeclaration
    {
        public override BuiltKind Kind => BuiltKind.MethodDeclaration;

        public BuiltMethodDeclaration(BuiltIdentifierExpression identifierExpression) : base(identifierExpression)
        {

        }

        public BuiltBlockStatement Body { get; set; } = new BuiltBlockStatement();
    }
}
