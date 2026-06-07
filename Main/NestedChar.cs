
/*
    NestedChar - an object containing one character and the depth it is
                 nested by parens, square brackets and quotes
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    public class NestedChar
    {
        // members
        protected  char  character;
        protected  sbyte bracketlevel;
        protected  sbyte parenlevel;
        protected  sbyte quotelevel;  // can only be 0 or 1, no nested quotes allowed

        // public access properties
        public char Character {get {if (IsOpenQuote || IsCloseQuote) return quote;
                                    else return character;}} 

        public sbyte BracketLevel    {get {return bracketlevel;} set {bracketlevel = value;}} 
        public sbyte ParenLevel      {get {return parenlevel;}   set {parenlevel = value;}} 
        public sbyte QuoteLevel      {get {return quotelevel;}   set {quotelevel = value;}} 

        public bool IsOpenParen    {get {return character == '(';}}
        public bool IsCloseParen   {get {return character == ')';}}
        public bool IsOpenBracket  {get {return character == '[';}}
        public bool IsCloseBracket {get {return character == ']';}}
        public bool IsQuote        {get {return character == '\'';}}
        public bool IsOpenQuote    {get {return character == openquote;}}
        public bool IsCloseQuote   {get {return character == closequote;}}


        // Whitespace and Semi at nesting level 0 
        public bool IsWhitespace   {get {return character == ' ' && NestingLevel == 0;}}
        public bool IsSemicolon    {get {return character == ';' && NestingLevel == 0;}}

        public bool IsLetter {get {return Char.IsLetter (character);}}
        public bool IsAlphanumeric {get {return Char.IsLetterOrDigit (character);}}



        public int  NestingLevel   {get {return bracketlevel + parenlevel + quotelevel;}}

        private const char esc        = '\\';        
        private const char quote      = '\'';        
        private const char openquote  = (char) 145; // extended ASCII left single quote
        private const char closequote = (char) 146; //   "        "   right   "     "


        //**************************************************************************
        //
        // use this ctor for single char or the first character of a string
        //
        public NestedChar (char c)
        {
            character    = c;
            bracketlevel = 0;
            parenlevel   = 0;
            quotelevel   = 0;

            bracketlevel = IsOpenBracket ? (sbyte) 1 : (sbyte) 0;
            parenlevel   = IsOpenParen   ? (sbyte) 1 : (sbyte) 0;
            quotelevel   = IsQuote       ? (sbyte) 1 : (sbyte) 0;

            if (IsCloseParen) throw new Exception ("NestingLevel: paren nesting error");
            if (IsCloseBracket) throw new Exception ("NestingLevel: bracket nesting error");
        }

        //**************************************************************************
        //
        // use this ctor for 2nd and following characters of a string
        //
        public NestedChar (NestedChar prev, char ch)
        {
            character = ch;

            // start by setting each level equal to previous character, then adjust as necessary
            parenlevel   = prev.parenlevel;
            bracketlevel = prev.bracketlevel;
            quotelevel   = prev.quotelevel;

            //********************************************************

            // check parenthesis level
            if      (IsOpenParen)  parenlevel++;
            if (prev.IsCloseParen) parenlevel--;

            //********************************************************

            // check bracket level
            if      (IsOpenBracket)  bracketlevel++;
            if (prev.IsCloseBracket) bracketlevel--;

            //********************************************************

            // check quote level. can only be 0 or 1
            if (IsQuote)
            {
                if (quotelevel == 0)
                {
                    if (prev.IsWhitespace) {quotelevel = 1; character = openquote;}
                    if (prev.IsOpenParen)  {quotelevel = 1; character = openquote;}
                }
                else // quotelevel == 1
                {
                    if (prev.character != esc) // ignore escaped quote inside of a string
                        character = closequote;
                }
            }

            if (prev.character == closequote) quotelevel--;

            //********************************************************

            // check for errors
            if (bracketlevel < 0) throw new Exception ("nestinglevel: bracket nesting error");
            if (parenlevel   < 0) throw new Exception ("nestinglevel: paren nesting error");
        }
    }
}
