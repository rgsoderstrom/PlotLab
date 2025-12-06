using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public readonly struct NestingLevel
    {
        private readonly char character;

        public char Character 
        { 
            get 
            { 
                switch (character)
                {
                    case openquote: return quote;
                    case closequote: return quote;
                    default: return character;
                }
            } 
        }




        public readonly int quotelevel;  // can only be 0 or 1, no nested quotes allowed
        public readonly int bracketlevel;
        public readonly int parenlevel;

        public override string ToString ()
        {
            string str = "char: " + Character;
                   str += "\t quotelevel: " + quotelevel;
                   str += "\t bracketlevel: " + bracketlevel;
                   str += "\t parenlevel: " + parenlevel;
            return str;
        }

        //
        // use this ctor for the first character of a string
        //
        public NestingLevel (char c)
        {
            character = c;
            quotelevel   = character == quote ? 1 : 0;
            bracketlevel = character == openbracket ? 1 : 0;
            parenlevel   = character == openparen ? 1 : 0;

            if (character == closeparen) throw new Exception ("NestingLevel: paren nesting error");
            if (character == closebracket) throw new Exception ("NestingLevel: bracket nesting error");
        }

        //
        // use this ctor for subsequent characters
        //
        public NestingLevel (NestingLevel prev, char ch)
        {
            character = ch;



            if (prev.Character == esc)
                Console.WriteLine ("aaa");



            quotelevel = prev.quotelevel;

            if (prev.character != esc && character == quote)
            { 
                if      (quotelevel == 0)  {quotelevel = 1; character = openquote;}
                else if (quotelevel == 1)  character = closequote;
            }

            if (prev.character == closequote) quotelevel--;




            bracketlevel = prev.bracketlevel;
            if      (character == openbracket)  bracketlevel++;
            if (prev.character == closebracket) bracketlevel--;


            parenlevel = prev.parenlevel;
            if      (character == openparen)  parenlevel++;
            if (prev.character == closeparen) parenlevel--;

            if (bracketlevel < 0) throw new Exception ("nestinglevel: bracket nesting error");
            if (parenlevel < 0) throw new Exception ("nestinglevel: paren nesting error");
        }

        static public readonly char esc = '\\';
        
        static public readonly char quote = '\'';
        
        private const char  openquote  = (char) 145; // extended ASCII left single quote

        private const char closequote = (char) 146; //   "        "   right   "     "




        static public readonly char openbracket = '[';
        static public readonly char closebracket = ']';
        
        static public readonly char openparen = '(';
        static public readonly char closeparen = ')';

        public int Quotelevel {get {return quotelevel;}}
        public int Level      {get {return quotelevel + bracketlevel + parenlevel;}}
    }
}
