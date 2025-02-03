using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree
{
    internal enum EmitKind
    {
        None = 0,

        BlockStatement,
        ReturnStatement,
        IfStatement,
        ElseStatement,
        WhileStatement,
        LabelStatement,
        JumpStatement,

        NamespaceDeclaration,
        ClassDeclaration,
        MethodDeclaration,
        ParameterDeclaration,
        LocalDeclaration,
        FieldDeclaration,


        AssignmentExpression,
        BinaryExpression,
        LiteralExpression,
        IdentifierExpression,
        CallExpression,
        LogicalExpression,
        UnaryExpression,
        MemberExpression,
        CastExpression,
        NewExpression,
    }
}