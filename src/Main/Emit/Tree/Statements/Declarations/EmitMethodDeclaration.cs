using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitMethodDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.MethodDeclaration;
        public EmitMethodDeclaration(EmitNode parent, EmitLabelStatement label, EmitLabelStatement labelExit, EmitBlockStatement<EmitNode> body) : base(parent)
        {
            Label = label;
            LabelExit = labelExit;
            Body = body;
            Params = new();
        }

        public EmitMethodDeclaration(EmitNode parent, EmitLabelStatement label) : base(parent)
        {
            Label = label;
            LabelExit = new(parent, $"exit_{label.Name}");
            Params = new();
            Body = new(this);
        }

        public EmitMethodDeclaration(EmitNode parent, string name) : base(parent)
        {
            Label = new(this, name);
            LabelExit = EmitLabelStatement.Create(this, $"exit_{name}");
            Params = new();
            Body = new(this);
        }

        public const int UpperStackInitialPos = 8; //skip return address

        public EmitLabelStatement Label { get; set; }
        public EmitLabelStatement LabelExit { get; set; }

        public int StackAllocatedBytes { get; set; }
        public int UpperStackPosition { get; set; } = UpperStackInitialPos;
        public EmitBlockStatement<EmitNode> Body { get; set; }
        public List<EmitParameterDeclaration> Params { get; set; }
    }
}
