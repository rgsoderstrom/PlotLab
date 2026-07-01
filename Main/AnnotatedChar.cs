
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
                            /*Letter,*/ Number, DecimalPoint, 
                            OpenBracket, CloseBracket,
                            OpenParen, CloseParen,
                            Quote,    
                            Operator, Escape, Percent,
                            
                            //Separator,          // comma, colon, semicolon
                            Alphanumeric, 
                            String, OpenQuote, CloseQuote, EscapedQuote, 
                            Transpose,
                            TwoCharOperator,
                } 

        public ACType thisCharType = ACType.Unknown;

        //**************************************************************************

        // access properties
        public char Character {get {return character;}}

        public sbyte BracketLevel {get {return bracketlevel;} set {bracketlevel = value;}}
        public sbyte ParenLevel   {get {return parenlevel;}   set {parenlevel = value;}}
        public int   NestingLevel {get {return bracketlevel + parenlevel;}}

        private bool IsOpenParen_    {get {return character == '(';}}
        private bool IsCloseParen_   {get {return character == ')';}}
        private bool IsOpenBracket_  {get {return character == '[';}}
        private bool IsCloseBracket_ {get {return character == ']';}}
        private bool IsQuote_        {get {return character == quote;}}
        private bool IsEscape_       {get {return character == esc;}}
        private bool IsPercent_      {get {return character == '%';}}

       // public bool IsOpenQuote    {get {return thisCharType == ACType.OpenQuote;}}
      //  public bool IsCloseQuote   {get {return thisCharType == ACType.CloseQuote;}}

        // Whitespace and Semi at nesting level 0 
        public bool IsLevel0Whitespace {get {return thisCharType == ACType.Whitespace && NestingLevel == 0;}}
        public bool IsLevel0Semicolon  {get {return thisCharType == ACType.Semicolon && NestingLevel == 0;}}

        private bool IsWhitespace_ {get {return character == ' ';}}
        private bool IsSemicolon_  {get {return character == ';';}}
        private bool IsColon_      {get {return character == ':';}}


        private bool IsLetter_ {get {return Char.IsLetter (character);}}
        private bool IsAlphanumeric_ {get {return Char.IsLetterOrDigit (character);}}




        private const char esc        = '\\';        
        private const char quote      = '\'';


        //**************************************************************************

        // Test for 2 AnnotatedChars having all same nesting levels
        public static bool SameNesting (AnnotatedChar c1, AnnotatedChar c2)
        {
            return c1.bracketlevel == c2.bracketlevel && c1.parenlevel == c2.parenlevel;
        }

        //***************************************************************************

        public bool IsAlphanumeric {get {return thisCharType == ACType.Alphanumeric;}}
        public bool IsDecimal      {get {return thisCharType == ACType.DecimalPoint;}}
        public bool IsNumber       {get {return thisCharType == ACType.Number;}}
        public bool IsQuote        {get {return thisCharType == ACType.Quote;}}
        public bool IsOperator     {get {return thisCharType == ACType.Operator;}}
        public bool IsWhitespace   {get {return thisCharType == ACType.Whitespace;}}
        public bool IsSemicolon    {get {return thisCharType == ACType.Semicolon;}}
        public bool IsEqualSign    {get {return thisCharType == ACType.Operator && IsEqualSign_;}}  //

        public bool IsExponential {get {return IsExponential_;}}
        public bool IsPlusMinus   {get {return IsPlusMinus_;}}

        private bool IsDecimal_     {get {return character == '.';}}
        private bool IsNumber_      {get {return Char.IsDigit (character);}}
  //    private bool IsAlpha_       {get {return Char.IsLetter (character) || IsUnderscore_;}}
        private bool IsUnderscore_  {get {return character == '_';}}
        private bool IsOperator_    {get {return Operators.Contains (character);}}
     // private bool IsTilde_       {get {return character == '~';}}
        private bool IsEqualSign_   {get {return character == '=';}}
        private bool IsExponential_ {get {return char.ToUpper (character) == 'E';}}
        private bool IsMinus_       {get {return character == '-';}}
        private bool IsPlusMinus_   {get {return character == '+' || character == '-';}}

        public bool IsTwoCharOp   {get {return thisCharType == ACType.TwoCharOperator;}}
        public bool IsTranspose   {get {return thisCharType == ACType.Transpose;}}
        public bool IsInString    {get {return thisCharType == ACType.String;}}

        //  public bool IsExponent      {get {return character == '^';}}
        //    public bool IsColon         {get {return character == ':';}}
        ////    public bool IsSemicolon     {get {return character == ';';}}
        //   public bool IsComma         {get {return character == ',';}}

        //**********************************************************************************

        // all charcters in any operator: oneChar, twoChar, unary, transpose
        //static List<char> Operators = new List<char> () {',', ';', ':', '\'', '.', '^', '*', '/', '+', '-', '&', '|', '>', '<', '~', '='};
        static List<char> Operators = new List<char> () {',', ';', ':',       '.', '^', '*', '/', '+', '-', '&', '|', '>', '<', '~', '='};

        //static public bool IsTwoCharOpStr (string s) {return twoCharBinaryOperators.Contains (s);}
        //static readonly List<string> twoCharBinaryOperators = new List<string> () {".*", "./", ".^", "&&", "||", "~=", "==", ">=", "<="};

        //public bool CanPreceedTranspose {get {return PreceedTranspose.Contains (character) || IsAlpha;}}
        //private static readonly List<char> PreceedTranspose = new List<char> () {']', ')'};

        public bool CanPreceedString { get { return PreceedString.Contains (character) || thisCharType == ACType.Operator; } }
        private static readonly List<char> PreceedString = new List<char> () {' ', '(', '[' };

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

            bracketlevel = IsOpenBracket_ ? (sbyte) 1 : (sbyte) 0;
            parenlevel   = IsOpenParen_   ? (sbyte) 1 : (sbyte) 0;
        }

        private void AssignInitialType ()
        { 
            if      (IsWhitespace_)  thisCharType = ACType.Whitespace;
            else if (IsSemicolon_)   thisCharType = ACType.Semicolon;
            else if (IsColon_)       thisCharType = ACType.Colon;

            else if (IsLetter_)      thisCharType = ACType.Alphanumeric;
            else if (IsUnderscore_)  thisCharType = ACType.Alphanumeric;
            else if (IsDecimal_)     thisCharType = ACType.DecimalPoint;
            else if (IsNumber_)      thisCharType = ACType.Number;

            else if (IsOpenBracket_)  thisCharType = ACType.OpenBracket;
            else if (IsCloseBracket_) thisCharType = ACType.CloseBracket;
            else if (IsOpenParen_)    thisCharType = ACType.OpenParen;
            else if (IsCloseParen_)   thisCharType = ACType.CloseParen;

            else if (IsEscape_)       thisCharType = ACType.Escape;
            else if (IsQuote_)        thisCharType = ACType.Quote;
            else if (IsOperator_)     thisCharType = ACType.Operator;
            else if (IsPercent_)      thisCharType = ACType.Percent;

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
            if      (IsOpenParen_)  parenlevel++;
            if (prev.IsCloseParen_) parenlevel--;

            //********************************************************

            // check bracket level
            if      (IsOpenBracket_)  bracketlevel++;
            if (prev.IsCloseBracket_) bracketlevel--;

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
