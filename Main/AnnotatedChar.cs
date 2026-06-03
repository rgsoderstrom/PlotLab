
/*
    AnnotatedChar - 
*/

using System;
using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedChar : NestedChar
    {
        private ContextType overrideType;

        //**************************************************************************

        // Test for 2 AnnotatedChars having all same nesting levels
        public static bool SameNesting (AnnotatedChar c1, AnnotatedChar c2)
        {
            return c1.quotelevel   == c2.quotelevel
                && c1.bracketlevel == c2.bracketlevel
                && c1.parenlevel   == c2.parenlevel
                && c1.overrideType == c2.overrideType;
        }

        //***************************************************************************

        internal enum ContextType {None, IsNumber, IsTwoCharOperator, IsTranspose, IsLetter};

        internal ContextType OverrideType
        {
            set {overrideType = value;}
            get {return overrideType;}
        }

        //***************************************************************************

        public bool IsDecimal     {get {return OverrideType == ContextType.None && character == '.';}}

        public bool IsNumber      {get {return OverrideType == ContextType.None ? Char.IsDigit (character) 
                                                                                : OverrideType == ContextType.IsNumber;}}

        public bool IsAlpha       {get {return OverrideType == ContextType.None ? Char.IsLetter (character) || IsUnderscore
                                                                                : OverrideType == ContextType.IsLetter;}}

        public bool IsUnderscore  {get {return OverrideType == ContextType.None && character == '_';}}

        public bool IsOperator    
            {
                get 
                {
                    bool b1 = OverrideType == ContextType.None && Operators.Contains (character) && QuoteLevel == 0;
                    bool b2 = false;//OverrideType == ContextType.IsTranspose;
                    return b1 || b2;
                }
            }

        public bool IsTilde       {get {return OverrideType == ContextType.None && character == '~';}}

        public bool IsEqualSign   {get {return OverrideType == ContextType.None && character == '=' && QuoteLevel == 0;}}

        public bool IsExponential {get {return OverrideType == ContextType.None && char.ToUpper (character) == 'E';}}

        public bool IsMinus       {get {return OverrideType == ContextType.None && (character == '-');}}
        public bool IsPlusMinus   {get {return OverrideType == ContextType.None && (character == '+' || character == '-');}}

        // these only exist as overrides
        public bool IsTwoCharOp {get {return OverrideType == ContextType.IsTwoCharOperator;}}
        public bool IsTranspose {get {return OverrideType == ContextType.IsTranspose;}}

        public bool IsExponent      {get {return character == '^';}}
        public bool IsColon         {get {return character == ':';}}
        public bool IsSemicolon     {get {return character == ';';}}
        public bool IsComma         {get {return character == ',';}}

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

        public string ToDetailedString ()
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
        public AnnotatedChar (char c) : base (c)
        {
            overrideType = ContextType.None;
        }

        //**************************************************************************
        //
        // use this ctor for subsequent characters
        //
        public AnnotatedChar (AnnotatedChar prev, char ch) : base (prev, ch)
        {
            overrideType = ContextType.None;
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
