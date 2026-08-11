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

        //***********************************************************************

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

                        //AnnotatedStringTest (trimmed);
                        //Print ("===========================================");

                        //AnnotatedStringAppendTest (trimmed);
                        //Print ("===========================================");

                        //AnnotatedStringSetTest (trimmed);
                        //Print ("===========================================");

                        TokenParsingTest (trimmed);
                        Print ("===========================================");

                        //TokenUtilsTest (trimmed);
                        //Print ("===========================================");
                    }
                }

                inputFile.Close ();
            }

            catch (NotImplementedException ex)
            {
                Print ("Not implemented exception: " + ex.Message);
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
              //Print (ex.StackTrace);
            }
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool AnnotatedStringTest (string inputString)
        {
            if (inputString.Length == 0)
                return false;

            AnnotatedString astr = new AnnotatedString (inputString);   
            astr.CheckForTrailingSemi ();
            Print (inputString);
            Print (astr.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool AnnotatedStringAppendTest (string inputString)
        {
            if (inputString.Length == 0)
                return false;

            AnnotatedString allAtOnce = new AnnotatedString (inputString);    
            Print (inputString);
            Print (allAtOnce.ToString ());

            Print ("------------");

            AnnotatedString charAtATime = new AnnotatedString (allAtOnce [0]);

            for (int i=1; i<allAtOnce.Length; i++)
                charAtATime.Append (allAtOnce [i]);

            Print (charAtATime.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        private static bool AnnotatedStringSetTest (string str)
        {
            AnnotatedString nested = new AnnotatedString (str);

            if (nested == null)
                return false;

            if (nested.IsEmpty)
                return false;

            AnnotatedStringSet nestedSet = new AnnotatedStringSet ();
            nestedSet.Add (nested);

            Print ("count = " + nestedSet.Count);

            while (nestedSet.Count > 0)
            {
              AnnotatedString next = nestedSet.GetOldest ();

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
            Print (str);

            AnnotatedString annotated = new AnnotatedString (str);

            if (annotated == null)
                return false;

            if (annotated.IsEmpty)
                return false;

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
            AnnotatedString annot = new AnnotatedString (str);
            TokenParsing parsing = new TokenParsing ();

            Print ("Before split:");
            Print (annot.ToString () + "\n");

            //AnnotatedStringSet args = parsing.SplitBracketArgs_Colon (annot);
            AnnotatedStringSet args = parsing.SplitBracketArgs_Space (annot);
            //AnnotatedStringSet args = parsing.SplitBracketArgs_Semi (annot);
            //AnnotatedStringSet args = parsing.SplitBracketArgs_Comma (annot);

            Print ("\n" + args.Count + " args after split:");

            while (args.IsEmpty == false)
            {
                AnnotatedString nstr = args.GetOldest ();
                Print (nstr.Plain);
            //  Print (nstr.ToString ());
            }

            return true;
        }
    }
}
