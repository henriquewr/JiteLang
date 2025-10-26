using JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ClassDeclarationSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public ClassDeclarationSyntax(SimpleNameExpressionSyntax nameExpression, BlockStatement<SyntaxNode> body) : base()
        {
            NameExpression = nameExpression;
            Body = body;
        }

        public BlockStatement<SyntaxNode> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public SimpleNameExpressionSyntax NameExpression { get; set; }

        public string GetFullName()
        {
            var parentNamespace = (NamespaceDeclarationSyntax)Parent.Parent!;
            return $"{parentNamespace.NameExpression.Text}.{((SimpleNameExpressionSyntax)NameExpression).Text}";
        }
    }
}