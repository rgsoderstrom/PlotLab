using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public partial class TokenParsing
    {
        //***********************************************************************************************************
        //
        // NestingLevel - called with each character in a string to find places outside of parens, strings or brackets
        //

        //static public readonly List<char> CanPreceedString = new List<char> () {'\0', ' ', ',' , '(', '='};



        //**************************************************************************************************
        //
        // SplitFunctionArgs
        //      - of the form (A, B, C)
        //
        static public List<AnnotatedString> SplitFunctionArgs (AnnotatedString str)
        {
            // verify first character is an open paren and last is close paren
            int lastIndex = str.Count - 1;
            if (str [0].IsOpenParen          == false) throw new Exception ("Function arg syntax error at open paren: " + str.Raw);
            if (str [lastIndex].IsCloseParen == false) throw new Exception ("Function arg syntax error at close paren: " + str.Raw);
            
            // copy first char to compare its nesting levels to subsequent characters
            AnnotatedChar initial = str [0];

            // verify all nesting levels same at both ends
            if (AnnotatedChar.SameNesting (str [0], str [lastIndex]) == false)
                throw new Exception ("Function argument error: " + str.Raw);

            // find all commas at same nesting level as open paren. these are where we will start and end copying
            List <int> copyEndpoints = new List<int> ();
            copyEndpoints.Add (0); // start first copy here

            // look for commas at same nesting level
            for (int i=1; i<lastIndex; i++)
                if (str [i].IsComma && AnnotatedChar.SameNesting (str [0], str [i]))
                    copyEndpoints.Add (i);

            copyEndpoints.Add (lastIndex); // end last copy here

            // do the copying
            List<AnnotatedString> extractedArgs = new List<AnnotatedString> ();

            for (int i=0; i<copyEndpoints.Count-1; i++)
            {
                int start = copyEndpoints [i] + 1;
                int end   = copyEndpoints [i+1] - 1;
                int count = end - start + 1;
                AnnotatedString arg = str.TrimmedSubstring (start, count);
                extractedArgs.Add (arg);
            }

            return extractedArgs;
        }

        //***********************************************************************************************************
        //***********************************************************************************************************
        //***********************************************************************************************************

        //
        // BreakIntoSubstrings - break string at points where passed-in CharacterTest is true
        //

        delegate bool CharacterTest (char c);

        void BreakIntoSubstrings (string source, List<string> substrings, CharacterTest test)
        {
            //ResetNestingLevel ();
            //substrings.Clear ();
            //int start = 0;

            //for (int i = 0; i<source.Length; i++)
            //{
            //    NestingLevel (source [i]);

            //    if (ParenNestingLevel == 0 && BracketNestingLevel == 0)
            //    {
            //        if (test (source [i]) == true) // then we found separator
            //        {
            //            string s = source.Substring (start, i - start);

            //            bool empty = true;

            //            foreach (char c in s)
            //            {
            //                if (TokenUtils.IsWhitespace (c) == false)
            //                {
            //                    empty = false;
            //                    break;
            //                }
            //            }

            //            if (empty == false)
            //                substrings.Add (s);

            //            start = i + 1;
            //        }
            //    }                
            //}

            //if (substrings.Count > 0)
            //    substrings.Add (source.Substring (start, source.Length - start));
        }

        //******************************************************************************************************

        //
        // SplitBracketArgs
        //  - break one string [(A + B) : (C + D)] into two
        //

        string RemoveBrackets (string str)
        {
            int index = str.LastIndexOf (']');
            if (index == -1) throw new Exception ("Missing closing bracket: " + str);
            string arguments = str.Remove (index);

            index = arguments.IndexOf ('[');
            if (index == -1) throw new Exception ("Missing opening bracket: " + str);
            arguments = arguments.Remove (0, index + 1);
            return arguments;
        }

        public List<string> SplitBracketArgs_Comma (string str)
        {
            List<string> args = new List<string> ();
            //BreakIntoSubstrings (RemoveBrackets (str), args, TokenUtils.IsComma);
            return args;
        }

        public List<string> SplitBracketArgs_Colon (string str)
        {
            List<string> args = new List<string> ();
            //BreakIntoSubstrings (RemoveBrackets (str), args, TokenUtils.IsColon);
            return args;
        }

        public List<string> SplitBracketArgs_Semi (string str)
        {
            List<string> args = new List<string> ();
            //BreakIntoSubstrings (RemoveBrackets (str), args, TokenUtils.IsSemicolon);
            return args;
        }

        public List<string> SplitBracketArgs_Space (string str)
        {
            //string str1 = RemoveBrackets (str);

            List<string> args = new List<string> ();
            //BreakIntoSubstrings (str1, args, TokenUtils.IsWhitespace);
            //if (args.Count == 0) args.Add (str1); // sigle token, e.g. [0.5]
            return args;
        }

        //********************************************************************************
        //
        // SplitSubmatrixArgs - break one string into two
        //  - eg: (2:4, 6:7) => "2:4", "6:7"
        //  

        public List<string> SplitSubmatrixArgs (string str)
        {
            // remove outer parens 
            int index = str.LastIndexOf (')');
            if (index == -1) throw new Exception ("Submatrix arguments missing closing paren: " + str);
            string arguments = str.Remove (index);

            index = arguments.IndexOf ('(');
            if (index == -1) throw new Exception ("Submatrix arguments missing opening paren: " + str);
            arguments = arguments.Remove (0, index + 1);

            // split arguments string at any commas at nesting level 0
            int [] nestingLevel = new int [arguments.Length];
            int level = 0;

            for (int i=0; i<arguments.Length; i++)
            {
                if (arguments [i] == '(') level++;
                nestingLevel [i] = level;
                if (arguments [i] == ')') level--;
            }

            if (level != 0)
                throw new Exception ("Parenthesis nesting error in " + str);

            List<string> argStrings = new List<string> ();

            int trailing = 0, leading = 1;

            while (leading < arguments.Length)
            {
                if (arguments [leading] == ',' && nestingLevel [leading] == 0)
                {
                    string arg = arguments.Substring (trailing, leading - trailing);
                    argStrings.Add (arg);
                    trailing = leading + 1;
                }

                leading++;
            }

            argStrings.Add (arguments.Substring (trailing, leading - trailing));
            return argStrings;
        }


    }
}
