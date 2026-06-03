
/*
    AnnotatedString - list of AnnotatedCharacters
*/

using System;
using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedString : NestedString
    {
        // private members
        private readonly List<AnnotatedChar> annotatedChars = new List<AnnotatedChar> ();

        //*************************************************************************
        //
        // ctors
        //

        internal AnnotatedString (string text) : base (text)
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

        //public int CharacterCount {get {return annotatedChars.Count;}}
        public bool IsEmpty {get {return CharacterCount == 0;}}
        public int FinalBracketLevel {get {return annotatedChars [CharacterCount-1].BracketLevel;}}

        private bool suppressOutput = false; // set true if last char is semicolon
        public  bool SuppressOutput {get {return suppressOutput;} set {suppressOutput = value;}}

        private bool isCompound = false; // set true for stmts of the form: a = 7; b = 10; c = 12;
        public  bool IsCompound {get {return isCompound;} private set {isCompound = value;}}

        public override int CharacterCount {get {return annotatedChars.Count;}}

        //************************************************************************

        // if line ends in semicolon, remove it and set SuppressOutput = true

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

                if (nextAC.IsSemicolon)   semicolons.Add   (i);
                annotatedChars.Add (nextAC);
            }
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


        // Copy constructor

        ////private AnnotatedString (AnnotatedString source)
        ////{
        ////    annotatedChars = new List<AnnotatedChar> (source.CharacterCount);

        ////    try
        ////    {
        ////        for (int i = 0; i<source.CharacterCount; i++)
        ////            annotatedChars.Add (source [i]);
        ////    }

        ////    catch (Exception ex)
        ////    {
        ////        throw new Exception ("Exception in AnnotatedStringCtor: " + ex.Message);
        ////        //Console.WriteLine ("Exception: " + ex.Message);
        ////    }
        ////}

        //*******************************************************************
        //
        // Operator overloading
        //
        public static AnnotatedString operator + (AnnotatedString left, AnnotatedString right)
        {
            string sum = left.Plain + right.Plain;
            AnnotatedString asum = new AnnotatedString (sum);
            return asum;
        }

        //*******************************************************************
        //
        // Append
        //

        internal static AnnotatedString Append (AnnotatedString orig, char ch)
        {
            string str = orig.Plain;
            str += ch;
            return new AnnotatedString (str);
        }

        internal static AnnotatedString Append (AnnotatedString orig, string added)
        {
            string str = orig.Plain;
            str += added;
            return new AnnotatedString (str);
        }

        internal void Append (AnnotatedString astr)
        {
            throw new NotImplementedException ("Not implemented");
            //for (int i = 0; i<astr.CharacterCount; i++)
            //    annotatedChars.Add (astr [i]);
        }

        //*******************************************************************
        //
        // Return substring with leading and trailing spaces removed
        //
        public AnnotatedString TrimmedSubstring (int start, int count)
        {
            string sub = Plain.Substring (start, count);
            string trimmed = sub.Trim ();
            return new AnnotatedString (trimmed);
        }

        // no trimming
        public AnnotatedString Substring (int start, int count)
        {
            string sub = Plain.Substring (start, count);
            return new AnnotatedString (sub);
        }

        //*******************************************************************
        //
        // Add outer parenthesis
        //

        internal AnnotatedString AddOuterParens ()
        {
            throw new NotImplementedException ("Not implemented");
          //  List<AnnotatedChar> newChars = new List<AnnotatedChar> (CharacterCount + 2);

          //  foreach (AnnotatedChar ac in annotatedChars)
          //  {
          //      AnnotatedChar newChar = ac;
          //      newChar.ParenLevel++;
          //      newChars.Add (newChar); 
          //  }

          //  // new initial character
          //  AnnotatedChar c1 = new AnnotatedChar ('(');

          //  // if the previous first char raised a nesting level, we need to undo that for new first char
          //  c1.ParenLevel   = (sbyte) (annotatedChars [0].IsOpenParen   ? annotatedChars [0].ParenLevel - 1   : annotatedChars [0].ParenLevel);
          //  c1.BracketLevel = (sbyte) (annotatedChars [0].IsOpenBracket ? annotatedChars [0].BracketLevel - 1 : annotatedChars [0].BracketLevel);
          //  c1.QuoteLevel   = (sbyte) (annotatedChars [0].IsQuote       ? annotatedChars [0].QuoteLevel - 1   : annotatedChars [0].QuoteLevel);

          //  newChars.Insert (0, c1);

          //  // new final close paren
          //  AnnotatedChar c2 = new AnnotatedChar (annotatedChars [annotatedChars.Count - 1], ')');
          //  newChars.Add (c2);

          ////  return new AnnotatedString (newChars);

          //  annotatedChars = newChars;
          //  return this;
        }

        //*******************************************************************
        //
        // Add outer square brackets
        //

        internal AnnotatedString AddOuterBrackets ()
        {
            throw new NotImplementedException ("Not implemented");
            //List<AnnotatedChar> newChars = new List<AnnotatedChar> (CharacterCount + 2);

            //foreach (AnnotatedChar ac in annotatedChars)
            //{
            //    AnnotatedChar newChar = ac;
            //    newChar.BracketLevel++;
            //    newChars.Add (newChar); 
            //}

            //// new initial character
            //AnnotatedChar c1 = new AnnotatedChar ('[');

            //// if the previous first char raised a nesting level, we need to undo that for new first char
            //c1.ParenLevel   = (sbyte) (annotatedChars [0].IsOpenParen   ? annotatedChars [0].ParenLevel - 1   : annotatedChars [0].ParenLevel);
            //c1.BracketLevel = (sbyte) (annotatedChars [0].IsOpenBracket ? annotatedChars [0].BracketLevel - 1 : annotatedChars [0].BracketLevel);
            //c1.QuoteLevel   = (sbyte) (annotatedChars [0].IsQuote       ? annotatedChars [0].QuoteLevel - 1   : annotatedChars [0].QuoteLevel);

            //newChars.Insert (0, c1);

            // new final close paren
          //  AnnotatedChar c2 = new AnnotatedChar (annotatedChars [annotatedChars.Count - 1], ']');
          //  newChars.Add (c2);

          ////  return new AnnotatedString (newChars);

          //  annotatedChars = newChars;
          //  return this;
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
            throw new NotImplementedException ("Not implemented");
            //int startIndex = 1;
            //int endIndex = annotatedChars.Count - 2;

            //while (annotatedChars [startIndex].IsWhitespace && startIndex < endIndex)
            //    startIndex++;

            //while (annotatedChars [endIndex].IsWhitespace && endIndex > startIndex)
            //    endIndex--;

            //return new AnnotatedString (annotatedChars, startIndex, endIndex - startIndex + 1);
        }

        //****************************************************************************************
        //
        // ToString ()
        //

        public override string ToString () 
        {
            string str = base.ToString ();

            string str1 = "OpenQuote:     ";
            string str2 = "CloseQuote:    ";

            string str3 = "EqualSign:     ";

            string str4 = "Alpha:         ";
            string str5 = "Number:        ";
            string str6 = "Decimal:       ";
            string str7 = "Transpose:     ";

            string str8 = "Minus:         ";
         // string str17 = "Unary Op :    ";
         // string str9 = "Exponent:      ";
         // string str10 = "Colon:         ";
            string str11 = "Semicolon:     ";
         // string str21 = "Level0Semi:    ";
            string str12 = "Comma:         ";
            string str13 = "Operator:      ";
            string str14 = "TwoCharOp:     ";

            foreach (AnnotatedChar ac in annotatedChars)
            {
                str1 += ac.IsOpenQuote    ? "1" : ".";
                str2 += ac.IsCloseQuote   ? "1" : ".";
                str3 += ac.IsEqualSign   ? "1" : ".";
                str4 += ac.IsAlpha       ? "1" : ".";
                str5 += ac.IsNumber      ? "1" : ".";
                str6 += ac.IsDecimal     ? "1" : ".";
                str7 += ac.IsTranspose   ? "1" : ".";

                str8 += ac.IsMinus       ? "1" : ".";
                str11 += ac.IsSemicolon   ? "1" : ".";
                str12 += ac.IsComma       ? "1" : ".";
                str13 += ac.IsOperator    ? "1" : ".";
                str14 += ac.IsTwoCharOp   ? "1" : ".";
            }

            // if trailing ; was removed, pad strings with extra dot to make
            // them same length as NestedString prints
            if (suppressOutput)
            {
                str1  += ".";
                str2  += ".";
                str3  += ".";
                str4  += ".";
                str5  += ".";
                str6  += ".";
                str7  += ".";
                str8  += ".";
                str11 += ".";
                str12 += ".";
                str13 += ".";
                str14 += ".";
            }

            if (str1.Contains ("1")) str += '\n' + str1;
            if (str2.Contains ("1")) str += '\n' + str2;

            if (str3.Contains ("1")) str += '\n' + str3;
            //if (str11.Contains ("1")) str += '\n' + str11;
            if (str4.Contains ("1")) str += '\n' + str4;
            if (str5.Contains ("1")) str += '\n' + str5;
            if (str6.Contains ("1")) str += '\n' + str6;
            if (str7.Contains ("1")) str += '\n' + str7;
            if (str8.Contains ("1")) str += '\n' + str8;
            //if (str9.Contains ("1")) str += '\n' + str9;
            //if (str10.Contains ("1")) str += '\n' + str10;
            if (str11.Contains ("1")) str += '\n' + str11;
            if (str12.Contains ("1")) str += '\n' + str12;
            if (str13.Contains ("1")) str += '\n' + str13;
            if (str14.Contains ("1")) str += '\n' + str14;

            str += "\n";
            str += "\n" + "SuppressOutput   = " + SuppressOutput.ToString ();
            str += "\n" + "IsCompound       = " + IsCompound.ToString ();

            return str;
        }
    }
}
