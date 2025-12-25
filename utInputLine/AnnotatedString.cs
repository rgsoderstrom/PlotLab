
/*
    NestingLevelString - an array of NestingLevel objects built from a text string
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class AnnotatedString
    {
        // private members
        private readonly AnnotatedChar [] annotatedChars;

        // public access properties
        internal string Text
        {
            get
            {
                char [] str = new char [annotatedChars.Length];

                for (int i=0; i<annotatedChars.Length; i++)
                    str [i] = annotatedChars [i].Character;

                return str.ToString ();
            }
        }

        public int Length {get {return annotatedChars.Length;}}

        //*******************************************************************
        //
        // ctor
        //
        internal AnnotatedString (string text)
        {
            annotatedChars = new AnnotatedChar [text.Length];

            annotatedChars [0] = new AnnotatedChar (text [0]); 

            for (int i=1; i<text.Length; i++)
                annotatedChars [i] = new AnnotatedChar (annotatedChars [i-1], text [i]); 
        }

        internal AnnotatedString (AnnotatedChar [] source, int start, int count)
        {
            annotatedChars = new AnnotatedChar [count];
            int put = 0;

            try
            { 
                for (int i=start; i<start+count; i++)
                    annotatedChars [put++] = source [i]; 
            }

            catch (Exception ex)
            {
                throw new Exception ("Exception: " + ex.Message);
              //Console.WriteLine ("Exception: " + ex.Message);
            }
        }

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

                for (int i=0; i<annotatedChars.Length; i++)
                    if (annotatedChars [i].Character == ';' && annotatedChars [i].Level == 0)
                        indices.Add (i);

                bool indicesEmpty = indices.Count == 0;
                bool lastCharNotSemi = annotatedChars [annotatedChars.Length-1].Character != ';';

                if (indicesEmpty || lastCharNotSemi)
                    indices.Add (annotatedChars.Length - 1);

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
        public override string ToString ()
        {
            string str1 = "Char:  ";
            string str2 = "Paren: ";
            string str3 = "Bkt:   ";
            string str4 = "Quote: ";

            foreach (AnnotatedChar ac in annotatedChars)
            {
                str1 += ac.Character;
                str2 += ac.ParenLevel;
                str3 += ac.BracketLevel;
                str4 += ac.QuoteLevel;
            }

            string str = str1 + '\n' + str2 + '\n' + str3 + '\n' + str4;
            return str;
        }
    }
}
