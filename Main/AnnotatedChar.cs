
/*
    AnnotatedChar - an object containing one character and the depth it is
                 nested by parens, square brackets and quotes
*/



/*

        Just starting re-write of AnnotatedChar & String to make them work better with Token Parsing

*/


using System;
using System.Collections;
using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedChar
    {
        // members
        internal  char  character;


        //**************************************************************************

        // 

        public enum ACType {Unknown, Whitespace, InParens, InBrackets, InQuotes,//Whitespace2, 
                             Alphanumeric, Decimal, Number, Operator,} //Letter, 

        internal ACType thisCharType = ACType.Unknown;



        //**************************************************************************

        // public access properties
        public char Character {get {if (IsOpenQuote || IsCloseQuote) return quote;
                                    else return character;}} 

        //public sbyte BracketLevel    {get {return bracketlevel;} set {bracketlevel = value;}} 
        //public sbyte ParenLevel      {get {return parenlevel;}   set {parenlevel = value;}} 
        //public sbyte QuoteLevel      {get {return quotelevel;}   set {quotelevel = value;}} 

        public bool IsOpenParen    {get {return character == '(';}}
        public bool IsCloseParen   {get {return character == ')';}}
        public bool IsOpenBracket  {get {return character == '[';}}
        public bool IsCloseBracket {get {return character == ']';}}
        public bool IsQuote        {get {return character == '\'';}}
        public bool IsOpenQuote    {get {return character == openquote;}}
        public bool IsCloseQuote   {get {return character == closequote;}}

        // Whitespace and Semi at nesting level 0 
        //public bool IsWhitespace0   {get {return character == ' ' && NestingLevel == 0;}}
        //public bool IsSemicolon0    {get {return character == ';' && NestingLevel == 0;}}

        public bool IsWhitespace   {get {return character == ' ';}}



        public bool IsLetter {get {return Char.IsLetter (character);}}
        public bool IsAlphanumeric {get {return Char.IsLetterOrDigit (character);}}



        //public int  NestingLevel   {get {return bracketlevel + parenlevel + quotelevel;}}

        private const char esc        = '\\';        
        private const char quote      = '\'';        
        private const char openquote  = (char) 145; // extended ASCII left single quote
        private const char closequote = (char) 146; //   "        "   right   "     "

        
        //**************************************************************************

        // Test for 2 AnnotatedChars having all same nesting levels
        //public static bool SameNesting (AnnotatedChar c1, AnnotatedChar c2)
        //{
        //    return c1.quotelevel   == c2.quotelevel
        //        && c1.bracketlevel == c2.bracketlevel
        //        && c1.parenlevel   == c2.parenlevel;
        //    //    && c1.overrideType == c2.overrideType;
        //}

        //***************************************************************************

        public bool IsDecimal     {get {return character == '.';}}

        public bool IsNumber      {get {return Char.IsDigit (character);}}

        public bool IsAlpha       {get {return Char.IsLetter (character) || IsUnderscore;}}

        public bool IsUnderscore  {get {return character == '_';}}

        public bool IsOperator    {get {return Operators.Contains (character); } }// && QuoteLevel == 0;}}

     //   public bool IsTilde       {get {return character == '~';}}

      //  public bool IsEqualSign   {get {return OverrideType == ContextType.None && character == '=' && QuoteLevel == 0;}}

    //    public bool IsExponential {get {return OverrideType == ContextType.None && char.ToUpper (character) == 'E';}}

    //    public bool IsMinus       {get {return OverrideType == ContextType.None && (character == '-');}}
    //    public bool IsPlusMinus   {get {return OverrideType == ContextType.None && (character == '+' || character == '-');}}

        // these only exist as overrides
    //    public bool IsTwoCharOp {get {return OverrideType == ContextType.IsTwoCharOperator;}}
     //   public bool IsTranspose {get {return OverrideType == ContextType.IsTranspose;}}

     //  public bool IsExponent      {get {return character == '^';}}
    //    public bool IsColon         {get {return character == ':';}}
    ////    public bool IsSemicolon     {get {return character == ';';}}
     //   public bool IsComma         {get {return character == ',';}}

        //**********************************************************************************

        // all charcters in any operator: oneChar, twoChar, unary, transpose
     // static List<char> Operators = new List<char> () {';', ':', '\'', '.', '^', '*', '/', '+', '-', '&', '|', '>', '<', '~'};
        static List<char> Operators = new List<char> () {';', ':', '\'',      '^', '*', '/', '+', '-', '&', '|', '>', '<', '~', '='};

        static public bool IsTwoCharOpStr (string s) {return twoCharBinaryOperators.Contains (s);}
        static List<string> twoCharBinaryOperators = new List<string> () {".*", "./", ".^", "&&", "||", "~=", "==", ">=", "<="};

        public bool CanPreceedTranspose {get {return PreceedTranspose.Contains (character) || IsAlpha;}}
        private static readonly List<char> PreceedTranspose = new List<char> () {']', ')'};

        //**********************************************************************************

        public override string ToString ()
        {
            string str = "";
            str += Character;
            return str;
        }


        //**************************************************************************
        //
        // use this ctor for the first character of a string
        //
        public AnnotatedChar (char c)
        {
            character    = c;

            if      (IsOpenBracket) thisCharType = ACType.InBrackets;
            else if (IsOpenParen)   thisCharType = ACType.InParens;

            else if (IsQuote)       {thisCharType = ACType.InQuotes; character = openquote;}

            else if (IsWhitespace)  thisCharType = ACType.Whitespace;
            else if (IsLetter)      thisCharType = ACType.Alphanumeric;
            else if (IsNumber)      thisCharType = ACType.Number;
            else if (IsDecimal)     thisCharType = ACType.Number;
            else if (IsOperator)    thisCharType = ACType.Operator;

        }

        //**************************************************************************
        //
        // use this ctor for subsequent characters
        //
        public AnnotatedChar (AnnotatedChar prev, char ch)
        {
            character = ch;
            ACType previousCharType = prev.thisCharType; // a copy with a name that makes more sense
                                                         // in this context
            switch (previousCharType)
            {
                case ACType.Unknown: 
                    break;

                case ACType.InBrackets:
                    if (prev.IsCloseBracket) thisCharType = ACType.Unknown;
                    else                     thisCharType = ACType.InBrackets;
                    break;

                case ACType.InParens:
                    if (prev.IsCloseParen) thisCharType = ACType.Unknown;
                    else                   thisCharType = ACType.InParens;
                    break;

                //case ACType.InQuotes:
                //    if (prev.IsQuote) thisCharType = ACType.Unknown;
                //    else                     thisCharType = ACType.InBrackets;
                //    break;

                case ACType.Alphanumeric:
                    if (IsNumber) thisCharType = ACType.Alphanumeric;
                    break;

                case ACType.Number:
                    if (IsDecimal) thisCharType = ACType.Number;
                    break;

                    //case Whitespace: InParens, InBrackets, InQuotes,//Whitespace2, 
                      //      Alphanumeric, Decimal, Number, Operator,} //Letter, 

            }

            if (thisCharType == ACType.Unknown)
            {
                if      (IsOpenBracket) thisCharType = ACType.InBrackets;
                else if (IsOpenParen)   thisCharType = ACType.InParens;

                else if (IsQuote)       {thisCharType = ACType.InQuotes; character = openquote;}

                else if (IsWhitespace)  thisCharType = ACType.Whitespace;
                else if (IsLetter)      thisCharType = ACType.Alphanumeric;
                else if (IsNumber)      thisCharType = ACType.Number;
                else if (IsDecimal)     thisCharType = ACType.Number;
                else if (IsOperator)    thisCharType = ACType.Operator;
            }

        }

        //*******************************************************************
        //
        // Operator overloading
        //
        public static bool operator == (AnnotatedChar left, char right)
        {
            return left.Character == right;  // is this adequate?
        }

        public static bool operator != (AnnotatedChar left, char right)
        {
            return left.Character != right;
        }

        // Override the virtual Equals method
        public override bool Equals (object obj)
        {
            char other = (char) obj;
            return character == other;
        }

        // Override GetHashCode
        public override int GetHashCode()
        {
            return character.GetHashCode();// ^ Name.GetHashCode(); // Combine hash codes of key properties
        }    
    }
}
