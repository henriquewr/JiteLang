using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.Shared.Type.Members;
using JiteLang.Main.Shared.Type.Members.Method;
using System.Collections.Generic;

namespace JiteLang.Main.Shared.Type
{
    internal class StructTypeSymbol : MemberedTypeSymbol
    {
        public StructTypeSymbol(string fullText, string text, List<FieldSymbol> fields, List<MethodSymbol> methods) : base(fullText, text, false, fields, methods)
        {
        }
    }
}
