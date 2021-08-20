

//
// COPIED FROM FrontEnd
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd
{
    public static class Utils
    {
        public static readonly string Prompt = "--> ";
        public static readonly string LineContinued = "...";
        public static readonly char   Semicolon = ';';

        //*************************************************************************************************

        public struct NestingLevel
        {
            public NestingLevel (char c, int q, int b, int p)
            {
                character = c;
                quoteLevel = q;
                bracketLevel = b;
                parenLevel = p;
            }

            public NestingLevel (NestingLevel prev, char ch)
            {
                character = ch;

                if (character == Quote)
                {
                    if (prev.character == Esc) quoteLevel = prev.quoteLevel;
                    if (prev.quoteLevel == 1) quoteLevel = 0;
                    else quoteLevel = 1;
                }
                else
                    quoteLevel = prev.quoteLevel;

                if      (character == OpenBracket)  bracketLevel = prev.bracketLevel + 1;
                else if (character == CloseBracket) bracketLevel = prev.bracketLevel - 1;
                else                                bracketLevel = prev.bracketLevel;

                if      (character == OpenParen)  parenLevel = prev.parenLevel + 1;
                else if (character == CloseParen) parenLevel = prev.parenLevel - 1;
                else                              parenLevel = prev.parenLevel;
            }

            static char Esc = '\\';
            static char Quote = '\'';
            static char OpenBracket = '[';
            static char CloseBracket = ']';
            static char OpenParen = '(';
            static char CloseParen = ')';

            public readonly char character;
            public readonly int quoteLevel;  // can only be 0 or 1, no nested quotes allowed
            public readonly int bracketLevel;
            public readonly int parenLevel;
        }

        //*************************************************************************************************

        public static void CleanupRawInput (string       raw,       // as typed in, pasted in or read from text file
                                            List<string> final,     // cleanup-up, separated or concatenated here
                                            ref bool     complete,  // true => no continuation line ready for lineProcessor
                                            ref bool     printFlag) // true => no trailing semicolon, so print answer
        {
        }

        //*************************************************************************************************

    }
}
