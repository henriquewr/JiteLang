using JiteLang.Syntax;

namespace JiteLang.Main.Bound
{
    internal enum PredefinedTypeKind
    {
        Int,
        Void,
        Char,
        Long,
        String,
        Bool,
    }

    internal class PredefinedTypeSymbol : TypeSymbol
    {
        private PredefinedTypeSymbol(string text, PredefinedTypeKind predefinedTypeKind) : base(text)
        {
            Kind = predefinedTypeKind;
        }
         
        public readonly PredefinedTypeKind Kind;
        public static readonly PredefinedTypeSymbol Int = new(SyntaxFacts.Int, PredefinedTypeKind.Int);
        public static readonly PredefinedTypeSymbol Void = new(SyntaxFacts.Void, PredefinedTypeKind.Void);
        public static readonly PredefinedTypeSymbol Char = new(SyntaxFacts.Char, PredefinedTypeKind.Char);
        public static readonly PredefinedTypeSymbol Long = new(SyntaxFacts.Long, PredefinedTypeKind.Long);
        public static readonly PredefinedTypeSymbol String = new(SyntaxFacts.String, PredefinedTypeKind.String);
        public static readonly PredefinedTypeSymbol Bool = new(SyntaxFacts.Bool, PredefinedTypeKind.Bool);

        public static PredefinedTypeSymbol? FromText(string text)
        {
            if (text == SyntaxFacts.Int)
            {
                return Int;
            }

            if (text == SyntaxFacts.String)
            {
                return String;
            }

            if (text == SyntaxFacts.Bool)
            {
                return Bool;
            }

            if (text == SyntaxFacts.Long)
            {
                return Long;
            }

            if (text == SyntaxFacts.Char)
            {
                return Char;
            }

            if (text == SyntaxFacts.Void)
            {
                return Void;
            }
            return null;
        }
    }
}
