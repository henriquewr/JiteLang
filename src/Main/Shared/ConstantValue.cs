using JiteLang.Main.Shared.Type;
using JiteLang.Syntax;
using System.Diagnostics;

namespace JiteLang.Main.Shared
{
    internal enum ConstantValueKind
    {
        String,
        Char,
        Int,
        Bool,
        Long,
        Null,
    }


    [DebuggerDisplay("{Text}")]
    internal class ConstantValue
    {
        private ConstantValue(ConstantValueKind kind, SyntaxPosition position, string text, bool dummy) // the string constructor is equals to this, it needs a dummy
        {
            Kind = kind;
            Position = position;
            Text = text;
        }

        public ConstantValue(SyntaxPosition position, int value) : this(ConstantValueKind.Int, position, value.ToString(), false)
        {
            IntValue = value;
        }

        public ConstantValue(SyntaxPosition position, string value) : this(ConstantValueKind.String, position, value, false)
        {
            StringValue = value;
        }

        public ConstantValue(SyntaxPosition position, bool value) : this(ConstantValueKind.Bool, position, value.ToString(), false)
        {
            BoolValue = value;
        }   

        public ConstantValue(SyntaxPosition position, long value) : this(ConstantValueKind.Long, position, value.ToString(), false)
        {
            LongValue = value;
        }

        public ConstantValue(SyntaxPosition position, char value) : this(ConstantValueKind.Char, position, value.ToString(), false)
        {
            CharValue = value;
        }

        public ConstantValue(SyntaxPosition position) : this(ConstantValueKind.Null, position, SyntaxFacts.Null, false)
        {
        }

        public ConstantValueKind Kind { get; set; }

        public string Text { get; set; }

        public string? StringValue { get; set; }
        public char CharValue { get; set; }
        public int IntValue { get; set; }
        public bool BoolValue { get; set; }
        public uint UintValue { get; set; }
        public long LongValue { get; set; }
        public ulong UlongValue { get; set; }
        public float FloatValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }

        public SyntaxPosition Position { get; set; }

        public MemberedTypeSymbol Type => GetTypeFromConstantValue(Kind);

        public static MemberedTypeSymbol GetTypeFromConstantValue(ConstantValueKind constantValueKind)
        {
            var type = constantValueKind switch
            {
                ConstantValueKind.String => (MemberedTypeSymbol)PredefinedTypeSymbol.String,
                ConstantValueKind.Char => PredefinedTypeSymbol.Char,
                ConstantValueKind.Int => PredefinedTypeSymbol.Int,
                ConstantValueKind.Bool => PredefinedTypeSymbol.Bool,
                ConstantValueKind.Long => PredefinedTypeSymbol.Long,
                ConstantValueKind.Null => PredefinedTypeSymbol.Object,
                _ => throw new UnreachableException(),
            };

            return type;
        }
    }
}
