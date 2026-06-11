
/*
    AnnotatedString - list of AnnotatedCharacters
*/

using System;
using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedString
    {
        // private members
        private readonly List<AnnotatedChar> annotatedChars = new List<AnnotatedChar> ();

        // public access properties
        public int  CharacterCount {get {return annotatedChars.Count;}}
        public bool IsEmpty {get {return CharacterCount == 0;}}


        // private members
        // white spaces outside of any brackets, parens or quotes
        //    - used to separate input text line into "words"
        private readonly List<int> level0Spaces  = new List<int> (); 


        private readonly List<int> level0Semis  = new List<int> ();
        public List<int> Level0Semis {get {return level0Semis;}}

        public bool IsCompound {get {return (level0Semis.Count > 0 && level0Semis [0] != CharacterCount - 1);}}

        // 
        private readonly List<string> level0Words = new List<string> ();
        public  bool SingleWord {get {return level0Words.Count == 1;}}


        // public properties
        private bool alphanumericOnly = true;
        public  bool AlphanumericOnly {get {return alphanumericOnly;}}



        //********************************************************************************
        //
        // Return first word of input string
        //
        //      Commands
        //          - clear a b c % returns "clear"
        //
        //      For block start
        //          - for a = 1:9, % return "for"
        //
        //      function declaration
        //          - function [x, y, z] =   % returns "function"
        public string FirstWord
        {
            get
            {
                if (level0Words.Count < 2)
                    return Plain;

                return level0Words [0];
            }
        }

        //************************************************************************
        //
        // Return everything after FirstWord
        //
        //      - clear a b c % returns "a b c" (no quotes)
        //      - for a = 1:9, % returns a = 1:9,

        public List<string> Arguments // all words after the first word
        {
            get
            {
                List<string> args = new List<string> ();

                for (int i = 1; i<level0Words.Count; i++)
                    args.Add (level0Words [i]);

                return args;
            }
        }





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

        public bool SuppressOutput {get; protected set;} = false;

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
                string noTabs = text.Replace ('\t', ' ');
                string trimmed = noTabs.Trim ();

                if (trimmed.Length == 0 || trimmed [0] == '%')
                    return;


                // causes problem when appending ;
                //   d = [1 ;

                //if (trimmed [trimmed.Length - 1] == ';')
                //{
                //    SuppressOutput = true;
                //    trimmed = trimmed.Remove (trimmed.Length - 1, 1);
                //}



                PassOne (trimmed);
                PassTwo ();
                Pass3 ();




                //bool t1 = annotatedChars [CharacterCount - 1].IsSemicolon;
                //bool t2 = annotatedChars [0].NestingLevel == annotatedChars [CharacterCount - 1].NestingLevel;

                //if (t1 && t2)
                //{
                //    SuppressOutput = true;
                //    annotatedChars.RemoveAt (CharacterCount - 1);

                //    if (Level0Semis [Level0Semis.Count - 1] >= CharacterCount)
                //        Level0Semis.RemoveAt (Level0Semis.Count - 1);
                //}




            }

            catch (Exception ex)
            {
                throw new Exception ("Error in AnnotatedString ctor:\n" + ex.Message);
            }
        }

        //*************************************************************************

        // during pass1 note the locations of characters we may want to modify during subsequent processing
        //private readonly List<int> digits       = new List<int> ();
        //private readonly List<int> decimals     = new List<int> ();
        //private readonly List<int> exponentials = new List<int> (); // E or e
        //private readonly List<int> operators    = new List<int> ();

        //*************************************************************************

        private void PassOne (string text)
        { 
            AnnotatedChar firstAC = new AnnotatedChar (text [0]);
            annotatedChars.Add (firstAC);

            // an all alphanumeric string must begin with a letter but subsequent characters may be letters or numbers
            if (firstAC.IsLetter == false)
                alphanumericOnly = false;

            for (int i=1; i<text.Length; i++)
            {
                AnnotatedChar nextAC = new AnnotatedChar (annotatedChars [i-1], text [i]);
                annotatedChars.Add (nextAC);

                //if (nextAC.IsNumber)      digits.Add       (i);   
                //if (nextAC.IsDecimal)     decimals.Add     (i); 
                //if (nextAC.IsExponential) exponentials.Add (i);
                //if (nextAC.IsOperator)    operators.Add (i);

                //if (nextAC.IsWhitespace0 == true  && nextAC.NestingLevel   == 0)     level0Spaces.Add  (i);
                //if (nextAC.IsSemicolon0  == true  && nextAC.NestingLevel   == 0)     level0Semis.Add  (i);
                //if (nextAC.IsWhitespace0 == false && nextAC.IsAlphanumeric == false) alphanumericOnly = false;
            }
        }

        //*************************************************************************

        private void PassTwo ()
        {
            //// look for digits that are part of a variable name, e.g. A12;
            //// change their type to Letter
            //foreach (int i in digits)
            //{
            //    int before = i - 1;

            //    if (before >= 0)
            //        if (annotatedChars [before].IsAlpha && annotatedChars [before].IsExponential == false)
            //            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsLetter;
            //}

            ////*********************************************************************

            //// combine decimal point with number (e.g. .123 or 123.456)
            //// change its type to Number
            //foreach (int i in decimals)
            //{
            //    int before = i - 1;

            //    if (before >= 0)
            //    {
            //        if (annotatedChars [before].IsNumber)
            //        {
            //            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
            //        }
            //    }

            //    int after = i + 1;

            //    if (after < CharacterCount)
            //    {
            //        if (annotatedChars [after].IsNumber)
            //        {
            //            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
            //        }
            //    }
            //}

            ////*********************************************************************

            //// mark leading +/- with number (e.g. -123.456) as numeric

            //foreach (int i in operators)
            //{
            //    if (annotatedChars [i].IsPlusMinus)
            //    { 
            //        bool beforeTest = false; // set true if char before the +/- indicates
            //                                 // the +/- is a unary op

            //        if (i == 0)
            //            beforeTest = true;

            //        else
            //            for (int before = i - 1; before >= 0; before--)
            //            {
            //                if (annotatedChars [before].IsEqualSign) {beforeTest = true; break;}
            //                if (annotatedChars [before].IsOperator) {beforeTest = true; break;}
            //                if (annotatedChars [before].IsAlpha) {break;}
            //                if (annotatedChars [before].IsNumber) {break;}
            //            }

            //        if (beforeTest == true)
            //        { 
            //            int after = i + 1;

            //            if (after < CharacterCount)
            //            {
            //                if (annotatedChars [after].IsNumber)
            //                {
            //                    annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
            //                }
            //            }
            //        }
            //    }
            //}

            ////*********************************************************************

            //// look for exponentials. Mark the "E" as a number
            //foreach (int i in exponentials)
            //{
            //    int before = i - 1;
            //    int after = i + 1;

            //    if (before >= 0 && after < CharacterCount)
            //    {
            //        if (annotatedChars [before].IsNumber && (annotatedChars [after].IsNumber || annotatedChars [after].IsPlusMinus))
            //        {
            //            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;

            //            if (annotatedChars [after].IsPlusMinus)
            //            {
            //                annotatedChars [after].OverrideType = AnnotatedChar.ContextType.IsNumber;
            //            }
            //        }
            //    }
            //}

            ////*********************************************************************

            //// look for:
            ////   two char operators, e.g. A >= B
            ////   transpose, A'
            //foreach (int i in operators)
            //{
            //    int before = i - 1;
            //    int after = i + 1;

            //    if (before >= 0 && after < annotatedChars.Count)
            //    {
            //        // two-char operators
            //        bool t1 = annotatedChars [i].IsOperator;// || annotatedChars [i].IsEqualSign;
            //        bool t2 = annotatedChars [after].IsOperator;// || annotatedChars [after].IsEqualSign;
            //        bool t3 = annotatedChars [before].IsDecimal;

            //        if (t1 && t2) // e.g. ">="
            //        {
            //            string str = annotatedChars [i].Character.ToString ();
            //            str += annotatedChars [after].Character;

            //            if (AnnotatedChar.IsTwoCharOpStr (str))
            //            {
            //                annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
            //                annotatedChars [after].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
            //            }
            //        }
            
            //        else if (t1 && t3) // e.g. ".*"
            //        {
            //            string str = annotatedChars [before].Character.ToString ();
            //            str += annotatedChars [i].Character;

            //            if (AnnotatedChar.IsTwoCharOpStr (str))
            //            {
            //                annotatedChars [before].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
            //                annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
            //            }
            //        }
            //    }

            //    // transpose
            //    if (before >= 0)
            //    {
            //        if (annotatedChars [i].IsQuote && annotatedChars [before].CanPreceedTranspose)
            //            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTranspose;
            //    }
            //}
        }

        //*************************************************************************

        private void Pass3 ()
        {
            // break into "words", character substrings separated by level 0 whitespaces
            int start = 0;
            int stop = 0;
            string plainCopy = Plain;

            for (int i = 0; i<level0Spaces.Count; i++)
            {
                stop = level0Spaces [i];
                string nextWord = plainCopy.Substring (start, stop - start);
                level0Words.Add (nextWord);
                start = stop + 1;
            }

            if (stop < plainCopy.Length)
                level0Words.Add (plainCopy.Substring (start, Plain.Length - start));
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




        //internal static AnnotatedString Append (AnnotatedString orig, char ch)
        //{
        //    AnnotatedChar wasLast = orig [orig.CharacterCount-1];
        //    orig.annotatedChars.Add (new AnnotatedChar (wasLast, ch));



        //}

        internal static AnnotatedString Append (AnnotatedString orig, char ch)
        {
            throw new NotImplementedException ("Append ch");
            //string str = orig.Plain;
            //str += ch;
            //return new AnnotatedString (str);
        }







        //internal static AnnotatedString Append (AnnotatedString orig, string added)
        //{
        //    string str = orig.Plain;
        //    str += added;
        //    return new AnnotatedString (str);
        //}

        internal void Append (AnnotatedString astr)
        {
            throw new NotImplementedException ("Append astr");
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

        internal static AnnotatedString AddOuterParens (AnnotatedString astr)
        {
            string str = astr.Plain;
            str = "(" + str + ")";
            return new AnnotatedString (str);

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

        internal static AnnotatedString AddOuterBrackets (AnnotatedString src)
        {
            string final = "[" + src.Plain + "]";
            return new AnnotatedString (final);


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
        public static AnnotatedString RemoveWrapper (AnnotatedString src)
        {
            string initial = src.Plain;
            bool SyntaxError = false;

            switch (initial [0])
            {
                case '[':
                    if (initial [initial.Length - 1] != ']')
                        SyntaxError = true;
                    break;

                case '(':
                    if (initial [initial.Length - 1] != ')')
                        SyntaxError = true;
                    break;

                default:
                    SyntaxError = true;
                    break;
            }

            if (SyntaxError)
                throw new Exception ("Syntax error in RemoveWrapper: " + initial);

            string final = initial.Remove (0, 1);
            final = final.Remove (final.Length - 1, 1);

            return new AnnotatedString (final.Trim ());
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
            string str0 = "Character:     ";

            string strA  = "In Parens:     ";
            string strB  = "In Brackets:   ";
            string strC  = "QuoteLevel:    ";
            string strD  = "NestingLevel:  ";

            //string str6  = "OpenParen:     ";
            //string str7  = "CloseParen:    ";
            //string str8  = "OpenBrkt:      ";  
        //    string str9  = "CloseBrkt:     ";
         //   string str10 = "Quote:         ";
        //    string str11 = "Level 0 Space  ";
        //    string str12 = "Level 0 Semi   ";

            string str1 = "OpenQuote:     ";
            string str2 = "CloseQuote:    ";

            string str3 = "EqualSign:     ";

            string str4 = "Alphanumeric:  ";
            string str5 = "Number:        ";
            //string str6 = "Decimal:       ";
            string str7 = "Transpose:     ";

            //string str8 = "Minus:         ";
            //string str9 = "Unary Op :    ";
            //string str10 = "Exponent:      ";
         // string str10 = "Colon:         ";
         // string str11 = "Semicolon:     ";
         // string str21 = "Level0Semi:    ";
       //     string str12 = "Comma:         ";
            string str13 = "Operator:      ";
            string str14 = "TwoCharOp:     ";

            foreach (AnnotatedChar ac in annotatedChars)
            {
                str0 += ac.Character;

                //strA += ac.ParenLevel   == 0 ? "." : ac.ParenLevel.ToString ();
                //strB += ac.BracketLevel == 0 ? "." : ac.BracketLevel.ToString ();
                //strC += ac.QuoteLevel   == 0 ? "." : ac.QuoteLevel.ToString ();
                //strD += ac.NestingLevel == 0 ? "." : ac.NestingLevel.ToString ();

                strA += ac.thisCharType == AnnotatedChar.ACType.InParens ? "1" : ".";
                strB += ac.thisCharType == AnnotatedChar.ACType.InBrackets ? "1" : ".";
                    
                str1 += ac.IsOpenQuote    ? "1" : ".";
                str2 += ac.IsCloseQuote   ? "1" : ".";
                //str3 += ac.IsEqualSign   ? "1" : ".";
                str4 += ac.thisCharType == AnnotatedChar.ACType.Alphanumeric ? "1" : ".";
                str5 += ac.thisCharType == AnnotatedChar.ACType.Number ? "1" : ".";
                //str6 += ac.IsDecimal     ? "1" : ".";
                //str7 += ac.IsTranspose   ? "1" : ".";
                //str8 += ac.IsMinus       ? "1" : ".";

           //     str11 += ac.IsSemicolon   ? "1" : ".";
             //   str12 += ac.IsComma       ? "1" : ".";
                str13 += ac.IsOperator    ? "1" : ".";
                //str14 += ac.IsTwoCharOp   ? "1" : ".";

            }

            string str = str0;

            if (NotAllDots (strA)) str += '\n' + strA;
            if (NotAllDots (strB)) str += '\n' + strB;
            if (NotAllDots (strC)) str += '\n' + strC;
            if (NotAllDots (strD)) str += '\n' + strD;

            if (str1.Contains ("1")) str += '\n' + str1;
            if (str2.Contains ("1")) str += '\n' + str2;

            if (str3.Contains ("1")) str += '\n' + str3;
            //if (str11.Contains ("1")) str += '\n' + str11;
            if (str4.Contains ("1")) str += '\n' + str4;
            if (str5.Contains ("1")) str += '\n' + str5;
            //if (str6.Contains ("1")) str += '\n' + str6;
            if (str7.Contains ("1")) str += '\n' + str7;
            //if (str8.Contains ("1")) str += '\n' + str8;
            //if (str9.Contains ("1")) str += '\n' + str9;
            //if (str10.Contains ("1")) str += '\n' + str10;
            //if (str11.Contains ("1")) str += '\n' + str11;
          //  if (str12.Contains ("1")) str += '\n' + str12;
            if (str13.Contains ("1")) str += '\n' + str13;
            if (str14.Contains ("1")) str += '\n' + str14;

            //str += "\n" + "AlphanumericOnly:  " + AlphanumericOnly.ToString ();            

            //str += "\n" + "Nesting level 0 words:";

            //foreach (string oneWord in level0Words) 
            //    str += "\n   " + oneWord;

            //str += "\n" + "SuppressOutput: " + SuppressOutput;

            return str;
        }
    }
}
