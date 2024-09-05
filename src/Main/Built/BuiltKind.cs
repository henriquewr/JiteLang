using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;

namespace JiteLang.Main.Built
{
    internal enum BuiltKind
    {
        None = 0,

        BlockStatement,


        NamespaceDeclaration,
        ClassDeclaration,
        MethodDeclaration,
        VariableDeclaration,
        

        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,
        IdentifierExpression,
    }
}
