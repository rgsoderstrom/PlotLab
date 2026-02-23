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
     // static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\TokenUtilsTests.m";
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
        
            //    AnnotatedStringTest ();
             //   return;


                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        TokenParsingTest (raw);
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

        static private List<string> AnnotatedTestStrings = new List<string> () {
            //"'c = a * b = %d'",
            //"c = 3 * b;",
            //"( 123, 456  )",
            "n = -4;",
            "z2 = a ~= -b;",
            "z2 = a .* b;",
        };

        static private void AnnotatedStringTest ()
        {
            Console.WriteLine ("AnnotatedStringTest\n");


            foreach (string str in AnnotatedTestStrings)
            {
                AnnotatedString annotated = new AnnotatedString (str);
                Print (annotated.ToString ());
                Print ("===========================================");
            }

        

         //   AnnotatedString a2 = annotated.RemoveWrapper ();
         //   Print (a2.ToString ());
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
