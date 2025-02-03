using JiteLang.Main.LangParser.SyntaxNodes;
using System.Collections.Generic;
using System.Xml.Linq;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitFieldDeclaration : EmitVariableDeclaration
    {
        public override EmitKind Kind => EmitKind.FieldDeclaration;

        public EmitFieldDeclaration(EmitNode parent, string name) : this(parent, name, new())
        {
        }

        public EmitFieldDeclaration(EmitNode parent, string name, List<SyntaxToken> modifiers) : base(parent, name)
        {
            Modifiers = modifiers;
        }

        public List<SyntaxToken> Modifiers { get; set; }
    }
}