
/*
    AnnotatedChar - an object containing one character and the depth it is
                 nested by parens, square brackets and quotes
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
        internal  sbyte bracketlevel;
        internal  sbyte parenlevel;

        //**************************************************************************

        // 

        public enum ACType {Unknown, Whitespace,
                            Semicolon, Colon,
                            Letter, Number, DecimalPoint, 
                            OpenBracket, CloseBracket,
                            OpenParen, CloseParen,
                            Quote,    
                            Operator, Escape, 
                            
                            //Alphanumeric, 
                            //OpenQuote, CloseQuote, EscapedQuote, 
                            //TwoCharOperator,
                } 

        internal ACType thisCharType = ACType.Unknown;

        //**************************************************************************

        // access properties
        public char Character {get {return character;}}

        public sbyte BracketLevel {get {return bracketlevel;} set {bracketlevel = value;}}
        public sbyte ParenLevel   {get {return parenlevel;}   set {parenlevel = value;}}
     //   public sbyte QuoteLevel   {get {return quotelevel;}   set {quotelevel = value;}}
        public int   NestingLevel {get {return bracketlevel + parenlevel /*+ quotelevel*/;}}

        public bool IsOpenParen    {get {return character == '(';}}
        public bool IsCloseParen   {get {return character == ')';}}
        public bool IsOpenBracket  {get {return character == '[';}}
        public bool IsCloseBracket {get {return character == ']';}}
        public bool IsQuote        {get {return character == quote;}}
        public bool IsEscape       {get {return character == esc;}}

       // public bool IsOpenQuote    {get {return thisCharType == ACType.OpenQuote;}}
      //  public bool IsCloseQuote   {get {return thisCharType == ACType.CloseQuote;}}

        // Whitespace and Semi at nesting level 0 
        //public bool IsLevel0Whitespace { get { return IsWhitespace && NestingLevel == 0; } }
        //public bool IsLevel0Semicolon  { get { return IsSemicolon && NestingLevel == 0; } }

        public bool IsWhitespace {get {return character == ' ';}}
        public bool IsSemicolon  {get {return character == ';';}}
        public bool IsColon      {get {return character == ':';}}


        public bool IsLetter {get {return Char.IsLetter (character);}}
        public bool IsAlphanumeric {get {return Char.IsLetterOrDigit (character);}}




        private const char esc        = '\\';        
        private const char quote      = '\'';        

        
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

        public bool IsOperator    {get {return Operators.Contains (character);}}

     //   public bool IsTilde       {get {return character == '~';}}

      //  public bool IsEqualSign   {get {return OverrideType == ContextType.None && character == '=' && QuoteLevel == 0;}}

        public bool IsExponential {get {return char.ToUpper (character) == 'E';}}

        public bool IsMinus       {get {return character == '-';}}
        public bool IsPlusMinus   {get {return character == '+' || character == '-';}}

        // these only exist as overrides
    //    public bool IsTwoCharOp {get {return OverrideType == ContextType.IsTwoCharOperator;}}
     //   public bool IsTranspose {get {return OverrideType == ContextType.IsTranspose;}}

     //  public bool IsExponent      {get {return character == '^';}}
    //    public bool IsColon         {get {return character == ':';}}
    ////    public bool IsSemicolon     {get {return character == ';';}}
     //   public bool IsComma         {get {return character == ',';}}

        //**********************************************************************************

        // all charcters in any operator: oneChar, twoChar, unary, transpose
        static List<char> Operators = new List<char> () {';', ':', '\'', '.', '^', '*', '/', '+', '-', '&', '|', '>', '<', '~', '='};

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
            AssignInitialType ();

            bracketlevel = IsOpenBracket ? (sbyte) 1 : (sbyte) 0;
            parenlevel   = IsOpenParen   ? (sbyte) 1 : (sbyte) 0;
        }

        private void AssignInitialType ()
        { 
            if      (IsWhitespace)  thisCharType = ACType.Whitespace;
            else if (IsSemicolon)   thisCharType = ACType.Semicolon;
            else if (IsColon)       thisCharType = ACType.Colon;

            else if (IsLetter)      thisCharType = ACType.Letter;
            else if (IsDecimal)     thisCharType = ACType.DecimalPoint;
            else if (IsNumber)      thisCharType = ACType.Number;

            else if (IsOpenBracket)  thisCharType = ACType.OpenBracket;
            else if (IsCloseBracket) thisCharType = ACType.CloseBracket;
            else if (IsOpenParen)    thisCharType = ACType.OpenParen;
            else if (IsCloseParen)   thisCharType = ACType.CloseParen;

            else if (IsEscape)       thisCharType = ACType.Escape;
            else if (IsQuote)        thisCharType = ACType.Quote;
            else if (IsOperator)     thisCharType = ACType.Operator;

            else throw new Exception ("AssignInitialType failed for character " + character);
        }

        //**************************************************************************
        //
        // use this ctor for subsequent characters
        //
        public AnnotatedChar (AnnotatedChar prev, char ch)
        {
            character = ch;
            AssignInitialType ();

            // start by setting each level equal to previous character, then adjust as necessary
            parenlevel   = prev.parenlevel;
            bracketlevel = prev.bracketlevel;

            //********************************************************

            // check parenthesis level
            if      (IsOpenParen)  parenlevel++;
            if (prev.IsCloseParen) parenlevel--;

            //********************************************************

            // check bracket level
            if      (IsOpenBracket)  bracketlevel++;
            if (prev.IsCloseBracket) bracketlevel--;

            //********************************************************



            /*************
            ACType previousCharType = prev.thisCharType; // a copy, with a name that makes more sense
                                                         // in this context
            switch (previousCharType)
            {
                case ACType.Unknown: 
                    break;


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
            } **********/



            //********************************************************

            // check for errors
            if (bracketlevel < 0) throw new Exception ("nestinglevel: bracket nesting error");
            if (parenlevel   < 0) throw new Exception ("nestinglevel: paren nesting error");

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
