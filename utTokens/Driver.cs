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
                        string trimmed = raw.Trim ();

                        if (trimmed.Length == 0 || trimmed [0] == '%')
                            continue;

                        bool ps = false; // print separator
                        //ps |= AnnotatedStringTest (trimmed);
                        //ps |= AnnotatedStringSetTest (trimmed);
                        //ps |= TokenParsingTest (trimmed);
                        ps |= TokenUtilsTest (trimmed);
                        
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

        static private bool AnnotatedStringTest (string inputString)
        {
            if (inputString.Length == 0)
                return false;

            AnnotatedString annotated = new AnnotatedString (inputString);    
            Print (annotated.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        private static bool AnnotatedStringSetTest (string str)
        {
            AnnotatedString annotated = new AnnotatedString (str);

            if (annotated == null)
                return false;

            if (annotated.IsEmpty)
                return false;

            AnnotatedStringSet annotatedSet = new AnnotatedStringSet ();
            annotatedSet.Add (annotated);

            Print ("annotatedSet count = " + annotatedSet.Count);

            while (annotatedSet.Count > 0)
            {
                AnnotatedString next = annotatedSet.GetOldest ();

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

        static private bool TokenParsingTest (string str)
        {
            AnnotatedString annotated = new AnnotatedString (str);

            if (annotated == null)
                return false;

            if (annotated.IsEmpty)
                return false;

            Print (annotated.Plain.ToString ());

            // pass annotated string to token processor
            TokenParsing parser = new TokenParsing ();
            TokenSet statementtokens = parser.StringToTokens (annotated);

            Print (statementtokens.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool TokenUtilsTest (string str)
        {
            AnnotatedString annotated = new AnnotatedString (str);
            TokenParsing parsing = new TokenParsing ();

            Print ("Before split:");
            Print (annotated.ToString () + "\n");

            Print ("\nAfter split:");

            // AnnotatedStringSet fargs = parsing.SplitBracketArgs_Space (annotated);
            // AnnotatedStringSet fargs = parsing.SplitBracketArgs_Semi (annotated);
               AnnotatedStringSet fargs = parsing.SplitBracketArgs_Comma (annotated);

            while (fargs.Count > 0)
            {
                AnnotatedString astr = fargs.GetOldest ();
                Print (astr.ToString () + "\n");
            }

            return true;
        }
    }
}
