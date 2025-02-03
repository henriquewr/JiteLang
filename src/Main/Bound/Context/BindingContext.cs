using JiteLang.Main.Bound.Context.Conversions;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Context
{
    internal class BindingContext
    {
        public BindingContext(ConversionTable conversionTable) 
        {
            ConversionTable = conversionTable;
        }

        public ConversionTable ConversionTable { get; set; }
    }
}