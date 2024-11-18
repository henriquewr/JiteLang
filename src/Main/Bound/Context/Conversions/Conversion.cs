using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Context.Conversions
{
    internal class Conversion
    {
        public Conversion(ConversionType type)
        {
            Type = type;
        }        

        public ConversionType Type { get; set; }
    }
}