using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;
using PLLibrary;

namespace PLKernel
{
    public partial class TokenParsing
    {
        /***************************************

        //
        // NestingLevel - called with each character in a string to find places outside of parens or brackets
        //

        int ParenNestingLevel = 0;
        int BracketNestingLevel = 0;
                    
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

            return ParenNestingLevel + BracketNestingLevel;
        }

        //**************************************************************************************************
        //
        // SplitFunctionArgs
        //      - of the form (A, B, C)
        //
        public List<string> SplitFunctionArgs (string str)
        {
            // remove outer parens 
            int index = str.LastIndexOf (')');
            if (index == -1) throw new Exception ("Function arguments missing closing paren: " + str);
            string arguments = str.Remove (index);

            index = arguments.IndexOf ('(');
            if (index == -1) throw new Exception ("Function arguments missing opening paren: " + str);
            arguments = arguments.Remove (0, index + 1);


            List<string> args = new List<string> ();

            ParenNestingLevel = BracketNestingLevel = 0;
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

        List<string> BreakIntoSubstrings (string source, List<string> substrings, CharacterTest test)
        {
            substrings.Clear ();

            ParenNestingLevel = BracketNestingLevel = 0;
            int start = 0;

            for (int i = 0; i<source.Length; i++)
            {
                if (NestingLevel (source [i]) == 0)
                {
                    if (test (source [i]) == true)
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

            return substrings;
        }

        //******************************************************************************************************

        //
        // SplitBracketArgs
        //  - break one string [(A + B) : (C + D)] into two
        //

        public enum BracketType {Unknown, 
                                 EnumeratedRowVector,   // all elements listed
                                 IteratedRowVector,     // [start : end] or [start : step : end]
                                 ColVector, 
                                 Expression,            // [1 + 2 + 3]
        };

        public BracketType SplitBracketArgs (string str, List<string> args, PrintFunction pf = null)
        {
            // remove outer brackets 
            int index = str.LastIndexOf (']');
            if (index == -1) throw new Exception ("Missing closing bracket: " + str);
            string arguments = str.Remove (index);

            index = arguments.IndexOf ('[');
            if (index == -1) throw new Exception ("Missing opening bracket: " + str);
            arguments = arguments.Remove (0, index + 1);

            //
            // first break into substrings at token separators, at zero nesting level
            //

            args = BreakIntoSubstrings (arguments, args, TokenUtils.IsColon);
            if (args.Count > 0) return BracketType.IteratedRowVector;

            args = BreakIntoSubstrings (arguments, args, TokenUtils.IsSemicolon);
            if (args.Count > 0) return BracketType.ColVector;

            args = BreakIntoSubstrings (arguments, args, TokenUtils.IsComma);
            if (args.Count > 0) return BracketType.EnumeratedRowVector;


            args = BreakIntoSubstrings (arguments, args, TokenUtils.IsBinaryOp);
            if (args.Count > 0) {args.Clear (); args.Add (arguments); return BracketType.Expression;}

            args = BreakIntoSubstrings (arguments, args, TokenUtils.IsWhitespace);

            if (args.Count == 0) // this happens for a single entry, e.g. [22]
                args.Add (arguments);

            return BracketType.EnumeratedRowVector;
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

            string[] argStrings = arguments.Split (new char [] { ',' });

            List<string> args = new List<string> ();

            foreach (string s in argStrings)
                args.Add (s);

            return args;
        }
        ******************/
    }
}
