using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLMain;

//using PLLibrary;

using static System.Net.Mime.MediaTypeNames;

namespace utTokens
{
    internal class Driver
    {
        //static readonly string InputMFileName = @"..\..\..\Examples\TokenUtilsTests.m";
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
                        ps |= AnnotatedStringTest (raw);
                        //ps |= AnnotatedStringSetTest (raw);
                        //ps |= TokenParsingTest (raw);
                        //ps |= TokenUtilsTest (raw);
                        
                        if (ps) Print ("===========================================");                    }
                }

                inputFile.Close ();
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
                Print (ex.StackTrace);
            }
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private readonly string continuationString = "...";
        static private string cumulative = "";

        static private AnnotatedString annotated = null;

        static private bool AnnotatedStringTest (string inputString, bool verbose = true)
        {
            string cleanedInput = InputLineProcessor.PreprocessInputLine (inputString);

            if (cleanedInput.Length == 0)
                return false;

            if (verbose)
                Print ("Cleaned file line: " + cleanedInput);

            bool continues = false;

            if (cleanedInput.EndsWith (continuationString))
            {
                cleanedInput = cleanedInput.Remove (cleanedInput.Length - continuationString.Length);
                continues = true;
            }                

            cumulative += cleanedInput;

            if (continues == true)
                return false;

            annotated = new AnnotatedString (cumulative);
            cumulative = "";
    
            if (verbose)
                Print (annotated.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        
        static AnnotatedStringSet annotatedSet = new AnnotatedStringSet ();

        private static bool AnnotatedStringSetTest (string str, bool verbose = true)
        {
            AnnotatedStringTest (str, false);

            if (annotated == null)
                return false;

            if (annotated.IsEmpty)
                return false;

            annotatedSet.Add (annotated);
            annotated = null;

            if (verbose == false)
                return false;

            Print ("annotatedSet count = " + annotatedSet.Count);

            while (annotatedSet.Count > 0)
            {
                AnnotatedString next = annotatedSet.GetOldest;

                if (next == null)
                    break;

                Print (next.Plain.ToString ());
                Print (next.ToString ());

                if (annotatedSet.Count > 0)
                    Print ("------------------------");
            }

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool TokenParsingTest (string str, bool verbose = true)
        {
            //AnnotatedStringSetTest (str, false);

            //if (annotatedSet.Count == 0)
            //    return false;

            //while (annotatedSet.Count > 0)
            //{
            //    AnnotatedString annotated = annotatedSet.GetOldest;

            //    Print (annotated.Plain.ToString ());

            //    // pass each annotated string to token processor
            //    TokenParsing parser = new TokenParsing ();
            //    TokenSet statementtokens = parser.StringToTokens (annotated);

            //    Print (statementtokens.ToString ());

            //    if (annotatedSet.Count != 0) // print separator if more to be printed
            //        Print ("");
            //}

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

         // AnnotatedStringSet fargs = parsing.SplitBracketArgs_Space (annotated);
            AnnotatedStringSet fargs = parsing.SplitBracketArgs_Semi (annotated);

            while (fargs.Count > 0)
            {                 
                AnnotatedString ann = fargs.GetOldest;
                Print (ann.ToString () + "\n");
            }
 
            return true;
        }
    }
}
