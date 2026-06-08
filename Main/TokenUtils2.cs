using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
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

        private void BreakIntoSubstrings (AnnotatedString    src, 
                                          AnnotatedStringSet substrings, 
                                          CharacterTest   test)
        {
            throw new NotImplementedException ();

            //List <int> copyEndpoints = new List<int> ();
            //copyEndpoints.Add (0); // start first copy here

            //int lastIndex = src.CharacterCount - 1;

            //// look for commas at same nesting level
            //for (int i=1; i<lastIndex; i++)
            //    if (test (src [i]) && AnnotatedChar.SameNesting (src [0], src [i]))
            //        copyEndpoints.Add (i);

            //copyEndpoints.Add (lastIndex); // end last copy here

            //// do the copying
            //for (int i=0; i<copyEndpoints.Count-1; i++)
            //{
            //    int start = copyEndpoints [i] + 1;
            //    int end   = copyEndpoints [i+1] - 1;
            //    int count = end - start + 1;
            //    AnnotatedString arg = src.TrimmedSubstring (start, count);
            //    substrings.Add (arg);
            //}
        }

        //**************************************************************************************************
        //
        // SplitFunctionArgs
        //      - of the form (A, B, C)
        //

        public AnnotatedStringSet SplitFunctionArgs (AnnotatedString str)
        {
            throw new NotImplementedException ();

            //// Error checking - verify first character is an open paren and last is close paren
            //int lastIndex = str.CharacterCount - 1;
            //if (str [0].IsOpenParen          == false) throw new Exception ("Function arg syntax error at open paren: " + str.Plain);
            //if (str [lastIndex].IsCloseParen == false) throw new Exception ("Function arg syntax error at close paren: " + str.Plain);
            
            //// extract substrings
            //AnnotatedStringSet extractedArgs = new AnnotatedStringSet ();  
            //BreakIntoSubstrings (str, extractedArgs, delegate (AnnotatedChar ac) {return ac.IsComma;});

            //return extractedArgs;
        }

        //***********************************************************************************************************
        //
        // SplitBracketArgs
        //  - break one string [(A + B) : (C + D)] into two
        //

        // z = [1,2,3]
        // x = [4 5 6]
        // c = [1 : 3 : 20]
        // v = [2 ; 4 ; 6]

        private void VerifyBrackets (AnnotatedString str)
        {
            throw new NotImplementedException ();
            //if (str [0].IsOpenBracket == false)              throw new Exception ("Missing opening bracket: " + str.Plain);
            //if (str [str.CharacterCount - 1].IsCloseBracket == false) throw new Exception ("Missing closing bracket: " + str.Plain);
        }

        public AnnotatedStringSet SplitBracketArgs_Comma (AnnotatedString str)
        {
            throw new NotImplementedException ();
            //VerifyBrackets (str);
            //AnnotatedStringSet args = new AnnotatedStringSet ();
            //BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsComma;});
            //return args;
        }

        public AnnotatedStringSet SplitBracketArgs_Colon (AnnotatedString str)
        {
            throw new NotImplementedException ();
            //VerifyBrackets (str);
            //AnnotatedStringSet args = new AnnotatedStringSet ();
            //BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsColon;});
            //return args;
        }

        public AnnotatedStringSet SplitBracketArgs_Semi (AnnotatedString str)
        {
            AnnotatedString allArgs = AnnotatedString.RemoveWrapper (str);
            AnnotatedStringSet splitArgs = new AnnotatedStringSet ();
            splitArgs.Add (allArgs);
            return splitArgs;
            
            //VerifyBrackets (str);
            //AnnotatedStringSet args = new AnnotatedStringSet ();
            //BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsSemicolon;});
            //return args;
        }

        public AnnotatedStringSet SplitBracketArgs_Space (AnnotatedString str)
        {
            VerifyBrackets (str);
            AnnotatedStringSet args = new AnnotatedStringSet ();
            BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsWhitespace;});
            return args;
        }

        //********************************************************************************
        //
        // SplitSubmatrixArgs - break one string into two
        //  - eg: (2:4, 6:7) => "2:4", "6:7"
        //  

        private void VerifyParenthesis (AnnotatedString str)
        {
            throw new NotImplementedException ();
            //if (str [0].IsOpenParen == false)              throw new Exception ("Missing opening parenthesis: " + str.Plain);
            //if (str [str.CharacterCount - 1].IsCloseParen == false) throw new Exception ("Missing closing parenthesis: " + str.Plain);
        }

        public AnnotatedStringSet SplitSubmatrixArgs (AnnotatedString str)
        {
            throw new NotImplementedException ();
            //VerifyParenthesis (str);

            //// split arguments string at any commas at same nesting level as first char
            //AnnotatedStringSet args = new AnnotatedStringSet ();
            //BreakIntoSubstrings (str, args, delegate (AnnotatedChar ac) {return ac.IsComma;});
            //return args;
        }
    }
}
