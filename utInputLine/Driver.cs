using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Main;

namespace utInputLine
{
    class Driver
    {
        static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\InputLineTests.m";

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
                List<List<IToken>> TokensForFileLines = new List<List<IToken>> ();

                InputLineProcessor inputProcessor = new InputLineProcessor (workspace, library, fileSystem, Print);

                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        List<List<IToken>> tok = inputProcessor.ParseOneInputLine (raw);
                        
                        foreach (List<IToken> lt in tok)
                            TokensForFileLines.Add (lt);
                    }
                }

                inputFile.Close ();

                //
                // print results to console
                //

                foreach (List<IToken> lt in TokensForFileLines)
                {
                    foreach (IToken tok in lt)
                        Print (tok.ToString ());

                    Print ("=======================================");




                }
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
        }
    }
}
