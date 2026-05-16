
/*
    AnnotatedString - list of AnnotatedCharacters
*/

using System;
using System.Collections.Generic;

using static System.Net.Mime.MediaTypeNames;

namespace PLMain
{
    public class AnnotatedString
    {
        // private members
        private List<AnnotatedChar> annotatedChars = new List<AnnotatedChar> ();

        //*************************************************************************
        //
        // ctors
        //
        internal AnnotatedString (string text)
        {
            if (text.Length == 0)
                return;

            try
            {
                string text2 = Preprocess (text);

                if (text2.Length == 0)
                    return;

                PassOne (text2);
                PassTwo ();
                PassThree ();
            }

            catch (Exception ex)
            {
                throw new Exception ("Error in AnnotatedString ctor:\n" + ex.Message);
                //throw new Exception ("Error in AnnotatedString ctor:\n" + ex.StackTrace);
            }

        }

        //*************************************************************************

        // during pass1 note the locations of characters we may want to modify during subsequent processing
        private readonly List<int> digits       = new List<int> ();
        private readonly List<int> decimals     = new List<int> ();
        private readonly List<int> exponentials = new List<int> (); // E or e
        private readonly List<int> operators    = new List<int> ();

        private readonly List<int> whiteSpaces  = new List<int> (); // now: level 0 whitespaces

        private readonly List<int> semicolons   = new List<int> ();
        private readonly List<int> level0Semis  = new List<int> ();

        public List<int> Level0Semis {get {return level0Semis;}}

        //*************************************************************************

        // public access properties
        public string Plain // plain text without annotation
        {
            get
            {
                string str = "";

                for (int i = 0; i<annotatedChars.Count; i++)
                    str += annotatedChars [i].Character;

                return str;
            }
        }

        public int CharacterCount {get {return annotatedChars.Count;}}
        public bool IsEmpty {get {return CharacterCount == 0;}}
        public int FinalBracketLevel {get {return annotatedChars [CharacterCount-1].BracketLevel;}}

        private bool alphanumericOnly = true;
        public  bool AlphanumericOnly {get {return alphanumericOnly;} set {alphanumericOnly = value;}}

        private bool suppressOutput = false; // set true if last char is semicolon
        public  bool SuppressOutput {get {return suppressOutput;} set {suppressOutput = value;}}

        private bool isCompound = false; // set true for stmts of the form: a = 7; b = 10; c = 12;
        public  bool IsCompound {get {return isCompound;} private set {isCompound = value;}}

        public  bool SingleWord {get {return ArgumentString == "";}}

        //************************************************************************

        // Return first word of input string

        // Commands
        //  - clear a b c % returns "clear"

        // For block start
        //  - for a = 1:9, % return "for"

        // function declaration
        //  - function [x, y, z] =   % returns "function"

        public string FirstWord 
        {
            get
            {
                if (whiteSpaces.Count == 0)
                    return Plain;

                string str = "";

                for (int i=0; i<CharacterCount; i++)
                {
                    if (i == whiteSpaces [0])
                        break;

                    str += annotatedChars [i].Character;
                }

                return str;
            }
        }

        //************************************************************************

        // Return everything after FirstWord

        //  - clear a b c % returns "a b c"
        //  - for a = 1:9, % returns a = 1:9,

        public string ArgumentString // all chars after the first word
        {
            get
            {
                string str = "";

                if (whiteSpaces.Count == 0)
                    return str;

                for (int i=whiteSpaces [0] + 1; i<CharacterCount; i++)
                    str += annotatedChars [i].Character;

                return str;
            }
        }

        //*************************************************************************

        // if line ends in semicolon, remove it and set SuppressOutput = true


        // ALREADY DONE IN PASS3 BUT NEEDS TO BE DONE EARLIER


        private string Preprocess (string text)
        { 
            if (text [text.Length - 1] == ';')
            {
                text = text.Remove (text.Length - 1, 1);
                SuppressOutput = true;
            }

            return text;
        }

        //*************************************************************************

