using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Built.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;

namespace JiteLang.Main.Built.Statements.Declaration
{
    internal abstract class BuiltDeclaration : BuiltStatement
    {
        public BuiltDeclaration(BuiltIdentifierExpression identifier)
        {
            Identifier = identifier;
        }
        public BuiltIdentifierExpression Identifier { get; init; }
    }
}
