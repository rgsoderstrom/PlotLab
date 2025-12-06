
/*
    InputLineUtils.cs - 
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using static System.Net.Mime.MediaTypeNames;

namespace Main
{
    internal partial class InputLineProcessor
    {
        public static readonly string Prompt = "--> ";
        public static readonly string LineContinued = "...";
        public static readonly char   Semicolon = ';';

        //**************************************************************************************

        private string RemovePromptAndComments (string lineIn)
        {
            bool inString = false;

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
            // remove any comment
            //
            if (lineOut.Contains ("%"))
            {
                for (int i=lineOut.Length-1; i>=0; i--)
                {
                    if (lineOut [i] == '\'') // then entering or leaving quoted string, where % doesn't mean comment
                        inString ^= true;

                    if (lineOut [i] == '%' && inString == false)
                    { 
                        lineOut = lineOut.Remove (i);
                        break;
                    }
                }
            }

            return lineOut.TrimEnd ();
        }

        //**************************************************************************************

        private string SqueezeConsecutiveSpaces (string text)
        {
            text = text.Replace ("\t", " ");

            string [] splitText = text.Split (new string [] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            string results = splitText [0];

            for (int i=1; i<splitText.Length; i++)
                results += " " + splitText [i].Trim ();

            return results;
        }

        //**************************************************************************************

        private string IdentifyInputLine (string lineIn)
        {

            return "";
        }


    }
}
