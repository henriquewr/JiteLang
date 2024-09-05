
namespace JiteLang.Syntax
{
    internal struct SyntaxPosition
    {
        public SyntaxPosition(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public SyntaxPosition(int line, int column, int length)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; set; }
        public int Column { get; set; }
        public int Length { get; set; }

        public readonly string GetPosText()
        {
            var text = $"Line: {Line}, Column: {Column}";
            return text;
        }
    }
}
