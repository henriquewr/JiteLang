using System.Collections.Generic;
using JiteLang.Main.Builder.Instructions;

namespace JiteLang.Main.Built.Statements
{
    internal class BuiltBlockStatement : BuiltStatement
    {
        public override BuiltKind Kind => BuiltKind.BlockStatement;

        public BuiltBlockStatement(IList<BuiltNode> members)
        {
            Members = members;
        }

        public BuiltBlockStatement()
        {
            Members = new List<BuiltNode>();
        }

        public IList<BuiltNode> Members { get; set; }
    }
}
