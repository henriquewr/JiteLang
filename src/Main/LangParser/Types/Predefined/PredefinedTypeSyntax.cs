﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangLexer.Token;
using JiteLang.Main.LangParser.SyntaxNodes;

namespace JiteLang.Main.LangParser.Types.Predefined
{
    internal class PredefinedTypeSyntax : TypeSyntax
    {
        public PredefinedTypeSyntax(SyntaxToken keyword) 
        {
            Keyword = keyword;
        }

        public SyntaxToken Keyword { get; set; }
        public override bool IsPreDefined => true;
    }
}
