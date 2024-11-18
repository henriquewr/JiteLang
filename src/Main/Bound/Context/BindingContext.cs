using JiteLang.Main.Bound.Context.Conversions;

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