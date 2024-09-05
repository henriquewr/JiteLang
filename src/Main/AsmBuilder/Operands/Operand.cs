using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Builder.Operands
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
    }
}
