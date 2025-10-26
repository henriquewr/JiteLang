using JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class NamespaceDeclarationSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NamespaceDeclaration;

        public NamespaceDeclarationSyntax(SimpleNameExpressionSyntax nameExpression, BlockStatement<ClassDeclarationSyntax> body) : base()
        {
            NameExpression = nameExpression;
            Body = body;
        }

        public BlockStatement<ClassDeclarationSyntax> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public SimpleNameExpressionSyntax NameExpression { get; set; }
    }
}
