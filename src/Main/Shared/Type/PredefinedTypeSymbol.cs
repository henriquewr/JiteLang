using JiteLang.Syntax;

namespace JiteLang.Main.Shared.Type
{
    internal enum PredefinedTypeKind
    {
        Int,
        Void,
        Char,
        Long,
        String,
        Bool,
        Object,
    }

    internal class PredefinedTypeSymbol : TypeSymbol
    {
        private PredefinedTypeSymbol(string text, bool isReferenceType, PredefinedTypeKind predefinedTypeKind) : base(text, isReferenceType, null)
        {
            Kind = predefinedTypeKind;
        }

        public readonly PredefinedTypeKind Kind;
        public static readonly PredefinedTypeSymbol Int = new(SyntaxFacts.Int, false, PredefinedTypeKind.Int);
        public static readonly PredefinedTypeSymbol Void = new(SyntaxFacts.Void, false, PredefinedTypeKind.Void);
        public static readonly PredefinedTypeSymbol Char = new(SyntaxFacts.Char, false, PredefinedTypeKind.Char);
        public static readonly PredefinedTypeSymbol Long = new(SyntaxFacts.Long, false, PredefinedTypeKind.Long);
        public static readonly PredefinedTypeSymbol Bool = new(SyntaxFacts.Bool, false, PredefinedTypeKind.Bool);

        public static readonly PredefinedTypeSymbol String = new(SyntaxFacts.String, true, PredefinedTypeKind.String);
        public static readonly PredefinedTypeSymbol Object = new(SyntaxFacts.Object, true, PredefinedTypeKind.Object);

        public static PredefinedTypeSymbol? FromText(string text)
        {
            switch (text)
            {
                case SyntaxFacts.Int:
                    return Int;

                case SyntaxFacts.String:
                    return String;

                case SyntaxFacts.Bool:
                    return Bool;

                case SyntaxFacts.Long:
                    return Long;

                case SyntaxFacts.Char:
                    return Char;

                case SyntaxFacts.Void:
                    return Void;

                case SyntaxFacts.Object:
                    return Object;

                default:
                    return null;
            }
        }
    }
}
