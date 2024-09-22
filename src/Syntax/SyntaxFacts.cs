
namespace JiteLang.Syntax
{
    internal static class SyntaxFacts
    {
        public const string Public = "public";
        public const string Private = "private";

        public const string Int = "int";
        public const string String = "string";
        public const string Bool = "bool";
        public const string Long = "long";
        public const string Char = "char";

        public const string False = "false";
        public const string True = "true";

        public const string Void = "void";

        public const string Null = "null";

        public const string Namespace = "namespace";
        public const string Class = "class";
        public const string Return = "return";
        public const string If = "if";
        public const string Else = "else";
        public const string While = "while";


        public const int LiteralsMinValue = 2000;
        public const int LiteralsMaxValue = 3000;

        public const int KeywordMinValue = 3000;
        public const int KeywordMaxValue = 4000;

        public static bool IsKeyword(SyntaxKind kind)
        {
            var kindAsInt = (int)kind;
            var isKeyword = kindAsInt > KeywordMinValue && kindAsInt < KeywordMaxValue;
            return isKeyword;
        }

        public static bool IsLiteral(SyntaxKind kind)
        {
            var kindAsInt = (int)kind;
            var isLiteral = kindAsInt > LiteralsMinValue && kindAsInt < LiteralsMaxValue;
            return isLiteral;
        }

        public static bool IsAccessModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumeric(SyntaxKind keyword)
        {
            switch (keyword)
            {
                case SyntaxKind.IntKeyword:
                case SyntaxKind.LongKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPredefinedType(SyntaxKind keyword)
        {
            switch (keyword)
            {
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.CharKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.VoidKeyword:
                    return true;
                default:
                    return false;
            }
        }

        public static SyntaxKind GetKeyword(string text)
        {
            switch (text)
            {
                case Bool:
                    return SyntaxKind.BoolKeyword;
                case False:
                    return SyntaxKind.FalseKeyword;
                case True:
                    return SyntaxKind.TrueKeyword;
                case Int:
                    return SyntaxKind.IntKeyword;
                case Long:
                    return SyntaxKind.LongKeyword;
                case Char:
                    return SyntaxKind.CharKeyword;
                case String:
                    return SyntaxKind.StringKeyword;
                case Void:
                    return SyntaxKind.VoidKeyword;

                case Namespace:
                    return SyntaxKind.NamespaceKeyword;
                case Class:
                    return SyntaxKind.ClassKeyword;
                case If:
                    return SyntaxKind.IfKeyword;
                case Else:
                    return SyntaxKind.ElseKeyword;
                case While:
                    return SyntaxKind.WhileKeyword;

                case Public:
                    return SyntaxKind.PublicKeyword;
                case Private:
                    return SyntaxKind.PrivateKeyword;

                case Return:
                    return SyntaxKind.ReturnKeyword;

                case Null:
                    return SyntaxKind.NullKeyword;

                default:
                    return SyntaxKind.None;
            }
        }

        public static SyntaxKind GetKeywordFromLiteral(SyntaxKind literalKind)
        {
            switch (literalKind)
            {
                case SyntaxKind.FalseLiteralToken:
                    return SyntaxKind.FalseKeyword;
                case SyntaxKind.TrueLiteralToken:
                    return SyntaxKind.TrueKeyword;

                case SyntaxKind.IntLiteralToken:
                    return SyntaxKind.IntKeyword;
                case SyntaxKind.LongLiteralToken:
                    return SyntaxKind.LongKeyword;

                case SyntaxKind.CharLiteralToken:
                    return SyntaxKind.CharKeyword;

                case SyntaxKind.StringLiteralToken:
                    return SyntaxKind.StringKeyword;

                default:
                    return SyntaxKind.None;
            }
        }

        public static string? GetText(SyntaxKind keyword)
        {
            switch (keyword)
            {
                case SyntaxKind.BoolKeyword:
                    return Bool;
                case SyntaxKind.FalseKeyword:
                    return False;
                case SyntaxKind.TrueKeyword:
                    return True;
                case SyntaxKind.IntKeyword:
                    return Int;
                case SyntaxKind.LongKeyword:
                    return Long;
                case SyntaxKind.CharKeyword:
                    return Char;
                case SyntaxKind.StringKeyword:
                    return String;
                case SyntaxKind.VoidKeyword:
                    return Void;

                case SyntaxKind.PublicKeyword:
                    return Public;
                case SyntaxKind.PrivateKeyword:
                    return Private;

                case SyntaxKind.NamespaceKeyword:
                    return Namespace;
                case SyntaxKind.ClassKeyword:
                    return Class;    
                case SyntaxKind.IfKeyword:
                    return If;

                case SyntaxKind.ReturnKeyword:
                    return Return;

                case SyntaxKind.NullKeyword:
                    return Null;

                default:
                    return null;
            }
        }
    }
}
