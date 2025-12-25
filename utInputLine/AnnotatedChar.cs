
/*
    NestingLevel - an object containing one character and the depth it is
                   nested in by parens, square brackets and quotes
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public readonly struct AnnotatedChar
    {
        // private members
        private readonly char character;
        private readonly int  quotelevel;  // can only be 0 or 1, no nested quotes allowed
        private readonly int  bracketlevel;
        private readonly int  parenlevel;

        // public access properties
        public char Character 
        { 
            get 
            { 
                switch (character)
                {
                    case openquote:  return quote;
                    case closequote: return quote;
                    default:         return character;
                }
            } 
        }

        public int QuoteLevel   {get {return quotelevel;}} 
        public int BracketLevel {get {return bracketlevel;}} 
        public int ParenLevel   {get {return parenlevel;}} 

        //**************************************************************************

        public override string ToString ()
        {
            string str = "char: " + Character;
                   str += "\t quotelevel: " + quotelevel;
                   str += "\t bracketlevel: " + bracketlevel;
                   str += "\t parenlevel: " + parenlevel;
            return str;
        }

        //**************************************************************************
        //
        // use this ctor for the first character of a string
        //
        public AnnotatedChar (char c)
        {
            character = c;
            quotelevel   = character == quote ? 1 : 0;
            bracketlevel = character == openbracket ? 1 : 0;
            parenlevel   = character == openparen ? 1 : 0;

            if (character == closeparen) throw new Exception ("NestingLevel: paren nesting error");
            if (character == closebracket) throw new Exception ("NestingLevel: bracket nesting error");
        }

        //**************************************************************************
        //
        // use this ctor for subsequent characters
        //
        public AnnotatedChar (AnnotatedChar prev, char ch)
        {
            character = ch;

            // start by setting each level equal to previous character, then
            // adjust as necessary

            parenlevel = prev.parenlevel;
            bracketlevel = prev.bracketlevel;
            quotelevel = prev.quotelevel;

            // adjust parenthesis level
            if      (character == openparen)  parenlevel++;
            if (prev.character == closeparen) parenlevel--;

            // adjust bracket level
            if      (character == openbracket)  bracketlevel++;
            if (prev.character == closebracket) bracketlevel--;

            // adjust quote level. can only be 0 or 1
            if (prev.character != esc && character == quote)
            { 
                if      (quotelevel == 0)  {quotelevel = 1; character = openquote;}
                else if (quotelevel == 1)  character = closequote;
            }

            if (prev.character == closequote) quotelevel--;

            // check for errors
            if (bracketlevel < 0) throw new Exception ("nestinglevel: bracket nesting error");
            if (parenlevel   < 0) throw new Exception ("nestinglevel: paren nesting error");
        }

        static public readonly char esc   = '\\';        

        static public readonly char quote = '\'';        
        private const char  openquote     = (char) 145; // extended ASCII left single quote
        private const char closequote     = (char) 146; //   "        "   right   "     "

        static public readonly char openbracket  = '[';
        static public readonly char closebracket = ']';
        
        static public readonly char openparen  = '(';
        static public readonly char closeparen = ')';

        public int Quotelevel {get {return quotelevel;}}
        public int Level      {get {return quotelevel + bracketlevel + parenlevel;}}
    }
}
