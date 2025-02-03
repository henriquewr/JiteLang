using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.Shared.Type.Members;
using JiteLang.Main.Shared.Type.Members.Method;
using System.Collections.Generic;

namespace JiteLang.Main.Shared.Type
{
    internal class ClassTypeSymbol : MemberedTypeSymbol
    {
        public ClassTypeSymbol(string fullText, string text, List<FieldSymbol> fields, List<MethodSymbol> methods, ClassTypeSymbol? subClass = null) : base(fullText, text, true, fields, methods)
        {
            SubClass = subClass ?? PredefinedTypeSymbol.Object;
        }

        public ClassTypeSymbol SubClass { get; init; }

        public bool IsSubClassOf(ClassTypeSymbol typeSymbol)
        {
            var currentSubClass = SubClass;

            while (currentSubClass != null)
            {
                if (currentSubClass.Equals(typeSymbol))
                {
                    return true;
                }

                currentSubClass = currentSubClass?.SubClass;
            }

            return false;
        }
    }
}
