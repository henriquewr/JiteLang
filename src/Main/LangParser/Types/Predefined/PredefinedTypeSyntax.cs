using JiteLang.Main.LangParser.SyntaxNodes;

namespace JiteLang.Main.LangParser.Types.Predefined
{
    internal class PredefinedTypeSyntax : TypeSyntax
    {
        public override bool IsPreDefined => true;

        public PredefinedTypeSyntax(SyntaxToken keyword) : base(keyword)
        {
        }
    }
}
