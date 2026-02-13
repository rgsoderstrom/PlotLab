using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public partial class TokenParsing
    {
 
        //static public readonly List<char> CanPreceedString = new List<char> () {'\0', ' ', ',' , '(', '='};

        //***********************************************************************************************************
        //
        // BreakIntoSubstrings - break string at points where
        //   1. nesting level matches first character
        //   2. some passed-in CharacterTest is true
        //

        delegate bool CharacterTest (AnnotatedChar c);

        static private void BreakIntoSubstrings (AnnotatedString       src, 
                                                 List<AnnotatedString> substrings, 
                                                 CharacterTest         test)
        {
            List <int> copyEndpoints = new List<int> ();
            copyEndpoints.Add (0); // start first copy here

            int lastIndex = src.Count - 1;

            // look for commas at same nesting level
            for (int i=1; i<lastIndex; i++)
                if (test (src [i]) && AnnotatedChar.SameNesting (src [0], src [i]))
                    copyEndpoints.Add (i);

            copyEndpoints.Add (lastIndex); // end last copy here

            // do the copying
            for (int i=0; i<copyEndpoints.Count-1; i++)
            {
                int start = copyEndpoints [i] + 1;
                int end   = copyEndpoints [i+1] - 1;
                int count = end - start + 1;
                AnnotatedString arg = src.TrimmedSubstring (start, count);
                substrings.Add (arg);
            }
        }

        //**************************************************************************************************
        //
        // SplitFunctionArgs
        //      - of the form (A, B, C)
        //

        static public List<AnnotatedString> SplitFunctionArgs (AnnotatedString str)
        {
            // Error checking - verify first character is an open paren and last is close paren
            int lastIndex = str.Count - 1;
            if (str [0].IsOpenParen          == false) throw new Exception ("Function arg syntax error at open paren: " + str.Plain);
            if (str [lastIndex].IsCloseParen == false) throw new Exception ("Function arg syntax error at close paren: " + str.Plain);
            
            // extract substrings
            List<AnnotatedString> extractedArgs = new List<AnnotatedString> ();  
            BreakIntoSubstrings (str, extractedArgs, delegate (AnnotatedChar ac) {return ac.IsComma;});

            return extractedArgs;
        }

        //***********************************************************************************************************
        //***********************************************************************************************************

        //******************************************************************************************************

        //
        // SplitBracketArgs
        //  - break one string [(A + B) : (C + D)] into two
        //

        // z = [1,2,3]
        // x = [4 5 6]
        // c = [1 : 3 : 20]
        // v = [2 ; 4 ; 6]

        static private void VerifyBrackets (AnnotatedString str)
        {
            if (str [0].IsOpenBracket == false)              throw new Exception ("Missing opening bracket: " + str.Plain);
            if (str [str.Count - 1].IsCloseBracket == false) throw new Exception ("Missing closing bracket: " + str.Plain);
        }

        static public List<AnnotatedString> SplitBracketArgs_Comma (AnnotatedString str)
        {
            VerifyBrackets (str);
            List<AnnotatedString> args = new List<AnnotatedString> ();
            BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsComma;});
            return args;
        }

        static public List<AnnotatedString> SplitBracketArgs_Colon (AnnotatedString str)
        {
            VerifyBrackets (str);
            List<AnnotatedString> args = new List<AnnotatedString> ();
            BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsColon;});
            return args;
        }

        static public List<AnnotatedString> SplitBracketArgs_Semi (AnnotatedString str)
        {
            VerifyBrackets (str);
            List<AnnotatedString> args = new List<AnnotatedString> ();
            BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsSemicolon;});
            return args;
        }

        static public List<AnnotatedString> SplitBracketArgs_Space (AnnotatedString str)
        {
            VerifyBrackets (str);
            List<AnnotatedString> args = new List<AnnotatedString> ();
            BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsWhitespace;});
            return args;
        }

        //********************************************************************************
        //
        // SplitSubmatrixArgs - break one string into two
        //  - eg: (2:4, 6:7) => "2:4", "6:7"
        //  

        static public List<AnnotatedString> SplitSubmatrixArgs (AnnotatedString str)
        {
            VerifyBrackets (str);

            // split arguments string at any commas at same nesting level as first char
            List<AnnotatedString> args = new List<AnnotatedString> ();
            BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsComma;});
            return args;
        }
    }
}
