using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions.Naming
{
    internal abstract class NameExpressionSyntax
    {
        public abstract NameKind Kind { get; }

        public virtual SyntaxPosition Position { get; set; }
    }
}
