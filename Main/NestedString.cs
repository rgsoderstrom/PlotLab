
/*
    NestedString.cs - begin parsing an input string
*/

using System;
using System.Collections.Generic;

namespace PLMain
{
    public class NestedString
    {
        // protected members
        protected readonly List<NestedChar> nestedChars = new List<NestedChar> ();
        private   readonly List<string> level0Words = new List<string> ();

        // white spaces outside of any brackets, parens or quotes
        protected readonly List<int> Level0Spaces  = new List<int> (); 

        // public properties
        private bool alphanumericOnly = true;
        public  bool AlphanumericOnly {get {return alphanumericOnly;}} // set {alphanumericOnly = value;}}

        public  bool SingleWord {get {return level0Words.Count == 1;}}

        //*************************************************************************
        //
        // ctor
        //
        public NestedString (string text)
        {
            if (text.Length == 0)
                return;

            try
            {
                NestedChar nextNC = new NestedChar (text [0]);
                nestedChars.Add (nextNC);

                // an Alphanumeric must begin with a letter but subsequent characters may be letters or numbers
                if (nextNC.IsLetter == false)
                    alphanumericOnly = false;

                for (int i=1 ; i<text.Length; i++)
                {
                    nextNC = new NestedChar (nestedChars [i-1], text [i]);
                    nestedChars.Add (nextNC);

                    if (nextNC.IsWhitespace && nextNC.NestingLevel == 0)  
                        Level0Spaces.Add  (i);

                    if (nextNC.IsWhitespace == false && nextNC.IsAlphanumeric == false)
                        alphanumericOnly = false;
                }

                //
                // break into "words", character substrings separated by level 0 whitespaces
                //
                int start = 0;
                int stop = 0;
                string plainCopy = Plain;

                for (int i=0; i<Level0Spaces.Count; i++)
                {
                    stop = Level0Spaces [i];
                    string nextWord =  plainCopy.Substring (start, stop - start);
                    level0Words.Add (nextWord);
                    start = stop + 1;
                }

                if (stop < plainCopy.Length)
                    level0Words.Add (plainCopy.Substring (start, Plain.Length - start));
            }

            catch (Exception ex)
            {
                throw new Exception ("Error in NestedString ctor:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        //********************************************************************************

        // public access properties
        public string Plain // plain text without annotation
        {
            get
            {
                string str = "";

                for (int i = 0; i<nestedChars.Count; i++)
                    str += nestedChars [i].Character;

                return str;
            }
        }

        public virtual int CharacterCount {get {return nestedChars.Count;}}

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

                for (int i=1; i<level0Words.Count; i++)
                    args.Add (level0Words [i]);

                return args;
            }
        }


        //****************************************************************************************
        //
        // ToString ()
        //

        // test for text line with something other than '.' after initial colon
        protected bool NotAllDots (string str)
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
            string str5  = "NestingLevel:  ";
            string str6  = "OpenParen:     ";
            string str7  = "CloseParen:    ";
            string str8  = "OpenBrkt:      ";  
            string str9  = "CloseBrkt:     ";
            string str10 = "Quote:         ";
            string str11 = "Level 0 Space  ";

            foreach (NestedChar ac in nestedChars)
            {
                str1 += ac.Character;
                str2 += ac.ParenLevel   == 0 ? "." : ac.ParenLevel.ToString ();
                str3 += ac.BracketLevel == 0 ? "." : ac.BracketLevel.ToString ();
                str4 += ac.QuoteLevel   == 0 ? "." : ac.QuoteLevel.ToString ();
                str5 += ac.NestingLevel == 0 ? "." : ac.NestingLevel.ToString ();

                str6  += ac.IsOpenParen    ? "1" : ".";
                str7  += ac.IsCloseParen   ? "1" : ".";
                str8  += ac.IsOpenBracket  ? "1" : ".";
                str9  += ac.IsCloseBracket ? "1" : ".";
                str10 += ac.IsQuote        ? "1" : ".";

                str11 += ac.IsWhitespace   ? "1" : ".";
            }

            string str = str1;

            if (NotAllDots (str2)) str += '\n' + str2; 
            if (NotAllDots (str3)) str += '\n' + str3;
            if (NotAllDots (str4)) str += '\n' + str4;
            if (NotAllDots (str5)) str += '\n' + str5;

            if (str6.Contains ("1"))  str += '\n' + str6;
            if (str7.Contains ("1"))  str += '\n' + str7;
            if (str8.Contains ("1"))  str += '\n' + str8;
            if (str9.Contains ("1"))  str += '\n' + str9;
            if (str10.Contains ("1")) str += '\n' + str10;
            if (str11.Contains ("1")) str += '\n' + str11;

            return str;
        }
    }
}
