//using System;
//using System.Diagnostics;
//using System.Threading;

//namespace JiteLang.Main.Bound.Statements
//{
//    [DebuggerDisplay($"{nameof(GetDebuggerDisplay)}()")]
//    internal class BoundLabelStatement : BoundStatement
//    {
//        public override BoundKind Kind => BoundKind.LabelStatement;

//        public BoundLabelStatement(BoundNode parent, string name) : base(parent)
//        {
//            Name = name;
//        }

//        public string Name { get; set; }

//        protected virtual string GetDebuggerDisplay()
//        {
//            return Name;
//        }
//    }

//    internal static class BoundLabelFactory
//    {
//        private static int s_labelCount = 0;

//        public static BoundLabelStatement ElseLabel(BoundNode parent)
//        {
//            var label = CreateLabel(parent, "else");
//            return label;
//        }

//        public static BoundLabelStatement IfLabel(BoundNode parent)
//        {
//            var label = CreateLabel(parent, "if");
//            return label;
//        }

//        public static BoundLabelStatement StartLabel(BoundNode parent)
//        {
//            var label = CreateLabel(parent, "start");
//            return label;
//        }

//        public static BoundLabelStatement StartLabel(BoundNode parent, in ReadOnlySpan<char> name)
//        {
//            var label = CreateLabel(parent, $"start_{name}");
//            return label;
//        }

//        public static BoundLabelStatement ExitLabel(BoundNode parent)
//        {
//            var label = CreateLabel(parent, "exit");
//            return label;
//        }

//        public static BoundLabelStatement ExitLabel(BoundNode parent, in ReadOnlySpan<char> name)
//        {
//            var label = CreateLabel(parent, $"exit_{name}");
//            return label;
//        }

//        public static BoundLabelStatement ReturnLabel(BoundNode parent, in ReadOnlySpan<char> name)
//        {
//            var label = CreateLabel(parent, $"return_{name}");
//            return label;
//        }

//        public static BoundLabelStatement ExitMethod(BoundNode parent, in ReadOnlySpan<char> methodName)
//        {
//            var label = CreateLabel(parent, $"exit_method_{methodName}");
//            return label;
//        }

//        private static BoundLabelStatement CreateLabel(BoundNode parent, in ReadOnlySpan<char> prefix)
//        {
//            var num = Interlocked.Increment(ref s_labelCount);
//            var name = $"{prefix}_{num}";

//            BoundLabelStatement label = new(parent, name);
//            return label;
//        }
//    }
//}
