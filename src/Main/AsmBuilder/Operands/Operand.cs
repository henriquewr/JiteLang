using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.AsmBuilder.Operands
{
    internal struct Operand
    {
        public Operand(string value)
        {
            Value = value;
        }

        public Operand(char value)
        {
            Value = value.ToString();
        }

        public Operand(int value)
        {
            Value = value.ToString();
        }

        public string Value { get; set; }

        public static Operand Rax => new Operand("rax");
        public static Operand Rbx => new Operand("rbx");
        public static Operand Rcx => new Operand("rcx");
        public static Operand Rdx => new Operand("rdx");

        public static Operand Rdi => new Operand("rdi");
        public static Operand Rsi => new Operand("rsi");
        public static Operand Rbp => new Operand("rbp");
        public static Operand Rsp => new Operand("rsp");

        public static Operand R8 => new Operand("r8");
        public static Operand R9 => new Operand("r9");
        public static Operand R10 => new Operand("r10");
    }
}
