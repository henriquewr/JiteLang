using System.Diagnostics;
using JiteLang.Utilities;

namespace JiteLang.Source
{
    [DebuggerDisplay("{GetDebuggerDisplay()}")]
    internal class SourceCode : ControllableArray<char>
    {
        public SourceCode(string text) : base(text.ToCharArray(), char.MaxValue)
        {

        }

        protected override string GetDebuggerDisplay()
        {
            var text = $"Text: {new string(Array)} Current: {Current}";
            return text;
        }
    }
}
