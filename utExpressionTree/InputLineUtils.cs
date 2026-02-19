
/*
    InputLineUtils.cs - 
*/

using System;

namespace Main
{
    internal partial class InputLineProcessor
    {
        public static readonly string Prompt = "--> ";
        public static readonly string LineContinued = "...";
        public static readonly char   Semicolon = ';';

        //**************************************************************************************

        public static string RemovePromptAndComments (string lineIn)
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

                for (int i=0; i<lineOut.Length; i++)
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

        static private bool CanPreceedTranspose (char c)
        {
            if (c == ')') return true;
            if (c == ']') return true;
            if (char.IsLetter (c)) return true;
            if (char.IsNumber (c)) return true;
            return false;
        }

        //**************************************************************************************

        public static string SqueezeConsecutiveSpaces (string text)
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

        private string IdentifyInputLine (string lineIn)
        {

            return "";
        }


    }
}
