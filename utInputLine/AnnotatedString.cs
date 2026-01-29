
/*
    AnnotatedString - 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    [Serializable]
    public class AnnotatedString
    {
        // private members
        private List<AnnotatedChar> annotatedChars;

        // public access properties
        internal string Raw
        {
            get
            {
                string str = "";

                for (int i = 0; i<annotatedChars.Count; i++)
                    str += annotatedChars [i].Character;

                return str;
            }
        }

        public int Count {get {return annotatedChars.Count;}}

        //*******************************************************************
        //
        // ctors
        //
        internal AnnotatedString (List<AnnotatedChar> src)
        {
            annotatedChars = src;
        }



        internal AnnotatedString (string text)
        {
            annotatedChars = new List<AnnotatedChar> (text.Length);

            // note the locations of characters we may want to modify on pass 2
            List<int> digits       = new List<int> ();
            List<int> decimals     = new List<int> ();
            List<int> exponentials = new List<int> (); // E or e
            List<int> operators    = new List<int> ();

            // pass one
            annotatedChars.Add (new AnnotatedChar (text [0])); 

            for (int i=1; i<text.Length; i++)
            {
                AnnotatedChar nextAC = new AnnotatedChar (annotatedChars [i-1], text [i]);
                if (nextAC.IsNumber)      digits.Add (i);   
                if (nextAC.IsDecimal)     decimals.Add (i); // decimals will also be in "operator" list
                if (nextAC.IsExponential) exponentials.Add (i);
                if (nextAC.IsOperator)    operators.Add (i);

                annotatedChars.Add (nextAC);
            }

            //*************************************************************************************
            //
            // pass two
            //
            //*********************************************************************

            // look for digits inside an alphanumeric (e.g. variable name A12).
            // Set their override to "letter"

            foreach (int i in digits)
            {
                int before = i - 1;

                if (before >= 0)
                    if (annotatedChars [before].IsAlpha && annotatedChars [before].IsExponential == false)
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsLetter;
            }

            // combine decimal point with number (e.g. .123 or 123.456)
            foreach (int i in decimals)
            {
                int before = i - 1;

                if (before >= 0)
                {
                    if (annotatedChars [before].IsNumber)
                    {
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
                        break;
                    }
                }

                int after = i + 1;

                if (after < Count)
                {
                    if (annotatedChars [after].IsNumber)
                    {
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsNumber;
                        break;
                    }
                }
            }

            //*********************************************************************

            // look for exponentials
            foreach (int i in exponentials)
            {
                int before = i - 1;
                int after = i + 1;

                if (before >= 0 && after < Count)
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
            //   two char operators
            //   transpose
            //   supress output (trailing semicolon)
            foreach (int i in operators)
            {
                int before = i - 1;
                int after = i + 1;

                if (before >= 0 && after < annotatedChars.Count)
                {
                    // two-char operators
                    AnnotatedChar bb = annotatedChars [i];
                    AnnotatedChar cc = annotatedChars [after];

                    if (annotatedChars [i].IsOperator && annotatedChars [after].IsOperator)
                    {
                        string str = annotatedChars [i].Character.ToString ();
                        str += annotatedChars [after].Character;

                        if (AnnotatedChar.IsTwoCharOpStr (str))
                        {
                            annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
                            annotatedChars [after].OverrideType = AnnotatedChar.ContextType.IsTwoCharOperator;
                        }
                    }
                }

                // transpose
                if (before >= 0)
                {
                    if (annotatedChars [i].IsQuote && annotatedChars [before].CanPreceedTranspose)
                        annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsTranspose;
                }

                // supress output
                if (annotatedChars [i].IsSemicolon && annotatedChars [i].NestingLevel == 0)
                {
                    annotatedChars [i].OverrideType = AnnotatedChar.ContextType.IsSupressOutput;
                }
            }
        }

        //*******************************************************************

        internal AnnotatedString (char Character)
        {
            annotatedChars = new List<AnnotatedChar> (10)
            {
                new AnnotatedChar (Character)
            };
        }

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
                throw new Exception ("Exception: " + ex.Message);
                //Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        //*******************************************************************

        // Copy constructor

        private AnnotatedString (AnnotatedString source)
        {
            annotatedChars = new List<AnnotatedChar> (source.Count);

            try
            {
                for (int i = 0; i<source.Count; i++)
                    annotatedChars.Add (source [i]);
            }

            catch (Exception ex)
            {
                throw new Exception ("Exception: " + ex.Message);
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
            for (int i=0; i<astr.Count; i++)
                annotatedChars.Add (astr [i]);
        }

        //*******************************************************************
        //
        // Add outer parenthesis
        //

        internal AnnotatedString AddOuterParens ()
        {
            List<AnnotatedChar> newChars = new List<AnnotatedChar> (Count + 2);

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

            return new AnnotatedString (newChars);
        }

        //internal void AddOuterParens ()
        //{
        //    // increment paren nesting for all characters
        //    for (int i=0; i<annotatedChars.Count; i++)
        //        annotatedChars [i].ParenLevel++;

        //    // new initial character
        //    AnnotatedChar c1 = new AnnotatedChar ('(');

        //    // if the previous first char raised a nesting level, we need to undo that for new first char
        //    c1.ParenLevel   = (sbyte) (annotatedChars [0].IsOpenParen   ? annotatedChars [0].ParenLevel - 1   : annotatedChars [0].ParenLevel);
        //    c1.BracketLevel = (sbyte) (annotatedChars [0].IsOpenBracket ? annotatedChars [0].BracketLevel - 1 : annotatedChars [0].BracketLevel);
        //    c1.QuoteLevel   = (sbyte) (annotatedChars [0].IsQuote       ? annotatedChars [0].QuoteLevel - 1   : annotatedChars [0].QuoteLevel);

        //    annotatedChars.Insert (0, c1);

        //    // new final close paren
        //    AnnotatedChar c2 = new AnnotatedChar (annotatedChars [annotatedChars.Count - 1], ')');
        //    annotatedChars.Add (c2);

        //}
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

                throw new IndexOutOfRangeException ("Index is out of range in AnnotatedString get.");
            }

            set
            {
                if (index >= 0 && index < annotatedChars.Count)
                    annotatedChars [index] = value;

                else
                    throw new IndexOutOfRangeException ("Index is out of range in AnnotatedString set.");
            }
        }

        //*******************************************************************
        //
        // Operator overloading
        //
        //public static AnnotatedString operator + (AnnotatedString left, char right)
        //{
        //    return new AnnotatedString (left.Text + right);
        //}

        //public static AnnotatedString operator + (AnnotatedString left, AnnotatedChar right)
        //{
        //    AnnotatedString AS = new AnnotatedString (left.Text + right);

        //    return new AnnotatedString (left.Text + right);
        //}

        //*******************************************************************
        //
        // Remove
        //
        //public AnnotatedString Remove (int startIndex, int count)
        //{
        //    return new AnnotatedString (annotatedChars, startIndex, count);
        //}

        //*******************************************************************
        //
        // SplitAtLevel0Semicolon - split one string into several. break at semicolons at nesting level == 0
        //
        // Split lines like:
        // a = 1; b = 2; c = 3;
        // into individual statements, while leaving lines like:
        // a = [1 ; 2 ; 4];
        // as a single statement

        internal List<AnnotatedString> SplitAtLevel0Semicolon ()
        {
            try
            { 
                // indexes where semicolons at nesting level 0 found
                List<int> indices = new List<int> ();

                for (int i=0; i<annotatedChars.Count; i++)
                    if (annotatedChars [i].Character == ';' && annotatedChars [i].NestingLevel == 0)
                        indices.Add (i);

                bool indicesEmpty = indices.Count == 0;
                bool lastCharNotSemi = annotatedChars [annotatedChars.Count-1].Character != ';';

                if (indicesEmpty || lastCharNotSemi)
                    indices.Add (annotatedChars.Count - 1);

                List<AnnotatedString> nlStrings = new List<AnnotatedString> ();

                int startIndex = 0;
                int endIndex;

                for (int i=0; i<indices.Count; i++)
                {
                    // skip any space between statements in a compound statement
                    if (annotatedChars [startIndex].Character == ' ') startIndex++;

                    endIndex = indices [i];
                    nlStrings.Add (new AnnotatedString (annotatedChars, startIndex, endIndex - startIndex + 1));
                    startIndex = endIndex + 1;
                }
            
                return nlStrings;
            }
    
            catch (Exception ex)
            {
                throw new Exception ("Exception in SplitAtLevel0Semicolon: " + ex.Message);
            }
        }

        //****************************************************************************************
        //
        // ToString ()
        //
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
            string str11 = "Whitespace:    ";
            string str12 = "Alpha:         ";
            string str13 = "Number:        ";
            string str14 = "Decimal:       ";
            string str15 = "Transpose:     ";

         // string str16 = "Binary Op:    ";
         // string str17 = "Unary Op :    ";
            string str18 = "Exponent:      ";
            string str19 = "Colon:         ";
            string str20 = "Semicolon:     ";
            string str21 = "Comma:         ";
            string str22 = "Operator:      ";
            string str23 = "TwoCharOp:     ";
            string str24 = "SupressOutput: ";

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

            //    str10 += ac.IsEqualSign   ? "1" : ".";
           //     str11 += ac.IsWhitespace  ? "1" : ".";
                str12 += ac.IsAlpha       ? "1" : ".";
                str13 += ac.IsNumber      ? "1" : ".";
                str14 += ac.IsDecimal     ? "1" : ".";
                str15 += ac.IsTranspose   ? "1" : ".";

             //   str16 += ac.IsBinaryOp    ? "1" : ".";
             //   str17 += ac.IsUnaryOp     ? "1" : ".";
                //str18 += ac.IsExponent    ? "1" : ".";
                //str19 += ac.IsColon       ? "1" : ".";
                //str20 += ac.IsSemicolon   ? "1" : ".";
                //str21 += ac.IsComma       ? "1" : ".";
                str22 += ac.IsOperator    ? "1" : ".";
                str23 += ac.IsTwoCharOp   ? "1" : ".";
                str24 += ac.IsSupress     ? "1" : ".";
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
         //   if (str11.Contains ("1")) str += '\n' + str11;
            if (str12.Contains ("1")) str += '\n' + str12;
            if (str13.Contains ("1")) str += '\n' + str13;
            if (str14.Contains ("1")) str += '\n' + str14;
            if (str15.Contains ("1")) str += '\n' + str15;
            if (str18.Contains ("1")) str += '\n' + str18;
            if (str19.Contains ("1")) str += '\n' + str19;
            if (str20.Contains ("1")) str += '\n' + str20;
            if (str21.Contains ("1")) str += '\n' + str21;
            if (str22.Contains ("1")) str += '\n' + str22;
            if (str23.Contains ("1")) str += '\n' + str23;
            if (str24.Contains ("1")) str += '\n' + str24;
            
            return str;
        }
    }
}
