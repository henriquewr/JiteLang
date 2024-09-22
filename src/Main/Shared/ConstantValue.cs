﻿using JiteLang.Main.Bound;
using JiteLang.Syntax;
using System.Diagnostics;
using System.Security.AccessControl;

namespace JiteLang.Main.Shared
{
    internal enum ConstantValueKind
    {
        String,
        Char,
        Int,
        Bool,
        Long,
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

        public ConstantValue(ConstantValueKind kind, SyntaxPosition position, int value) : this(kind, position, value.ToString(), false)
        {
            IntValue = value;
        }

        public ConstantValue(ConstantValueKind kind, SyntaxPosition position, string value) : this(kind, position, value, false)
        {
            StringValue = value;
        }

        public ConstantValue(ConstantValueKind kind, SyntaxPosition position, bool value) : this(kind, position, value.ToString(), false)
        {
            BoolValue = value;
        }   

        public ConstantValue(ConstantValueKind kind, SyntaxPosition position, long value) : this(kind, position, value.ToString(), false)
        {
            LongValue = value;
        }

        public ConstantValue(ConstantValueKind kind, SyntaxPosition position, char value) : this(kind, position, value.ToString(), false)
        {
            CharValue = value;
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

        public PredefinedTypeSymbol Type
        {
            get
            {
                var type = Kind switch
                {
                    ConstantValueKind.String => PredefinedTypeSymbol.String,
                    ConstantValueKind.Char => PredefinedTypeSymbol.Char,
                    ConstantValueKind.Int => PredefinedTypeSymbol.Int,
                    ConstantValueKind.Bool => PredefinedTypeSymbol.Bool,
                    ConstantValueKind.Long => PredefinedTypeSymbol.Long,
                    _ => throw new UnreachableException(),
                };

                return type;
            }
        }
    }
}
