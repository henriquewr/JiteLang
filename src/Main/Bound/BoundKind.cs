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

        ///// <summary><c> ! </c></summary>
        //NotToken,

        ///// <summary><c> = </c></summary>
        EqualsToken,
        ///// <summary><c> == </c></summary>
        //EqualsEqualsToken,

        ///// <summary><c> != </c></summary>
        //NotEqualsToken,

        ///// <summary><c> &gt; </c></summary>
        //GreaterThanToken,

        ///// <summary><c> &gt;= </c>/summary>
        //GreaterThanEqualsToken,

        ///// <summary><c> &lt; </c></summary>
        //LessThanToken,
        ///// <summary><c> &lt;= </c></summary>
        //LessThanEqualsToken,

        ///// <summary><c> + </c></summary>
        //PlusToken,
        ///// <summary><c> += </c></summary>
        //PlusEqualsToken,

        ///// <summary><c> - </c></summary>
        //MinusToken,
        ///// <summary><c> -= </c></summary>
        //MinusEqualsToken,

        ///// <summary><c> * </c></summary>
        //AsteriskToken,
        ///// <summary><c> *= </c></summary>
        //AsteriskEqualsToken,

        ///// <summary><c> / </c></summary>
        //SlashToken,
        ///// <summary><c> /= </c></summary>
        //SlashEqualsToken,

        ///// <summary><c> % </c></summary>
        //PercentToken,
        ///// <summary><c> %= </c></summary>
        //PercentEqualsToken,

        ///// <summary><c> ^ </c></summary>
        //CaretToken,
        ///// <summary><c> ^= </c></summary>
        //CaretEqualsToken,

        ///// <summary><c> & </c></summary>
        //AmpersandToken,
        ///// <summary><c> && </c></summary>
        //AmpersandAmpersandToken,

        ///// <summary><c> | </c></summary>
        //BarToken,
        ///// <summary><c> || </c></summary>
        //BarBarToken,


        BlockStatement,
        ReturnStatement,
        IfStatement,
        WhileStatement,


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
