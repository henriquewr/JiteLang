using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.CodeAnalysis.Types
{
    [DebuggerDisplay("{GetStr()}")]
    internal class LangType
    {
        public LangType(TypeKind typeKind) 
        {
            Kind = typeKind;
        }

        public TypeKind Kind { get; set; }

        public static readonly LangType None = new LangType(TypeKind.None);

        public static readonly LangType Int = new LangType(TypeKind.Int);
        public static readonly LangType Bool = new LangType(TypeKind.Bool);
        public static readonly LangType Long = new LangType(TypeKind.Long);
        public static readonly LangType Char = new LangType(TypeKind.Char);
        public static readonly LangType String = new LangType(TypeKind.String);


        public string GetStr()
        {
            return this.Kind.ToString(); //Enum.ToString is slow, use switch on nameof (maybe use a source generator to make it)
        }

        public bool IsEqualsNotNone(LangType? langType)
        {
            var isEquals = this.Kind == langType?.Kind;

            var isEqualsNotNone = isEquals && this.Kind != TypeKind.None;

            return isEqualsNotNone;
        }
    }
}
