using JiteLang.Main.LangParser.SyntaxNodes;

namespace JiteLang.Main.LangParser.Types
{
    internal class TypeSyntax
    {
        public TypeSyntax(SyntaxToken token)
        {
            Token = token;
        }

        public virtual bool IsPreDefined => false;
        public string Text => Token.Text;
        public SyntaxToken Token { get; set; }
    }
}