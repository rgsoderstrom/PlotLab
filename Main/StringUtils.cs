using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    //*************************************************************************
    //
    // CleanStringQueue - 
    //
    // Queue of input strings (from console, a dot-m file or pasted) with:
    //  - any comment and extra spaces removed
    //  - prompt removed
    //  - continued lines concatenated
    //  - compound lines split

    internal class CleanStringQueue
    {
        // queue of complete CleanStrings,
        private readonly Queue<string> cleanedStrings = new Queue<string> ();

        // number of complete string ready for processing
        public int Count {get {return cleanedStrings.Count;}}

        // GetOldest
        public string GetOldest {get {return cleanedStrings.Dequeue ();}}

        //**********************************************************************

        private string cumulative = "";
        static private readonly string continuationString = "...";

        public void Add (string fileLine)
        {
            string cleanedInput = StringUtils.PreprocessInputLine (fileLine);

            if (cleanedInput.Length == 0)
                return;

            bool continues = false;

            if (cleanedInput.EndsWith (continuationString))
            {
                cleanedInput = cleanedInput.Remove (cleanedInput.Length - continuationString.Length);
                continues = true;
            }                

            cumulative += cleanedInput;

            if (continues == true)
                return;

            cleanedStrings.Enqueue (cumulative);
            cumulative = "";
        }
    }

    //*************************************************************************
    //
    // StringUtils - 
    //

    internal static class StringUtils
    {
        internal static readonly string Prompt = "--> ";

        //**************************************************************************************

        // first processing on each input line
        //  - remove prompt and any comments
        //  - remove extra spaces & tabs

        public static string PreprocessInputLine (string lineIn)
        {
            if (lineIn.Length == 0) return lineIn;
            string s1 = RemovePromptAndComments (lineIn);
            if (s1.Length == 0) return s1;
            string s2 = SqueezeConsecutiveSpaces (s1);
            return s2;
        }

        //**************************************************************************************

        private static string RemovePromptAndComments (string lineIn)
        {
            string lineOut = lineIn.Trim (); // remove leading and trailing spaces

            //
            // remove prompt if present
            //
            if (lineOut.Length >= Prompt.Length)
                if (lineOut.Substring (0, Prompt.Length).Contains (Prompt))
                    lineOut = lineOut.Remove (0, Prompt.Length);

            if (lineOut.Length == 0)
                return lineOut;

            //
            // remove any text after a comment sign
            //

            // Usually a comment is any text following a %
            // Exception is if % is inside a string, e.g. sprintf ('%3.4f', abc);
            // Further complication is that a single quote doesn't always mark the start or end of a string.
            // It can also mean Transpose but only when following a variable or array, e.g. [1:10]'

            if (lineOut [0] == '%') // whole line comment
                return "";

            if (lineOut.Contains ("%"))
            {
                bool inString = false;

                for (int i = 0; i<lineOut.Length; i++)
                {
                    if (lineOut [i] == '\'') // then may be entering a quoted string, where % doesn't indicate comment
                    {
                        if (i == 0)
                            inString = true;

                        else if (inString == false)
                        {
                            if (CanPreceedTranspose (lineOut [i-1]) == false)
                                inString = true;
                        }

                        else
                            inString = false;
                    }

                    if (lineOut [i] == '%' && inString == false)
                    {
                        lineOut = lineOut.Remove (i);
                        break;
                    }
                }
            }

            return lineOut.TrimEnd ();
        }

        //*******************************************************************************

        private static bool CanPreceedTranspose (char c)
        {
            if (c == ')') return true;
            if (c == ']') return true;
            if (char.IsLetter (c)) return true;
            if (char.IsNumber (c)) return true;
            return false;
        }

        //**************************************************************************************

        private static string SqueezeConsecutiveSpaces (string text)
        {
            string results = "";

            text = text.Replace ("\t", " ");

            string [] splitText = text.Split (new string [] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (splitText.Length > 0)
            { 
                results = splitText [0];

                for (int i=1; i<splitText.Length; i++)
                    results += " " + splitText [i].Trim ();
            }

            return results;
        }

        //**************************************************************************************


        // MOVED TO NestedString

        //public static string FirstWord (string text)
        //{
        //    int index = text.IndexOf (' ');

        //    if (index == -1)
        //        return text;

        //    string results = text.Substring (0, index);
        //    return results;
        //}

        //public static bool SingleWord (string text)
        //{
        //    int index = text.IndexOf (' ');
        //    return index == -1;
        //}

        //**************************************************************************************

        // MOVED TO NestedString

        //public static bool AlphanumericOnly (string text)
        //{
        //    if (char.IsLetter (text [0]) == false)
        //        return false;

        //    for (int i=1; i<text.Length; i++)
        //    {
        //        if (char.IsLetterOrDigit (text [i]) == false)
        //            return false;
        //    }

        //    return true;
        //}

    }
}
