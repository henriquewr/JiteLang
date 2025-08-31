using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Syntax
{
    internal enum SyntaxKind
    {
        None = 0,

        /// <summary><c> , </c></summary>
        CommaToken,
        /// <summary><c> . </c></summary>
        DotToken,

        /// <summary><c> ( </c></summary>
        OpenParenToken,
        /// <summary><c> ) </c></summary>
        CloseParenToken,

        /// <summary><c> [ </c></summary>
        OpenBracketToken,
        /// <summary><c> ] </c></summary>
        CloseBracketToken,

        /// <summary><c> { </c></summary>
        OpenBraceToken,
        /// <summary><c> } </c></summary>
        CloseBraceToken,

        /// <summary><c> ; </c></summary>
        SemiColon,

        /// <summary><c> ! </c></summary>
        NotToken,

        /// <summary><c> = </c></summary>
        EqualsToken,
        /// <summary><c> == </c></summary>
        EqualsEqualsToken,

        /// <summary><c> != </c></summary>
        NotEqualsToken,

        /// <summary><c> &gt; </c></summary>
        GreaterThanToken,

        /// <summary><c> &gt;= </c>/summary>
        GreaterThanEqualsToken,

        /// <summary><c> &lt; </c></summary>
        LessThanToken,
        /// <summary><c> &lt;= </c></summary>
        LessThanEqualsToken,

        /// <summary><c> + </c></summary>
        PlusToken,
        /// <summary><c> += </c></summary>
        PlusEqualsToken,

        /// <summary><c> - </c></summary>
        MinusToken,
        /// <summary><c> -= </c></summary>
        MinusEqualsToken,

        /// <summary><c> * </c></summary>
        AsteriskToken,
        /// <summary><c> *= </c></summary>
        AsteriskEqualsToken,

        /// <summary><c> / </c></summary>
        SlashToken,
        /// <summary><c> /= </c></summary>
        SlashEqualsToken,

        /// <summary><c> % </c></summary>
        PercentToken,
        /// <summary><c> %= </c></summary>
        PercentEqualsToken,

        /// <summary><c> & </c></summary>
        AmpersandToken,
        /// <summary><c> && </c></summary>
        AmpersandAmpersandToken,

        /// <summary><c> | </c></summary>
        BarToken,
        /// <summary><c> || </c></summary>
        BarBarToken,

        /// <summary><c> ^ </c></summary>
        CaretToken,
        

        /// <summary><c> </c></summary>
        IdentifierToken,


        //Literals: 2000 -> 3000
        /// <summary><c> </c></summary>
        StringLiteralToken = 2001,
        /// <summary><c> </c></summary>
        CharLiteralToken = 2002,
        /// <summary><c> </c></summary>
        IntLiteralToken = 2003,
        /// <summary><c> </c></summary>
        LongLiteralToken = 2004,
        /// <summary><c> </c></summary>
        FalseLiteralToken = 2005,
        /// <summary><c> </c></summary>
        TrueLiteralToken = 2006,
        /// <summary><c> </c></summary>
        NullLiteralToken = 2007,




        //Keywords: 3000 -> 4000
        /// <summary><c> namespace </c></summary>
        NamespaceKeyword = 3001,
        /// <summary><c> class </c></summary>
        ClassKeyword = 3002,
        /// <summary><c> void </c></summary>
        VoidKeyword = 3003,
        /// <summary><c> bool </c></summary>
        BoolKeyword = 3004,
        /// <summary><c> int </c></summary>
        IntKeyword = 3005,
        /// <summary><c> long </c></summary>
        LongKeyword = 3006,
        /// <summary><c> char </c></summary>
        CharKeyword = 3007,
        /// <summary><c> string </c></summary>
        StringKeyword = 3008,
        /// <summary><c> false </c></summary>
        FalseKeyword = 3009,
        /// <summary><c> true </c></summary>
        TrueKeyword = 3010,
        /// <summary><c> null </c></summary>
        NullKeyword = 3011,
        /// <summary><c> public </c></summary>
        PublicKeyword = 3012,
        /// <summary><c> private </c></summary>
        PrivateKeyword = 3013,
        /// <summary><c> return </c></summary>
        ReturnKeyword = 3014,
        /// <summary><c> if </c></summary>
        IfKeyword = 3015,
        /// <summary><c> else </c></summary>
        ElseKeyword = 3016,
        /// <summary><c> while </c></summary>
        WhileKeyword = 3017,
        /// <summary><c> while </c></summary>
        ForKeyword = 3018,
        /// <summary><c> extern </c></summary>
        ExternKeyword = 3019,
        /// <summary><c> new </c></summary>
        NewKeyword = 3020,        
        /// <summary><c> object </c></summary>
        ObjectKeyword = 3021,
        /// <summary><c> static </c></summary>
        StaticKeyword = 3022,


        //Declarations: 4000 -> 5000
        /// <summary><c> </c></summary>
        NamespaceDeclaration = 4001,
        /// <summary><c> </c></summary>
        ClassDeclaration = 4002,
        /// <summary><c> </c></summary>
        MethodDeclaration = 4003,
        /// <summary><c> </c></summary>
        LocalDeclaration = 4004,
        /// <summary><c> </c></summary>
        ParameterDeclaration = 4005,
        /// <summary><c> </c></summary>
        FieldDeclaration = 4006,


        //Expressions: 5000 -> 6000
        AssignmentExpression = 5001,
        /// <summary><c> </c></summary>
        LiteralExpression = 5002,
        /// <summary><c> </c></summary>
        MemberExpression = 5003,
        /// <summary><c> </c></summary>
        UnaryExpression = 5004,
        /// <summary><c> </c></summary>
        BinaryExpression = 5005,
        /// <summary><c> </c></summary>
        LogicalExpression = 5006,
        /// <summary><c> </c></summary>
        IdentifierExpression = 5007,
        /// <summary><c> </c></summary>
        CastExpression = 5008,        
        /// <summary><c> </c></summary>
        CallExpression = 5009,
        /// <summary><c> </c></summary>
        NewExpression = 5010,

        //Statements: 6000 -> 7000
        /// <summary><c> </c></summary>
        BlockStatement = 6001,
        /// <summary><c> return </c></summary>
        ReturnStatement = 6002,
        /// <summary><c> if </c></summary>
        IfStatement = 6003,
        /// <summary><c> else / else if </c></summary>
        ElseStatement = 6004,
        /// <summary><c> while </c></summary>
        WhileStatement = 6005,
        /// <summary><c> for </c></summary>
        ForStatement = 6006,
    }
}
