
/*
    NestingLevel - an object containing one character and the depth it is
                   nested in by parens, square brackets and quotes
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Main
{
    public class AnnotatedChar
    {
        // private members
        private char  character;
        private sbyte quotelevel;  // can only be 0 or 1, no nested quotes allowed
        private sbyte bracketlevel;
        private sbyte parenlevel;

        private ContextType overrideType;

        //***************************************************************************

        internal enum ContextType {None, IsNumber, IsTwoCharOperator, IsTranspose, IsLetter, IsSupressOutput};

        internal ContextType OverrideType
        {
            set {overrideType = value;}
            get {return overrideType;}
        }

        //***************************************************************************

        // public access properties
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

        public sbyte QuoteLevel      {get {return quotelevel;}   set {quotelevel = value;}} 
        public sbyte BracketLevel    {get {return bracketlevel;} set {bracketlevel = value;}} 
        public sbyte ParenLevel      {get {return parenlevel;}   set {parenlevel = value;}} 

        public bool IsDecimal     {get {return OverrideType == ContextType.None && character == '.';}}

        public bool IsNumber      {get {return OverrideType == ContextType.None ? Char.IsDigit (character) 
                                                                                : OverrideType == ContextType.IsNumber;}}


        public bool IsAlpha       {get {return OverrideType == ContextType.None ? Char.IsLetter (character) || IsUnderscore
                                                                                : OverrideType == ContextType.IsLetter;}}

        public bool IsUnderscore  {get {return OverrideType == ContextType.None && character == '_';}}

        public bool IsOperator    
            {get 
                {
                    bool b1 = OverrideType == ContextType.None && Operators.Contains (character);
                    bool b2 = false;//OverrideType == ContextType.IsTranspose;
                    return b1 || b2;
                }
            }

        public bool IsQuote       {get {return OverrideType == ContextType.None && character == '\'';}}
        public bool IsTilde       {get {return OverrideType == ContextType.None && character == '~';}}

        public bool IsEqualSign   {get {return OverrideType == ContextType.None && character == '=';}}

        public bool IsExponential {get {return OverrideType == ContextType.None && char.ToUpper (character) == 'E';}}

        public bool IsPlusMinus   {get {return OverrideType == ContextType.None && (character == '+' || character == '-');}}

        // these only exist as overrides
        public bool IsTwoCharOp { get { return OverrideType == ContextType.IsTwoCharOperator; } }
        public bool IsTranspose { get { return OverrideType == ContextType.IsTranspose; } }
        public bool IsSupress   { get { return OverrideType == ContextType.IsSupressOutput; } }



        // these are never overriden
        public bool IsOpenParen { get { return character == '('; } }
        public bool IsCloseParen { get { return character == ')'; } }
        public bool IsOpenBracket { get { return character == '['; } }
        public bool IsCloseBracket { get { return character == ']'; } }








        public bool IsOpenQuote { get { return character == openquote; } }
        public bool IsCloseQuote { get { return character == closequote; } }
        public bool IsWhitespace   {get {return (character == ' ' || character == '\t'); }}


        public bool IsUnaryOp  {get {return unaryLeftOperators.Contains (character);}}
        //      public bool IsUnaryRightOp {get {return unaryRightOperators.Contains (character);}}




        //      public bool IsExponent      {get {return character == '^';}}
        //      public bool IsColon         {get {return character == ':';}}
        public bool IsSemicolon     {get {return character == ';';}}
        //      public bool IsComma         {get {return character == ',';}}

        //**********************************************************************************

        // all charcters in any operator: oneChar, twoChar, unary, transpose
        static List<char> Operators = new List<char> () {';', '\'', '.', '^', '*', '/', '+', '-', '=', '&', '|', '>', '<', '~' };

        static public bool IsTwoCharOpStr (string s) {return twoCharBinaryOperators.Contains (s);}
        static List<string> twoCharBinaryOperators = new List<string> () {".*", "./", ".^", "&&", "||", "~=", "==", ">=", "<="};


        // static List<char>   oneCharBinaryOperators = new List<char> ()   {'^', '*', '/', '+', '-', '=', '&', '|', '>', '<', '~'};
        static List<char>   unaryLeftOperators     = new List<char> ()   {'+', '-', '~'}; // to the left of operand
        // static List<char>   unaryRightOperators    = new List<char> ()   {'\''};          // "  "  right "  "

        //      public bool IsBinaryOp     {get {return oneCharBinaryOperators.Contains (character);}}

        public bool CanPreceedTranspose { get {return PreceedTranspose.Contains (character) || IsAlpha; } }
        static List<char>   PreceedTranspose = new List<char> ()   {']', ')'};

        //**********************************************************************************

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
        // use this ctor for single char or the first character of a string
        //
        public AnnotatedChar (char c)
        {
            character    = c;
            bracketlevel = 0; // language requires all fields be initialized
            parenlevel   = 0;
            quotelevel   = 0;
            overrideType = ContextType.None;

            quotelevel   = IsQuote       ? (sbyte) 1 : (sbyte) 0;
            bracketlevel = IsOpenBracket ? (sbyte) 1 : (sbyte) 0;
            parenlevel   = IsOpenParen   ? (sbyte) 1 : (sbyte) 0;

            if (IsCloseParen) throw new Exception ("NestingLevel: paren nesting error");
            if (IsCloseBracket) throw new Exception ("NestingLevel: bracket nesting error");
        }

        //**************************************************************************
        //
        // use this ctor for subsequent characters
        //
        public AnnotatedChar (AnnotatedChar prev, char ch)
        {
            character = ch;

            // start by setting each level equal to previous character, then adjust as necessary
            parenlevel   = prev.parenlevel;
            bracketlevel = prev.bracketlevel;
            quotelevel   = prev.quotelevel;
            overrideType = ContextType.None;

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
            if (quotelevel < 0 || quotelevel > 1) throw new Exception ("nestinglevel: quote nesting error");
            if (bracketlevel < 0)                 throw new Exception ("nestinglevel: bracket nesting error");
            if (parenlevel   < 0)                 throw new Exception ("nestinglevel: paren nesting error");
        }

        private const char esc   = '\\';        

        static public char quote = '\'';        
        private const char openquote     = (char) 145; // extended ASCII left single quote
        private const char closequote    = (char) 146; //   "        "   right   "     "

     //   static public char openbracket  = '[';
      //  static public char closebracket = ']';
        
     //   static public char openparen  = '(';
     //   static public char closeparen = ')';

       // public int Quotelevel {get {return quotelevel;}}
        public int NestingLevel {get {return quotelevel + bracketlevel + parenlevel;}}

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
