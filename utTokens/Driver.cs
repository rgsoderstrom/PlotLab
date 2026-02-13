using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

namespace utTokens
{
    internal class Driver
    {
     // static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\TokenUtilsTests.m";
        static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\TokenTests.m";

        private static readonly Workspace  workspace  = new Workspace ();
        private static readonly Library    library    = new Library ();
        private static readonly FileSystem fileSystem = new FileSystem ();

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        static void Main (string [] args)
        {
            try
            {
                //  AnnotatedStringTest ();



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

        static private void AnnotatedStringTest ()
        {
            AnnotatedString annotated = new AnnotatedString ("( 123, 456  )");
            Print (annotated.ToString ());
        
            Print ("");

            AnnotatedString a2 = annotated.RemoveWrapper ();
            Print (a2.ToString ());
        }

        static private void TokenParsingTest (string raw)
        {
            string text = InputLineProcessor.RemovePromptAndComments (raw);

            if (text.Length == 0)
                return;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.ToString ());

            //
            // pass each annotated string to token processor
            //
            TokenParsing parser = new TokenParsing ();
            List<IToken> statementtokens = parser.StringToTokens (annotated, workspace, library, fileSystem);

            foreach (IToken tok in statementtokens)
                Print (tok.ToString ());

             Print ("======================================");
        }

        static private void TokenUtilsTest (string raw)
        {
            string text = InputLineProcessor.RemovePromptAndComments (raw);

            if (text.Length == 0)
                return;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.ToString () + "\n");
            List<AnnotatedString> fargs = TokenParsing.SplitBracketArgs_Space (annotated);

            foreach (AnnotatedString AS in fargs)
                Print (AS.ToString () + "\n");
 
            Print ("======================================");
        }

    }
}
