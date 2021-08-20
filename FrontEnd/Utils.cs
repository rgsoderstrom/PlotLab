using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd
{
    public static partial class Utils
    {
        //*************************************************************************************************

        public class InputLine
        {
            public InputLine (string txt, bool comp, bool pf) //, NestingLevel nl)
            {
                text      = txt;
                complete  = comp;
                printFlag = pf;
                //finalNesting = nl;
            }

            public readonly string text;
            public readonly bool complete;    // true => no continuation
            public readonly bool printFlag;   // true => no trailing semicolon
            //public NestingLevel finalNesting; // nesting level at end of line
        }

        //*************************************************************************************************

        public static void CleanupRawInput (string raw,       // as typed in, pasted in or read from script file
                                            List<InputLine> final,     // cleanup-up, separated or concatenated here            
                                            ref NestingLevel callersNesting) 
        {
            if (raw.Length == 0)
                return;

            string rawCopy = (string)raw.Clone ();

            //
            // remove leading and trailing whitespaces
            //
            rawCopy = rawCopy.Trim ();

            if (rawCopy.Length == 0)
                return;

            //
            // get nesting level for each character
            //
            List<NestingLevel> nesting = new List<NestingLevel> ();

            nesting.Add (new NestingLevel (callersNesting, rawCopy [0]));

            for (int i = 1; i<rawCopy.Length; i++)
                nesting.Add (new NestingLevel (nesting [i-1], rawCopy [i]));

            //
            // remove any comment. Look for '%' outside of quoted string
            //

            int index = -1;

            for (int i = 0; i<rawCopy.Length; i++)
            {
                if (rawCopy [i] == '%' && nesting [i].QuoteLevel == 0)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                rawCopy = rawCopy.Remove (index);
                nesting.RemoveAt (index);

                if (rawCopy.Length == 0)
                    return;
            }

            //
            // look for expression separators at nesting level == 0
            //

            char [] separators = new char [] { ',', ';' };
            List<int> indices = new List<int> ();

            for (int i = 0; i<rawCopy.Length; i++)
            {
                if (nesting [i].Level == 0)
                    if (separators.Contains (rawCopy [i]))
                        indices.Add (i);
            }

            // split into list of strings with separators
            List<string> rawSubstrings = new List<string> ();

            if (indices.Count == 0) // || (indices [indices.Count - 1] != rawCopy.Length - 1))
                indices.Add (rawCopy.Length - 1);

            int startIndex = 0;

            foreach (int endIndex in indices)
            {
                string str = rawCopy.Substring (startIndex, endIndex - startIndex + 1); // terminator included
                rawSubstrings.Add (str.Trim ());
                startIndex = endIndex + 1;
            }

            if (startIndex < rawCopy.Length)
                rawSubstrings.Add (rawCopy.Substring (startIndex));

            // look at substrings for terminators and continuation dots

            foreach (string str in rawSubstrings)
            {
                string str1 = str; // need a mutable copy

                bool printFlag = str1 [str1.Length - 1] != Semicolon;

                if (separators.Contains (str1 [str1.Length - 1]))
                    str1 = str1.Remove (str1.Length - 1);

                str1 = str1.Trim ();

                bool completeFlag = str1.EndsWith (LineContinued) == false;

                if (completeFlag == false)
                {
                    int i = str1.LastIndexOf (LineContinued);

                    if (i != -1) str1 = str1.Remove (i);
                    else throw new Exception ("Error removing line continuation");
                }

                final.Add (new InputLine (str1, completeFlag, printFlag)); 
            }

            callersNesting = nesting [nesting.Count - 1];
        }
    }
}