        private void PassOne (string text)
        { 
            for (int i=0; i<text.Length; i++)
            {
                AnnotatedChar nextAC = i > 0 ? new AnnotatedChar (annotatedChars [i-1], text [i])
                                             : new AnnotatedChar (text [i]);

                if (nextAC.IsNumber)      digits.Add       (i);   
                if (nextAC.IsDecimal)     decimals.Add     (i); 
                if (nextAC.IsExponential) exponentials.Add (i);



                // don't count trailing semicolon as an operator
                if (nextAC.IsSemicolon == false || i != text.Length - 1)
                    if (nextAC.IsOperator)    
                        operators.Add    (i);



                if (nextAC.IsWhitespace && nextAC.NestingLevel == 0)  
                    whiteSpaces.Add  (i);

                if (nextAC.IsSemicolon)   semicolons.Add   (i);

              //if (nextAC.IsEqualSign)   AlphanumericOnly = false;
                if (nextAC.IsOpenParen)   AlphanumericOnly = false;
                if (nextAC.IsOpenBracket) AlphanumericOnly = false;

                annotatedChars.Add (nextAC);
            }

            if (operators.Count > 0) AlphanumericOnly = false;
        }

        //*************************************************************************

        private void PassTwo ()
        {
            // look for digits that are part of a variable name, e.g. A12;
            // change their type to Letter
            foreach (int i in digits)
            {
                int before = i - 1;

                if (before >= 0)
                    if (annotatedChars [before].IsAlpha && annotatedChars [before].IsExponential == false)
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsLetter;
            }

            //*********************************************************************

            // combine decimal point with number (e.g. .123 or 123.456)
            // change its type to Number
            foreach (int i in decimals)
            {
                int before = i - 1;

                if (before >= 0)
                {
                    if (annotatedChars [before].IsNumber)
                    {
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
                    }
                }

                int after = i + 1;

                if (after < CharacterCount)
                {
                    if (annotatedChars [after].IsNumber)
                    {
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
                    }
                }
            }

            //*********************************************************************

            // mark leading +/- with number (e.g. -123.456) as numeric

            foreach (int i in operators)
            {
                if (annotatedChars [i].IsPlusMinus)
                { 
                    bool beforeTest = false; // set true if char before the +/- indicates
                                             // the +/- is a unary op

                    if (i == 0)
                        beforeTest = true;

                    else
                        for (int before = i - 1; before >= 0; before--)
                        {
                            if (annotatedChars [before].IsEqualSign) {beforeTest = true; break;}
                            if (annotatedChars [before].IsOperator) {beforeTest = true; break;}
                            if (annotatedChars [before].IsAlpha) {break;}
                            if (annotatedChars [before].IsNumber) {break;}
                        }

                    if (beforeTest == true)
                    { 
                        int after = i + 1;

                        if (after < CharacterCount)
                        {
                            if (annotatedChars [after].IsNumber)
                            {
                                annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
                            }
                        }
                    }
                }
            }

            //*********************************************************************

            // look for exponentials. Mark the "E" as a number
            foreach (int i in exponentials)
            {
                int before = i - 1;
                int after = i + 1;

                if (before >= 0 && after < CharacterCount)
                {
                    if (annotatedChars [before].IsNumber && (annotatedChars [after].IsNumber || annotatedChars [after].IsPlusMinus))
                    {
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;

                        if (annotatedChars [after].IsPlusMinus)
                        {
                            annotatedChars [after].OverrideType = AnnotatedChar.ContextType.IsNumber;
                        }
                    }
                }
            }

            //*********************************************************************

            // look for:
            //   two char operators, e.g. A >= B
            //   transpose, A'
            foreach (int i in operators)
            {
                int before = i - 1;
                int after = i + 1;

                if (before >= 0 && after < annotatedChars.Count)
                {
                    // two-char operators
                    bool t1 = annotatedChars [i].IsOperator;// || annotatedChars [i].IsEqualSign;
                    bool t2 = annotatedChars [after].IsOperator;// || annotatedChars [after].IsEqualSign;
                    bool t3 = annotatedChars [before].IsDecimal;

                    if (t1 && t2) // e.g. ">="
                    {
                        string str = annotatedChars [i].Character.ToString ();
                        str += annotatedChars [after].Character;

                        if (AnnotatedChar.IsTwoCharOpStr (str))
                        {
                            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
                            annotatedChars [after].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
                        }
                    }
            
                    else if (t1 && t3) // e.g. ".*"
                    {
                        string str = annotatedChars [before].Character.ToString ();
                        str += annotatedChars [i].Character;

                        if (AnnotatedChar.IsTwoCharOpStr (str))
                        {
                            annotatedChars [before].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
                            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
                        }
                    }
                }

                // transpose
                if (before >= 0)
                {
                    if (annotatedChars [i].IsQuote && annotatedChars [before].CanPreceedTranspose)
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTranspose;
                }
            }
        }

