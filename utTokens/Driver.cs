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
                        string noTabs = raw.Replace ('\t', ' ');
                        string trimmed = noTabs.Trim ();

                        if (trimmed.Length == 0 || trimmed [0] == '%')
                            continue;

                        //NestedStringTest (trimmed);
                        //Print ("===========================================");                    

                        //AnnotatedStringTest (trimmed);
                        //Print ("===========================================");

                        //NestedStringSetTest (trimmed);
                        //Print ("===========================================");

                        TokenParsingTest (trimmed);
                        Print ("===========================================");                    

                        //TokenUtilsTest (trimmed);
                        //Print ("===========================================");                    
                    }
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

        static private bool NestedStringTest (string inputString)
        {
            if (inputString.Length == 0)
                return false;

            NestedString nested = new NestedString (inputString);    
            Print (nested.ToString ());

            return true;
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

        private static bool NestedStringSetTest (string str)
        {
            NestedString nested = new NestedString (str);

            if (nested == null)
                return false;

            if (nested.IsEmpty)
                return false;

            NestedStringSet nestedSet = new NestedStringSet ();
            nestedSet.Add (nested);

            Print ("count = " + nestedSet.Count);

            while (nestedSet.Count > 0)
            {
                NestedString next = nestedSet.GetOldest ();

                if (next == null)
                    break;

                Print (next.Plain.ToString ());
                Print (next.ToString ());

                if (nestedSet.Count > 0)
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
            Print (annotated.ToString ());

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
            NestedString nested = new NestedString (str);
            TokenParsing parsing = new TokenParsing ();

            Print ("Before split:");
            Print (nested.ToString () + "\n");

            Print ("\nAfter split:");

            // AnnotatedStringSet fargs = parsing.SplitBracketArgs_Space (annotated);
            NestedStringSet fargs = parsing.SplitBracketArgs_Semi (nested);
            //   AnnotatedStringSet fargs = parsing.SplitBracketArgs_Comma (annotated);

            while (fargs.IsEmpty == false)
            {
                NestedString nstr = fargs.GetOldest ();
                Print (nstr.ToString () + "\n");
            }

            return true;
        }
    }
}
