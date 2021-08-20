using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLKernel;

namespace FrontEnd
{
    public static partial class Utils
    {
        public static readonly string Prompt = "--> ";
        public static readonly string LineContinued = "...";
        public static readonly char   Semicolon = ';';

        //*************************************************************************************************

        public struct NestingLevel
        {
            public NestingLevel (char c)
            {
                character = c;
                quoteLevel   = character == Quote ? 1 : 0;
                bracketLevel = character == OpenBracket ? 1 : 0;
                parenLevel   = character == OpenParen ? 1 : 0;

                if (character == CloseParen)   throw new Exception ("NestingLevel: paren nesting error");
                if (character == CloseBracket) throw new Exception ("NestingLevel: bracket nesting error");
            }

            public NestingLevel (NestingLevel prev, char ch)
            {
                character = ch;

                if (character == Quote)
                {
                    if (prev.character == Esc) quoteLevel = prev.quoteLevel;
                    else if (prev.quoteLevel == 1) quoteLevel = 0;
                    else if (TokenParsing.CanPreceedString.Contains (prev.character)) quoteLevel = 1;
                    else quoteLevel = prev.quoteLevel;
                }
                else
                    quoteLevel = prev.quoteLevel;


                if      (character == OpenBracket)  bracketLevel = prev.bracketLevel + 1;
                else if (character == CloseBracket) bracketLevel = prev.bracketLevel - 1;
                else                                bracketLevel = prev.bracketLevel;

                if      (character == OpenParen)  parenLevel = prev.parenLevel + 1;
                else if (character == CloseParen) parenLevel = prev.parenLevel - 1;
                else                              parenLevel = prev.parenLevel;

                if (bracketLevel < 0) throw new Exception ("NestingLevel: bracket nesting error");
                if (parenLevel < 0)   throw new Exception ("NestingLevel: paren nesting error");
            }

            static public readonly char Esc = '\\';
            static public readonly char Quote = '\'';
            static public readonly char OpenBracket = '[';
            static public readonly char CloseBracket = ']';
            static public readonly char OpenParen = '(';
            static public readonly char CloseParen = ')';

            public readonly char character;
            public readonly int quoteLevel;  // can only be 0 or 1, no nested quotes allowed
            public readonly int bracketLevel;
            public readonly int parenLevel;

            public int QuoteLevel {get {return quoteLevel;}}
            public int Level {get {return quoteLevel + bracketLevel + parenLevel;}}
        }
    }
}
