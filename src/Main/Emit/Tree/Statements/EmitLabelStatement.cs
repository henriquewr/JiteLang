using System.Threading;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitLabelStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.LabelStatement;
        private static ulong _labelCount = 0;
        public EmitLabelStatement(EmitNode parent, string name) : base(parent)
        {
            Name = name;
        }

        public string Name { get; set; }

        public static EmitLabelStatement Create(EmitNode parent, string name)
        {
            name += Interlocked.Increment(ref _labelCount);
            return new EmitLabelStatement(parent, name);
        }
    }
}
