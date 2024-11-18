using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;

namespace JiteLang.Main.Bound
{
    internal enum BoundKind
    {
        None = 0,

        EqualsToken,


        BlockStatement,
        ReturnStatement,
        IfStatement,
        ElseStatement,
        WhileStatement,
        LabelStatement,


        NamespaceDeclaration,
        ClassDeclaration,
        MethodDeclaration,
        ParameterDeclaration,
        VariableDeclaration,
        

        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,
        IdentifierExpression,
        CallExpression,
        LogicalExpression,
        UnaryExpression,
        MemberExpression,
        CastExpression,
    }
}
