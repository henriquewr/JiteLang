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

    internal class PredefinedTypeSymbol : MemberedTypeSymbol
    {
        private PredefinedTypeSymbol(string text, bool isReferenceType, PredefinedTypeKind predefinedTypeKind) : base(text, text, isReferenceType, new(0), new(0))
        {
            Kind = predefinedTypeKind;
        }

        public readonly PredefinedTypeKind Kind;

        public static readonly PredefinedTypeSymbol Void = new(SyntaxFacts.Void, false, PredefinedTypeKind.Void);

        public static readonly StructTypeSymbol Int = new(SyntaxFacts.Int, SyntaxFacts.Int, new(0), new(0));
        public static readonly StructTypeSymbol Char = new(SyntaxFacts.Char, SyntaxFacts.Char, new(0), new(0));
        public static readonly StructTypeSymbol Long = new(SyntaxFacts.Long, SyntaxFacts.Long, new(0), new(0));
        public static readonly StructTypeSymbol Bool = new(SyntaxFacts.Bool, SyntaxFacts.Bool, new(0), new(0));

        public static readonly ClassTypeSymbol String = new(SyntaxFacts.String, SyntaxFacts.String, new(0), new(0));
        public static readonly ClassTypeSymbol Object = new(SyntaxFacts.Object, SyntaxFacts.Object, new(0), new(0));

        public static MemberedTypeSymbol? FromText(string text)
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
