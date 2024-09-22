using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal abstract class BoundDeclaration : BoundStatement
    {
        public BoundDeclaration(BoundIdentifierExpression identifier)
        {
            Identifier = identifier; 
            Position = identifier.Position;
        }

        public BoundIdentifierExpression Identifier { get; init; }
    }
}
