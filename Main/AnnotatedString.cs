
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
        public int  Length         {get {return CharacterCount;}}
        public bool IsEmpty {get {return CharacterCount == 0;}}


        // private members

        // white spaces outside of any brackets, parens or quotes
        //    - used to separate input text line into "words"
        private readonly List<int> level0Spaces  = new List<int> (); 

        private readonly List<int> level0Semis  = new List<int> ();
        public List<int> Level0Semis {get {return level0Semis;}}

        // used to parse conditional and iterated blocks
        private readonly List<int> level0Commas  = new List<int> ();
        public List<int> Level0Commas {get {return level0Commas;}}



        // Requires either:
        //   at least 1 level 0 semi, not the last character
        //   at least 1 level 0 comma, not the last character
        public bool IsCompound 
        {
            get 
            {   
                bool t1 = level0Semis.Count  > 0 && level0Semis  [0] != CharacterCount - 1;
                bool t2 = level0Commas.Count > 0 && level0Commas [0] != CharacterCount - 1;
                return t1 || t2;
            }
        }

        private readonly List<string> level0Words = new List<string> ();
        public bool SingleWord {get {return level0Words.Count == 1;}}


        // public properties
        private bool alphanumericOnly = true;
        public bool AlphanumericOnly {get {return alphanumericOnly;} protected set {alphanumericOnly = value;}}

        private bool supressPrinting = false;
        public bool SupressPrinting {get {return supressPrinting;} protected set {supressPrinting = value;}}

        //********************************************************************************
        //
        // CheckForTrailingSemi () - remove and mark string waith SupressPrinting true
        //
        internal void CheckForTrailingSemi ()
        {
            int index = CharacterCount - 1;

            if (annotatedChars [index].IsLevel0Semicolon)
            {
                annotatedChars.RemoveAt (index);
                Level0Semis.Remove (index);
                SupressPrinting = true;
            }
        }

        //********************************************************************************
        //
        // Return first word of string
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
                return level0Words [0];
            }
        }

        //************************************************************************
        //
        // Return everything after FirstWord
        //
        //      - clear a b c % returns "a b c" (no quotes)
        //      - for a = 1:9, % returns "a = 1:9,"

        public string Arguments // all words after the first word
        {
            get
            {
                string args = "";

                for (int i = 1; i<level0Words.Count; i++)
                    args += level0Words [i] + " ";

                return args.Trim ();
            }
        }

        //*************************************************************************
        //
        // Return plain text without annotation
        //
        public string Plain 
        {
            get
            {
                string str = "";

                for (int i = 0; i<annotatedChars.Count; i++)
                    str += annotatedChars [i].Character;

                return str;
            }
        }

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

                AssignInitialTypes (trimmed);
                AdjustTypes ();

                foreach (AnnotatedChar ac in annotatedChars)
                { 
                    if (ac.IsAlphanumeric == false && ac.IsWhitespace == false)
                    { 
                        AlphanumericOnly = false;
                        break;
                    }
                }

                BreakIntoWords ();
            }

            catch (Exception ex)
            {
                throw new Exception ("Error in AnnotatedString ctor for: " + text + "\n" + ex.Message);
            }
        }

        internal AnnotatedString (AnnotatedChar ac)
        {
            Append (ac);
        }

        internal void Append (AnnotatedChar ac)
        {
            annotatedChars.Add (ac);

            if (ac.IsWhitespace == true  && ac.NestingLevel == 0) level0Spaces.Add (annotatedChars.Count - 1);
            if (ac.IsSemicolon  == true  && ac.NestingLevel == 0) level0Semis.Add  (annotatedChars.Count - 1);

            if (ac.IsAlphanumeric == false) AlphanumericOnly = false;
        }

        internal void Append (char ch)
        {
            AnnotatedChar ac = CharacterCount == 0 ? new AnnotatedChar (ch)
                                                   : new AnnotatedChar (annotatedChars [CharacterCount - 1], ch);
            Append (ac);
        }

        //*************************************************************************

        // during pass1 note the locations of characters we may want to modify during subsequent processing
        private readonly List<int> digits       = new List<int> ();
        private readonly List<int> decimals     = new List<int> ();
        private readonly List<int> exponentials = new List<int> (); // E or e
        private readonly List<int> operators    = new List<int> ();
        private readonly List<int> quotes       = new List<int> ();

        //*************************************************************************

        private void AssignInitialTypes (string text)
        { 
            AnnotatedChar firstAC = new AnnotatedChar (text [0]);
            annotatedChars.Add (firstAC);

            for (int i=1; i<text.Length; i++)
            {
                AnnotatedChar nextAC = new AnnotatedChar (annotatedChars [i-1], text [i]);
                annotatedChars.Add (nextAC);
            }

            for (int i=0; i<text.Length; i++)
            { 
                if (annotatedChars [i].IsNumber)      digits.Add       (i);   
                if (annotatedChars [i].IsQuote)       quotes.Add       (i);
                if (annotatedChars [i].IsDecimal)     decimals.Add     (i);
                if (annotatedChars [i].IsExponential) exponentials.Add (i);
                if (annotatedChars [i].IsOperator)    operators.Add    (i);

                if (annotatedChars [i].IsWhitespace == true  && annotatedChars [i].NestingLevel == 0) level0Spaces.Add (i);
                if (annotatedChars [i].IsSemicolon  == true  && annotatedChars [i].NestingLevel == 0) level0Semis.Add  (i);
                if (annotatedChars [i].IsComma      == true  && annotatedChars [i].NestingLevel == 0) level0Commas.Add (i);

                /*
                if (annotatedChars [i].IsAlphanumeric == false) 
                    AlphanumericOnly = false;
                */
            }
        }

        //*************************************************************************

        // AdjustTypes - possible change a character's type based on context 

        private void AdjustTypes ()
        {
            // in each section keep a record of any characters whose type is changed. Use
            // this to remove them from lists they should no longer be on
            List<int> changedType = new List<int> ();

            //***********************************************************************************

            // identify the types of any quotes, Open, Close, etc.
            if (quotes.Count > 0)
            { 
                bool inString = false;
                List<int> openQuotes = new List<int> ();
                List<int> closeQuotes = new List<int> ();

                foreach (int i in quotes)
                {
                    if (i == 0)
                    {
                        annotatedChars [i].thisCharType = AnnotatedChar.ACType.OpenQuote;
                        openQuotes.Add (i);
                        inString = true;
                    }

                    else if (i > 0)
                    {
                        int before = i - 1;

                        if (annotatedChars [before].thisCharType == AnnotatedChar.ACType.Escape)
                            annotatedChars [i].thisCharType = AnnotatedChar.ACType.EscapedQuote;

                        else if (inString == false && annotatedChars [before].CanPreceedString == false)
                            annotatedChars [i].thisCharType = AnnotatedChar.ACType.Transpose;

                        else if (inString == false)
                        { 
                            annotatedChars [i].thisCharType = AnnotatedChar.ACType.OpenQuote;
                            openQuotes.Add (i);
                            inString = true;
                        }

                        else 
                        { 
                            annotatedChars [i].thisCharType = AnnotatedChar.ACType.CloseQuote;
                            closeQuotes.Add (i);
                            inString = false;
                        }
                    }
                }




                // Mark chars between OpenQuotes and CloseQuotes as part of a string and remove their
                // indices from the lists, e.g. digits, operators, etc.
                //if (openQuotes.Count != closeQuotes.Count)
                //    throw new Exception ("Mismatched quote: " + Plain);

                //if (openQuotes.Count != closeQuotes.Count)
                //    return;






                changedType.Clear ();

                for (int i=0; i<openQuotes.Count; i++)
                {
                    int first = openQuotes [i];
                    int last  = closeQuotes [i];

                    for (int j=first; j<=last; j++)
                        changedType.Add (j);
                }

                // for any characters now marked as String, remove their index from other index lists
                foreach (int i in changedType)
                {
                    annotatedChars [i].thisCharType = AnnotatedChar.ACType.String;
                    digits.Remove (i);
                    quotes.Remove (i);
                    decimals.Remove (i);
                    exponentials.Remove (i);
                    operators.Remove (i);
                    level0Spaces.Remove (i);
                    level0Semis.Remove (i);
                    level0Commas.Remove (i);
                }
            }

            //*********************************************************************

            // look for digits that are part of a variable name, e.g. A12;
            // change their type to Letter
            
            //changedType.Clear ();

            foreach (int i in digits)
            {
                int before = i - 1;

                if (before >= 0)
                { 
                    if (annotatedChars [before].thisCharType == AnnotatedChar.ACType.Alphanumeric && annotatedChars [before].IsExponential == false)
                    { 
                        annotatedChars [i].thisCharType = AnnotatedChar.ACType.Alphanumeric;
                      //  changedType.Add (i); 
                    }
                }
            }

            digits.Clear ();
            //foreach (int i in changedType)
            //    digits.Remove (i);

            //*********************************************************************

            // combine decimal point with number (e.g. .123 or 123.456)
            // change its type to Number
            changedType.Clear ();

            foreach (int i in decimals)
            {
                int before = i - 1;
                int after = i + 1;

                if (before >= 0)
                {
                    if (annotatedChars [before].thisCharType == AnnotatedChar.ACType.Number)
                    {
                        annotatedChars [i].thisCharType = AnnotatedChar.ACType.Number;
                        changedType.Add (i);
                    }
                }

                if (after < CharacterCount)
                {
                    if (annotatedChars [after].thisCharType == AnnotatedChar.ACType.Number)
                    {
                        annotatedChars [i].thisCharType = AnnotatedChar.ACType.Number;
                        changedType.Add (i);
                    }
                }
            }

            foreach (int i in changedType)
                decimals.Remove (i);

            //*********************************************************************

            // mark leading +/- with number (e.g. -123.456) as numeric
            changedType.Clear ();

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
                            if (annotatedChars [before].IsEqualSign) { beforeTest = true; break; }
                            if (annotatedChars [before].IsOperator) { beforeTest = true; break; }
                            if (annotatedChars [before].IsExponential) { beforeTest = true; break; }
                            if (annotatedChars [before].IsAlphanumeric) { break; }
                            if (annotatedChars [before].IsNumber) { break; }
                        }

                    if (beforeTest == true)
                    {
                        int after = i + 1;

                        if (after < CharacterCount)
                        {
                            if (annotatedChars [after].IsNumber)
                            {
                                annotatedChars [i].thisCharType = AnnotatedChar.ACType.Number;
                                changedType.Add (i);
                            }
                        }
                    }
                }
            }

            foreach (int i in changedType)
                operators.Remove (i);

            //*********************************************************************

            // look for exponentials. Mark the "E" as a number
            //changedType.Clear ();

            foreach (int i in exponentials)
            {
                int before = i - 1;
                int after = i + 1;

                if (before >= 0 && after < CharacterCount)
                {
                    if (annotatedChars [before].IsNumber && (annotatedChars [after].IsNumber || annotatedChars [after].IsPlusMinus))
                    {
                        annotatedChars [i].thisCharType = AnnotatedChar.ACType.Number;

                        if (annotatedChars [after].IsPlusMinus)
                        {
                            annotatedChars [after].thisCharType = AnnotatedChar.ACType.Number;
                          //  changedType.Add (i);
                        }
                    }
                }
            }

            exponentials.Clear (); // any left are just letter E with no special meaning
            //foreach (int i in changedType)
            //    exponentials.Remove (i);

            //*********************************************************************

            // Two-char operators:
            //    Z = x .* C;
            //    A >= B

            changedType.Clear ();

            // look for decimal followed by operator
            foreach (int i in decimals)
            {
                if (operators.Contains (i + 1))
                {
                    annotatedChars [i].thisCharType = AnnotatedChar.ACType.TwoCharOperator;
                    annotatedChars [i+1].thisCharType = AnnotatedChar.ACType.TwoCharOperator;
                    changedType.Add (i);
                    changedType.Add (i+1);
                }
            }

            foreach (int i in changedType)
            {
                decimals.Remove (i);
                operators.Remove (i);
            }

            // look for 2 consecutive operators
            changedType.Clear ();

            foreach (int i in operators)
            {   
                if (operators.Contains (i+1))
                {
                    annotatedChars [i].thisCharType   = AnnotatedChar.ACType.TwoCharOperator;
                    annotatedChars [i+1].thisCharType = AnnotatedChar.ACType.TwoCharOperator;
                    changedType.Add (i);
                    changedType.Add (i+1);
                }
            }

            foreach (int i in changedType)
                operators.Remove (i);
        }

        //*************************************************************************

        private void BreakIntoWords ()
        {
            level0Words.Clear ();

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

        //internal static AnnotatedString AddOuterBrackets (AnnotatedString src)
        //{
        //    string final = "[" + src.Plain + "]";
        //    return new AnnotatedString (final);


        //    //List<AnnotatedChar> newChars = new List<AnnotatedChar> (CharacterCount + 2);

        //    //foreach (AnnotatedChar ac in annotatedChars)
        //    //{
        //    //    AnnotatedChar newChar = ac;
        //    //    newChar.BracketLevel++;
        //    //    newChars.Add (newChar); 
        //    //}

        //    //// new initial character
        //    //AnnotatedChar c1 = new AnnotatedChar ('[');

        //    //// if the previous first char raised a nesting level, we need to undo that for new first char
        //    //c1.ParenLevel   = (sbyte) (annotatedChars [0].IsOpenParen   ? annotatedChars [0].ParenLevel - 1   : annotatedChars [0].ParenLevel);
        //    //c1.BracketLevel = (sbyte) (annotatedChars [0].IsOpenBracket ? annotatedChars [0].BracketLevel - 1 : annotatedChars [0].BracketLevel);

        //    //newChars.Insert (0, c1);

        //    // new final close paren
        //  //  AnnotatedChar c2 = new AnnotatedChar (annotatedChars [annotatedChars.Count - 1], ']');
        //  //  newChars.Add (c2);

        //  ////  return new AnnotatedString (newChars);

        //  //  annotatedChars = newChars;
        //  //  return this;
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

        // Helper method NotAllDots - test for text line with something other than '.' after initial colon

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

            string str1  = "ParenLevel:    ";
            string str2  = "BracketsLevel: ";
            string str4  = "NestingLevel:  ";

            string str5  = "OpenParen:     ";
            string str6  = "CloseParen:    ";
            string str7  = "OpenBrkt:      ";  
            string str8  = "CloseBrkt:     ";
            string str9  = "Quote:         ";
            string str10 = "OpenQuote:     ";
            string str11 = "CloseQuote:    ";
            string str21 = "EscapedQuote:  ";
            string str12 = "Number:        ";
            string str13 = "Alphanumeric:  ";
            string str14 = "Operator:      ";
            string str15 = "Decimal:       ";
            string str16 = "Semicolon:     ";
            string str17 = "Colon:         ";
            string str18 = "Escape:        ";
            string str19 = "Percent:       ";
            //string str10 = "Exponent:      ";
            string str20 = "Transpose:     ";
            string str22 = "StringChar:    ";
            string str23 = "TwoCharOp:     ";
            string str24 = "Comma:         ";

            foreach (AnnotatedChar ac in annotatedChars)
            {
                str0 += ac.Character;

                str1 += ac.ParenLevel   == 0 ? "." : ac.ParenLevel.ToString ();
                str2 += ac.BracketLevel == 0 ? "." : ac.BracketLevel.ToString ();
                str4 += ac.NestingLevel == 0 ? "." : ac.NestingLevel.ToString ();

                str5 += ac.thisCharType == AnnotatedChar.ACType.OpenParen    ? "1" : ".";
                str6 += ac.thisCharType == AnnotatedChar.ACType.CloseParen   ? "1" : ".";
                str7 += ac.thisCharType == AnnotatedChar.ACType.OpenBracket  ? "1" : ".";
                str8 += ac.thisCharType == AnnotatedChar.ACType.CloseBracket ? "1" : ".";
                str9 += ac.thisCharType == AnnotatedChar.ACType.Quote        ? "1" : ".";

                str10 += ac.thisCharType == AnnotatedChar.ACType.OpenQuote    ? "1" : ".";
                str11 += ac.thisCharType == AnnotatedChar.ACType.CloseQuote   ? "1" : ".";
                str21 += ac.thisCharType == AnnotatedChar.ACType.EscapedQuote ? "1" : ".";

                str12 += ac.thisCharType == AnnotatedChar.ACType.Number       ? "1" : ".";
                str13 += ac.thisCharType == AnnotatedChar.ACType.Alphanumeric ? "1" : ".";
                str14 += ac.thisCharType == AnnotatedChar.ACType.Operator     ? "1" : ".";
                str15 += ac.thisCharType == AnnotatedChar.ACType.DecimalPoint ? "1" : ".";
                str16 += ac.thisCharType == AnnotatedChar.ACType.Semicolon    ? "1" : ".";
                str17 += ac.thisCharType == AnnotatedChar.ACType.Colon        ? "1" : ".";
                str18 += ac.thisCharType == AnnotatedChar.ACType.Escape       ? "1" : ".";
                str19 += ac.thisCharType == AnnotatedChar.ACType.Percent      ? "1" : ".";
                str20 += ac.thisCharType == AnnotatedChar.ACType.Transpose    ? "1" : ".";
                str22 += ac.thisCharType == AnnotatedChar.ACType.String       ? "1" : ".";
                
                str23 += ac.thisCharType == AnnotatedChar.ACType.TwoCharOperator ? "1" : ".";
                str24 += ac.thisCharType == AnnotatedChar.ACType.Comma           ? "1" : ".";
            }

            string str = str0;

            if (NotAllDots (str1)) str += '\n' + str1;
            if (NotAllDots (str2)) str += '\n' + str2;
         // if (NotAllDots (str3)) str += '\n' + str3;
            if (NotAllDots (str4)) str += '\n' + str4;

            if (str5.Contains ("1")) str += '\n' + str5;
            if (str6.Contains ("1")) str += '\n' + str6;
            if (str7.Contains ("1")) str += '\n' + str7;
            if (str8.Contains ("1")) str += '\n' + str8;
            if (str9.Contains ("1")) str += '\n' + str9;
            if (str10.Contains ("1")) str += '\n' + str10;
            if (str11.Contains ("1")) str += '\n' + str11;
            if (str21.Contains ("1")) str += '\n' + str21;
            if (str12.Contains ("1")) str += '\n' + str12;
            if (str13.Contains ("1")) str += '\n' + str13;
            if (str14.Contains ("1")) str += '\n' + str14;
            if (str15.Contains ("1")) str += '\n' + str15;
            if (str16.Contains ("1")) str += '\n' + str16;
            if (str17.Contains ("1")) str += '\n' + str17;
            if (str18.Contains ("1")) str += '\n' + str18;
            if (str19.Contains ("1")) str += '\n' + str19;
            if (str20.Contains ("1")) str += '\n' + str20;
            if (str22.Contains ("1")) str += '\n' + str22;
            if (str23.Contains ("1")) str += '\n' + str23;
            if (str24.Contains ("1")) str += '\n' + str24;

            str += "\n" + "SupressPrinting:  " + SupressPrinting.ToString ();
            str += "\n" + "IsCompound:       " + IsCompound.ToString ();
            str += "\n" + "AlphanumericOnly: " + AlphanumericOnly.ToString ();    
            
            //if (AlphanumericOnly)
            //{ 
            //    str += "\n" + "Nesting level 0 words:";
            //    BreakIntoWords ();
            //    foreach (string oneWord in level0Words)
            //        str += "\n   " + oneWord;
            //}

            str += "\n" + "FirstWord: " + FirstWord;
            str += "\n" + "Arguments: ";
            str += Arguments;
            //foreach (string astr in Arguments) str += " " + astr;

            if (digits.Count > 0)       {str += "\nDigits      : "; foreach (int i in digits) str += i + ", ";}
            if (quotes.Count > 0)       {str += "\nQuotes      : "; foreach (int i in quotes) str += i + ", ";}
            if (decimals.Count > 0)     {str += "\nDecimals    : "; foreach (int i in decimals) str += i + ", ";}
            if (exponentials.Count > 0) {str += "\nExponentials: "; foreach (int i in exponentials) str += i + ", ";}
            if (operators.Count > 0)    {str += "\nOperators   : "; foreach (int i in operators) str += i + ", ";}
            if (level0Spaces.Count > 0) {str += "\nlevel0Spaces: "; foreach (int i in level0Spaces) str += i + ", ";}
            if (level0Semis.Count > 0)  {str += "\nlevel0Semis : "; foreach (int i in level0Semis) str += i + ", ";}
            if (level0Commas.Count > 0) {str += "\nlevel0Commas: "; foreach (int i in level0Commas) str += i + ", ";}

            return str;
        }
    }
}
