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

        static public readonly List<char> CanPreceedString = new List<char> () {'\0', ' ', ',' , '(', '='};

        int  ParenNestingLevel   = 0;
        int  BracketNestingLevel = 0;
        int  QuoteNestingLevel   = 0; // never more than 1
        char PrevCharacter       = '\0';

        void ResetNestingLevel ()
        {
            ParenNestingLevel   = 0;
            BracketNestingLevel = 0;
            QuoteNestingLevel   = 0; // never more than 1
            PrevCharacter       = '\0';
        }
        
        int NestingLevel (char c)
        {
            //
            // parenthesis nesting level
            //
            if (TokenUtils.IsOpenParen (c))
                ParenNestingLevel++;

            else if (TokenUtils.IsCloseParen (c))
            {
                ParenNestingLevel--;

                if (ParenNestingLevel < 0)
                    throw new Exception ("Parenthesis argument syntax error, extra closing paren");
            }

            //
            // bracket nesting level
            //
            else if (TokenUtils.IsOpenBracket (c))
                BracketNestingLevel++;

            else if (TokenUtils.IsCloseBracket (c))
            {
                BracketNestingLevel--;

                if (BracketNestingLevel < 0)
                    throw new Exception ("Bracket argument syntax error, extra closing bracket");
            }

            //
            // Quote nesting level
            //
            if (TokenUtils.IsSingleQuote (c))
            {
                if (QuoteNestingLevel == 0)
                {
                    //if (char.IsLetter (PrevCharacter) == false && char.IsDigit (PrevCharacter) == false && CannotPreceedString.Contains (PrevCharacter) == false) // don't count quotes meant as transpose operator
                    if (CanPreceedString.Contains (PrevCharacter) == true)
                        QuoteNestingLevel = 1;
                }

                else 
                    QuoteNestingLevel = 0;
            }

            PrevCharacter = c;

            return ParenNestingLevel + BracketNestingLevel + QuoteNestingLevel;
        }

        //**************************************************************************************************
        //
        // SplitFunctionArgs
        //      - of the form (A, B, C)
        //
        public List<string> SplitFunctionArgs (string str)
        {
            string arguments;

            // if outer parens, remove them
            if (TokenUtils.IsOpenParen (str [0]) && TokenUtils.IsCloseParen (str [str.Length - 1]))
            {
                int index = str.LastIndexOf (')');
                if (index == -1) throw new Exception ("Function arguments missing closing paren: " + str);
                arguments = str.Remove (index);

                index = arguments.IndexOf ('(');
                if (index == -1) throw new Exception ("Function arguments missing opening paren: " + str);
                arguments = arguments.Remove (0, index + 1);
            }
            else
            {
                arguments = str;
            }


            List<string> args = new List<string> ();

            ResetNestingLevel ();
            int start = 0;

            for (int i = 0; i<arguments.Length; i++)
            {
                if (NestingLevel (arguments [i]) == 0)
                {
                    if (TokenUtils.IsComma (arguments [i]))
                    {
                        args.Add (arguments.Substring (start, i - start));
                        start = i + 1;
                    }
                }                
            }

            args.Add (arguments.Substring (start, arguments.Length - start));

            return args;
        }

        //***********************************************************************************************************
        //***********************************************************************************************************
        //***********************************************************************************************************

        //
        // BreakIntoSubstrings - break string at points where passed-in test is true
        //

        delegate bool CharacterTest (char c);

        void BreakIntoSubstrings (string source, List<string> substrings, CharacterTest test)
        {
            ResetNestingLevel ();
            substrings.Clear ();
            int start = 0;

            for (int i = 0; i<source.Length; i++)
            {
                NestingLevel (source [i]);

                if (ParenNestingLevel == 0 && BracketNestingLevel == 0)
                {
                    if (test (source [i]) == true) // then we found separator
                    {
                        string s = source.Substring (start, i - start);

                        bool empty = true;

                        foreach (char c in s)
                        {
                            if (TokenUtils.IsWhitespace (c) == false)
                            {
                                empty = false;
                                break;
                            }
                        }

                        if (empty == false)
                            substrings.Add (s);

                        start = i + 1;
                    }
                }                
            }

            if (substrings.Count > 0)
                substrings.Add (source.Substring (start, source.Length - start));
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
            BreakIntoSubstrings (RemoveBrackets (str), args, TokenUtils.IsComma);
            return args;
        }

        public List<string> SplitBracketArgs_Colon (string str)
        {
            List<string> args = new List<string> ();
            BreakIntoSubstrings (RemoveBrackets (str), args, TokenUtils.IsColon);
            return args;
        }

        public List<string> SplitBracketArgs_Semi (string str)
        {
            List<string> args = new List<string> ();
            BreakIntoSubstrings (RemoveBrackets (str), args, TokenUtils.IsSemicolon);
            return args;
        }

        public List<string> SplitBracketArgs_Space (string str)
        {
            string str1 = RemoveBrackets (str);

            List<string> args = new List<string> ();
            BreakIntoSubstrings (str1, args, TokenUtils.IsWhitespace);
            if (args.Count == 0) args.Add (str1); // sigle token, e.g. [0.5]
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
