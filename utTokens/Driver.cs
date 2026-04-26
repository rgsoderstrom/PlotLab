using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

//using PLLibrary;

using static System.Net.Mime.MediaTypeNames;

namespace utTokens
{
    internal class Driver
    {
     // static readonly string InputMFileName = @"..\..\..\Examples\TokenUtilsTests.m";
        static readonly string InputMFileName = @"..\..\..\Examples\TokenTests.m";

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        static void Main (string [] args)
        {
            try
            {
                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        bool ps = false; // print separator
                        //ps |= AnnotatedStringTest (raw);
                        ps |= TokenParsingTest (raw);
                        //ps |= TokenUtilsTest (raw);
                        
                        if (ps) Print ("===========================================");                    }
                }

                inputFile.Close ();
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool AnnotatedStringTest (string str)
        {
            string text = InputLineProcessor.PreprocessInputLine (str);

            if (text.Length == 0)
                return false;

            Print ("Cleaned file line: " + text);

            AnnotatedStringSet annotated = new AnnotatedStringSet (text);

            for (int i=0; i<annotated.Count; i++)
            { 
                Print (annotated [i].Plain.ToString ());
                Print (annotated [i].ToString ());

                if (i != annotated.Count - 1)
                    Print ("------------------------");
            }

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool TokenParsingTest (string str)
        {
            string text = InputLineProcessor.PreprocessInputLine (str);

            if (text.Length == 0)
                return false;

            Print ("");
            Print ("File line: " + str);
            Print ("After preprocess: " + text);
            Print ("");

            AnnotatedStringSet annSet = new AnnotatedStringSet (text);

            for (int i=0; i<annSet.Count; i++)
            { 
                AnnotatedString annotated = annSet [i];
                Print (annotated.Plain.ToString ());

             // pass each annotated string to token processor
                TokenParsing parser = new TokenParsing ();
                TokenSet statementtokens = parser.StringToTokens (annotated);

                Print (statementtokens.ToString ());

                if (i + 1 < annSet.Count) // print separator if more to be printed
                    Print ("");
            }

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool TokenUtilsTest (string raw)
        {
            TokenParsing parsing = new TokenParsing ();

            string text = InputLineProcessor.PreprocessInputLine (raw);

            if (text.Length == 0)
                return false;

            AnnotatedString annotated = new AnnotatedString (text);

            Print ("Before split:");
            Print (annotated.ToString () + "\n");

            Print ("\nAfter split:");

            List<AnnotatedString> fargs = parsing.SplitBracketArgs_Space (annotated);

            foreach (AnnotatedString AS in fargs)
                Print (AS.ToString () + "\n");
 
            return true;
        }
    }
}
