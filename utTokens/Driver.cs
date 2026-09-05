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

        enum TestToRun {AnnotatedString, AnnotatedStringAppend, AnnotatedStringSet,
                        TokenParsing, TokenUtils};

        private static readonly TestToRun test = TestToRun.AnnotatedStringAppend;

        private static readonly Dictionary<TestToRun, string> sectionHeader = new Dictionary<TestToRun, string>
        {
            {TestToRun.AnnotatedString,       "AnnotatedString"},
            {TestToRun.AnnotatedStringAppend, "AnnotatedStringAppend"},
            {TestToRun.AnnotatedStringSet,    "AnnotatedStringSet"},
            {TestToRun.TokenParsing,          "TokenParsing"},
            {TestToRun.TokenUtils,            "TokenUtils"},
        };

        private delegate bool TestFunction (string str);

        private static readonly Dictionary<TestToRun, TestFunction> testFunction = new Dictionary<TestToRun, TestFunction>
        {
            {TestToRun.AnnotatedString,       AnnotatedStringTest},
            {TestToRun.AnnotatedStringAppend, AnnotatedStringAppendTest},
            {TestToRun.AnnotatedStringSet,    AnnotatedStringSetTest},
            {TestToRun.TokenParsing,          TokenParsingTest},
            {TestToRun.TokenUtils,            TokenUtilsTest},
        };

        static void Main (string [] _)
        {
            try
            {
                ReadTestFileLines (sectionHeader [test]);
                string testCase;

                while ((testCase = GetNextLine ()) != null)
                {
                    Console.WriteLine ("Running test: " + test.ToString ());
                    Console.WriteLine ("Test case: " + testCase);

                    testFunction [test] (testCase);

                    Print ("===========================================");
                }
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

        private static readonly List<string> fileLines = new List<string> ();
        static private int get = 0;

        static bool ReadTestFileLines (string sectionStartText)
        {
            bool storingFileLines = false;

            using (var reader = new StreamReader (InputMFileName))
            {
                while (!reader.EndOfStream)
                {
                    string raw = reader.ReadLine ();
  
                    if (raw.Length > 0)
                    {
                        string noTabs = raw.Replace ('\t', ' ');
                        string trimmed = noTabs.Trim ();

                        if (trimmed.Length == 0 || trimmed [0] == '%')
                            continue;

                        if (trimmed [0] == '#')
                        {
                            if (storingFileLines)
                                storingFileLines = false;

                            else
                                if (trimmed.Substring (2) == sectionStartText)
                                    storingFileLines = true;
                        }

                        else if (storingFileLines == true)
                            fileLines.Add (trimmed);
                    }
                } 
            }

            return true;
        }

        static string GetNextLine ()
        {
            if (get < fileLines.Count)
                return fileLines [get++];

            return null;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool AnnotatedStringTest (string inputString)
        {
            if (inputString.Length == 0)
                return false;

            AnnotatedString astr = new AnnotatedString (inputString);   
            //astr.CheckForTrailingSemi ();
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

            AnnotatedString charAtATime = new AnnotatedString (inputString.Substring (0, 1));

            for (int i=1; i<allAtOnce.Length; i++)
                charAtATime.Append (inputString [i]);

            
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
          //  annotated.CheckForTrailingSemi ();

            if (annotated == null)
                return false;

            if (annotated.IsEmpty)
                return false;

            Print (annotated.ToString ());

            if (false) // annotated.AlphanumericOnly)
            {
                Print ("\nAlphanumericOnly, token parsing skipped");
            }
            else
            { 
                // pass annotated string to token processor
                TokenParsing parser = new TokenParsing ();
                TokenSet statementtokens = parser.StringToTokens (annotated);

                Print (statementtokens.ToString ());
            }

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

            AnnotatedStringSet args = parsing.SplitBracketArgs_Colon (annot);
            //AnnotatedStringSet args = parsing.SplitBracketArgs_Space (annot);
            //AnnotatedStringSet args = parsing.SplitBracketArgs_Semi (annot);
            //AnnotatedStringSet args = parsing.SplitBracketArgs_Comma (annot);

            Print ("\n" + args.Count + " args after split:");

            while (args.IsEmpty == false)
            {
                AnnotatedString nstr = args.GetOldest ();
                Print (nstr.Plain);
            }

            return true;
        }
    }
}
