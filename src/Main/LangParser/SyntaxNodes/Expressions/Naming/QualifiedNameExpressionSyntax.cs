using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming
{
    internal class QualifiedNameExpressionSyntax : NameExpressionSyntax
    {
        public override NameKind Kind => NameKind.QualifiedName;

        public QualifiedNameExpressionSyntax(NameExpressionSyntax left, SimpleNameExpressionSyntax right) : base()
        {
            Left = left;
            Right = right;
        }

        public override SyntaxPosition Position { get => Left.Position; set => Left.Position = value; }

        public NameExpressionSyntax Left { get; set; }

        public SimpleNameExpressionSyntax Right { get; set; }

        public string GetFullName()
        {
            var leftName = Left is QualifiedNameExpressionSyntax leftQualified ? leftQualified.GetFullName() : ((SimpleNameExpressionSyntax)Left).Text;

            return $"{leftName}.{Right.Text}";
        }
    }
}
