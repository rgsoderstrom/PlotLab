using System.Collections.Generic;

namespace PLMain
{
    public partial class TokenParsing
    {
        //***********************************************************************************************************
        //
        // BreakIntoSubstrings - break string at points where
        //   1. nesting level matches first character
        //   2. some passed-in CharacterTest is true
        //
        delegate bool CharacterTest (AnnotatedChar c);

        private static List<string> BreakIntoSubstrings (AnnotatedString src,                                          
                                                         CharacterTest   test)
        {
            List<string> substrings = new List<string> ();

            if (src.Length < 2)
            {
                substrings.Add (src.Plain);
                return substrings;
            }

            List<int> copyEndpoints = new List<int> () {0};

            int lastIndex = src.CharacterCount - 1;
            int nesting = src [0].NestingLevel;

            // apply "test" to each character inside the wrapper
            for (int i = 1; i<lastIndex; i++)
            {
                AnnotatedChar ac = src [i];
                bool results = test (ac) == true && ac.NestingLevel == nesting;

                if (results)
                    copyEndpoints.Add (i);
            }

            copyEndpoints.Add (lastIndex); // end last copy here

            // do the copying
            for (int i = 0; i<copyEndpoints.Count-1; i++)
            {
                int start = copyEndpoints [i] + 1;
                int end = copyEndpoints [i+1] - 1;
                int count = end - start + 1;
                string arg = src.TrimmedSubstring (start, count).Plain;
                substrings.Add (arg);
            }

            return substrings;
        }

        //**************************************************************************************************
        //
        // SplitFunctionArgs
        //      - of the form (A, B, C)
        //

        public List<string> SplitFunctionArgs (AnnotatedString str)
        {
            List<string> args = BreakIntoSubstrings (str, delegate (AnnotatedChar ac) {return ac.IsComma;});
            return args;
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

        public List<string> SplitBracketArgs_Comma (AnnotatedString str)
        {
            List<string> args = BreakIntoSubstrings (str, delegate (AnnotatedChar ac) {return ac.IsComma;});
            return args;
        }

        public List<string> SplitBracketArgs_Colon (AnnotatedString str)
        {
            List<string> args = BreakIntoSubstrings (str, delegate (AnnotatedChar ac) {return ac.IsColon;});
            return args;
        }

        public List<string> SplitBracketArgs_Semi (AnnotatedString str)
        {
            List<string> args = BreakIntoSubstrings (str, delegate (AnnotatedChar ac) {return ac.IsSemicolon;});
            return args;
        }

        public List<string> SplitBracketArgs_Space (AnnotatedString str)
        {
            List<string> args = BreakIntoSubstrings (str, delegate (AnnotatedChar ac) {return ac.IsWhitespace;});
            return args;
        }

        //********************************************************************************
        //
        // SplitSubmatrixArgs - break one string into two
        //  - eg: (2:4, 6:7) => "2:4", "6:7"
        //  

        public List<string> SplitSubmatrixArgs (AnnotatedString str)
        {
            // split arguments string at any commas at same nesting level as first char
            List<string> args = BreakIntoSubstrings (str, delegate (AnnotatedChar ac) {return ac.IsComma;});
            return args;
        }
    }
}
