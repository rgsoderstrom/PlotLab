using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

using static System.Net.WebRequestMethods;

namespace utTokens
{
    internal class Driver
    {
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
            StreamReader inputFile = new StreamReader (InputMFileName);
            string raw;

            while ((raw = inputFile.ReadLine ()) != null)
            {
                if (raw.Length > 0)
                {
                    string text = InputLineProcessor.RemovePromptAndComments (raw);

                    if (text.Length == 0)
                        continue;

                    text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
                    AnnotatedString annotated = new AnnotatedString (text);

                    Print (annotated.ToString ());


                    //
                    // pass each annotated string to token processor
                    //
                    TokenParsing parser = new TokenParsing ();


                    // NO SUPRESS OUTPUT


                    List<IToken> statementTokens = parser.StringToTokens (annotated, workspace, library, fileSystem);

                    foreach (IToken tok in statementTokens)
                        Print (tok.ToString ());

                    Print ("======================================");

                }
            }

            inputFile.Close ();
        }
    }
}
