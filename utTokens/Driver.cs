using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

using PLLibrary;

namespace utTokens
{
    internal class Driver
    {
     // static readonly string InputMFileName = @"..\..\..\TokenUtilsTests.m";
        static readonly string InputMFileName = @"..\..\..\Examples\TokenTests.m";

        private static readonly Workspace  workspace  = new Workspace ();
        private static readonly FileSystem fileSystem = new FileSystem ();

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
                        AnnotatedStringTest (raw);
                        //TokenParsingTest (raw);
                        //TokenUtilsTest (raw);
                    }
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

        static private void AnnotatedStringTest (string str)
        {
            string text = InputLineProcessor.RemovePromptAndComments (str);

            if (text.Length == 0)
                return;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.Plain.ToString ());
            Print (annotated.ToString ());
     
         //   AnnotatedString a2 = annotated.RemoveWrapper ();
         //   Print (a2.ToString ());
            Print ("===========================================");
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private void TokenParsingTest (string raw)
        {
            string text = InputLineProcessor.RemovePromptAndComments (raw);

            if (text.Length == 0)
                return;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.Plain.ToString ());

            //
            // pass each annotated string to token processor
            //
            TokenParsing parser = new TokenParsing ();
            List<IToken> statementtokens = parser.StringToTokens (annotated, workspace, fileSystem);

            foreach (IToken tok in statementtokens)
                Print (tok.ToString ());

             Print ("======================================");
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private void TokenUtilsTest (string raw)
        {
            TokenParsing parsing = new TokenParsing ();

            string text = InputLineProcessor.RemovePromptAndComments (raw);

            if (text.Length == 0)
                return;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.ToString () + "\n");
            List<AnnotatedString> fargs = parsing.SplitBracketArgs_Space (annotated);

            foreach (AnnotatedString AS in fargs)
                Print (AS.ToString () + "\n");
 
            Print ("======================================");
        }

    }
}