        //*************************************************************************

        private void PassThree () 
        {
            foreach (int index in semicolons)
            {
                if (annotatedChars [index].NestingLevel == 0)
                    level0Semis.Add (index);
            }

            if (level0Semis.Count > 0)
                IsCompound = true;
        }

        //*************************************************************************

        //internal AnnotatedString (char Character)
        //{
        //    annotatedChars = new List<AnnotatedChar> (10)
        //    {
        //        new AnnotatedChar (Character)
        //    };
        //}

        //*******************************************************************

        internal AnnotatedString (AnnotatedChar Character)
        {
            annotatedChars = new List<AnnotatedChar> (10)
            {
                Character
            };
        }

        //*******************************************************************

        private AnnotatedString (List<AnnotatedChar> source, int start, int count)
        {
            annotatedChars = new List<AnnotatedChar> (count);

            try
            {
                for (int i = start; i<start+count; i++)
                    annotatedChars.Add (source [i]);
            }

            catch (Exception ex)
            {
                throw new Exception ("Exception in AnnotatedStringCtor: " + ex.Message);
                //Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        //*******************************************************************

        // Copy constructor

        private AnnotatedString (AnnotatedString source)
        {
            annotatedChars = new List<AnnotatedChar> (source.CharacterCount);

            try
            {
                for (int i = 0; i<source.CharacterCount; i++)
                    annotatedChars.Add (source [i]);
            }

            catch (Exception ex)
            {
                throw new Exception ("Exception in AnnotatedStringCtor: " + ex.Message);
                //Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        //*******************************************************************
        //
        // Operator overloading
        //
        public static AnnotatedString operator + (AnnotatedString left, AnnotatedString right)
        {
            AnnotatedString sum = new AnnotatedString (left);
            sum.Append (right);
            return sum;
        }

        //*******************************************************************
        //
        // Append
        //
        internal void Append (AnnotatedChar ach)
        {
            annotatedChars.Add (ach);
        }

        internal void Append (AnnotatedString astr)
        {
            for (int i=0; i<astr.CharacterCount; i++)
                annotatedChars.Add (astr [i]);
        }

        //*******************************************************************
        //
        // Return substring with leading and trailing spaces removed
        //
        public AnnotatedString TrimmedSubstring (int start, int count)
        {
            while (annotatedChars [start].IsWhitespace && count > 1)
            {
                start++;
                count--;
            }

            while (annotatedChars [start + count - 1].IsWhitespace && count > 1)
            {
                count--;
            }

            return new AnnotatedString (annotatedChars, start, count);
        }

        // no trimming
        public AnnotatedString Substring (int start, int count)
        {
            return new AnnotatedString (annotatedChars, start, count);
        }

        //*******************************************************************
        //
        // Add outer parenthesis
        //

        internal AnnotatedString AddOuterParens ()
        {
            List<AnnotatedChar> newChars = new List<AnnotatedChar> (CharacterCount + 2);

            foreach (AnnotatedChar ac in annotatedChars)
            {
                AnnotatedChar newChar = ac;
                newChar.ParenLevel++;
                newChars.Add (newChar); 
            }

            // new initial character
            AnnotatedChar c1 = new AnnotatedChar ('(');

            // if the previous first char raised a nesting level, we need to undo that for new first char
            c1.ParenLevel   = (sbyte) (annotatedChars [0].IsOpenParen   ? annotatedChars [0].ParenLevel - 1   : annotatedChars [0].ParenLevel);
            c1.BracketLevel = (sbyte) (annotatedChars [0].IsOpenBracket ? annotatedChars [0].BracketLevel - 1 : annotatedChars [0].BracketLevel);
            c1.QuoteLevel   = (sbyte) (annotatedChars [0].IsQuote       ? annotatedChars [0].QuoteLevel - 1   : annotatedChars [0].QuoteLevel);

            newChars.Insert (0, c1);

            // new final close paren
            AnnotatedChar c2 = new AnnotatedChar (annotatedChars [annotatedChars.Count - 1], ')');
            newChars.Add (c2);

          //  return new AnnotatedString (newChars);

            annotatedChars = newChars;
            return this;
        }

        //*******************************************************************
        //
        // Add outer square brackets
        //

        internal AnnotatedString AddOuterBrackets ()
        {
            List<AnnotatedChar> newChars = new List<AnnotatedChar> (CharacterCount + 2);

            foreach (AnnotatedChar ac in annotatedChars)
            {
                AnnotatedChar newChar = ac;
                newChar.BracketLevel++;
                newChars.Add (newChar); 
            }

            // new initial character
            AnnotatedChar c1 = new AnnotatedChar ('[');

            // if the previous first char raised a nesting level, we need to undo that for new first char
            c1.ParenLevel   = (sbyte) (annotatedChars [0].IsOpenParen   ? annotatedChars [0].ParenLevel - 1   : annotatedChars [0].ParenLevel);
            c1.BracketLevel = (sbyte) (annotatedChars [0].IsOpenBracket ? annotatedChars [0].BracketLevel - 1 : annotatedChars [0].BracketLevel);
            c1.QuoteLevel   = (sbyte) (annotatedChars [0].IsQuote       ? annotatedChars [0].QuoteLevel - 1   : annotatedChars [0].QuoteLevel);

            newChars.Insert (0, c1);

            // new final close paren
            AnnotatedChar c2 = new AnnotatedChar (annotatedChars [annotatedChars.Count - 1], ']');
            newChars.Add (c2);

          //  return new AnnotatedString (newChars);

            annotatedChars = newChars;
            return this;
        }

        //*******************************************************************
        //
        // Indexer
        //
        internal AnnotatedChar this [int index]
        {
            get
            {
                if (index >= 0 && index < annotatedChars.Count)
                    return annotatedChars [index];

                throw new IndexOutOfRangeException ("Index is out of range in AnnotatedString indexer get.");
            }

            set
            {
                if (index >= 0 && index < annotatedChars.Count)
                    annotatedChars [index] = value;

                else
                    throw new IndexOutOfRangeException ("Index is out of range in AnnotatedString indexer set.");
            }
        }

        //*******************************************************************
        //
        // RemoveWrapper
        //  - typically parens or square brackets
        //  - also removes spaces on either end
        //
        public AnnotatedString RemoveWrapper ()
        {
            int startIndex = 1;
            int endIndex = annotatedChars.Count - 2;

            while (annotatedChars [startIndex].IsWhitespace && startIndex < endIndex)
                startIndex++;

            while (annotatedChars [endIndex].IsWhitespace && endIndex > startIndex)
                endIndex--;

            return new AnnotatedString (annotatedChars, startIndex, endIndex - startIndex + 1);
        }

        //****************************************************************************************
        //
        // ToString ()
        //

        // test for text line with something other than '.' after initial colon
        private bool NotAllDots (string str)
        {
            bool results = false;

            int i = str.IndexOf (':') + 1;

            while (i<str.Length)
            {
                if (str [i] != ' ' && str [i] != '.')
                {
                    results = true;
                    break;
                }

                i++;
            }
            return results;
        }

        public override string ToString ()
        {
            string str1  = "Character:     ";
            string str2  = "ParenLevel:    ";
            string str3  = "BktLevel:      ";
            string str4  = "QuoteLevel:    ";
            string str4a = "NestingLevel:  ";
            string str5  = "OpenParen:     ";
            string str6  = "CloseParen:    ";
            string str7  = "OpenBrkt:      ";  
            string str8  = "CloseBrkt:     ";

            string str9  = "Quote:         ";
            string str9a = "OpenQuote:     ";
            string str9b = "CloseQuote:    ";

            string str10 = "EqualSign:     ";

            string str11 = "Level 0 Space  ";

            string str12 = "Alpha:         ";
            string str13 = "Number:        ";
            string str14 = "Decimal:       ";
            string str15 = "Transpose:     ";

            string str16 = "Minus:         ";
         // string str17 = "Unary Op :    ";
            string str18 = "Exponent:      ";
            string str19 = "Colon:         ";
            string str20 = "Semicolon:     ";
         // string str21 = "Level0Semi:    ";
            string str22 = "Comma:         ";
            string str23 = "Operator:      ";
            string str24 = "TwoCharOp:     ";

            foreach (AnnotatedChar ac in annotatedChars)
            {
                str1  += ac.Character;
                str2  += ac.ParenLevel   == 0 ? "." : ac.ParenLevel.ToString ();
                str3  += ac.BracketLevel == 0 ? "." : ac.BracketLevel.ToString ();
                str4  += ac.QuoteLevel   == 0 ? "." : ac.QuoteLevel.ToString ();
                str4a += ac.NestingLevel == 0 ? "." : ac.NestingLevel.ToString ();

                str5  += ac.IsOpenParen    ? "1" : ".";
                str6  += ac.IsCloseParen   ? "1" : ".";
                str7  += ac.IsOpenBracket  ? "1" : ".";
                str8  += ac.IsCloseBracket ? "1" : ".";
                str9  += ac.IsQuote        ? "1" : ".";
                str9a += ac.IsOpenQuote    ? "1" : ".";
                str9b += ac.IsCloseQuote   ? "1" : ".";

                str10 += ac.IsEqualSign   ? "1" : ".";

                str11 += ac.IsWhitespace && ac.NestingLevel == 0  ? "1" : ".";

                str12 += ac.IsAlpha       ? "1" : ".";
                str13 += ac.IsNumber      ? "1" : ".";
                str14 += ac.IsDecimal     ? "1" : ".";
                str15 += ac.IsTranspose   ? "1" : ".";

                str16 += ac.IsMinus       ? "1" : ".";
             //   str17 += ac.IsUnaryOp     ? "1" : ".";
                //str18 += ac.IsExponent    ? "1" : ".";
                //str19 += ac.IsColon       ? "1" : ".";
                str20 += ac.IsSemicolon   ? "1" : ".";
                //str21 += ac.IsComma       ? "1" : ".";
                str23 += ac.IsOperator    ? "1" : ".";
                str24 += ac.IsTwoCharOp   ? "1" : ".";
            }

            string str = str1;

            if (NotAllDots (str2))  str += '\n' + str2; 
            if (NotAllDots (str3))  str += '\n' + str3;
            if (NotAllDots (str4))  str += '\n' + str4;
            if (NotAllDots (str4a)) str += '\n' + str4a;

            if (str5.Contains ("1")) str += '\n' + str5;
            if (str6.Contains ("1")) str += '\n' + str6;
            if (str7.Contains ("1")) str += '\n' + str7;
            if (str8.Contains ("1")) str += '\n' + str8;
            if (str9.Contains ("1")) str += '\n' + str9;
            if (str9a.Contains ("1")) str += '\n' + str9a;
            if (str9b.Contains ("1")) str += '\n' + str9b;

            if (str10.Contains ("1")) str += '\n' + str10;
            if (str11.Contains ("1")) str += '\n' + str11;
            if (str12.Contains ("1")) str += '\n' + str12;
            if (str13.Contains ("1")) str += '\n' + str13;
            if (str14.Contains ("1")) str += '\n' + str14;
            if (str15.Contains ("1")) str += '\n' + str15;
            if (str16.Contains ("1")) str += '\n' + str16;
            if (str18.Contains ("1")) str += '\n' + str18;
            if (str19.Contains ("1")) str += '\n' + str19;
            if (str20.Contains ("1")) str += '\n' + str20;
            if (str22.Contains ("1")) str += '\n' + str22;
            if (str23.Contains ("1")) str += '\n' + str23;
            if (str24.Contains ("1")) str += '\n' + str24;

            str += "\n" + "AlphanumericOnly = " + AlphanumericOnly.ToString ();
            str += "\n" + "FirstWord:         " + FirstWord;
            str += "\n" + "FollowingWords:    " + ArgumentString;
            str += "\n" + "SuppressOutput   = " + SuppressOutput.ToString ();
            str += "\n" + "IsCompound       = " + IsCompound.ToString ();

            return str;
        }
    }
}
