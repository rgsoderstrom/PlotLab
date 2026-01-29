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
        static readonly string InputMFile = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\InputLineTests.m";


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
                List<List<Token>> TokensForFileLines = new List<List<Token>> ();

                InputLineProcessor inputProcessor = new InputLineProcessor (workspace, library, fileSystem, Print);

                StreamReader inputFile = new StreamReader (InputMFile);
                string raw;

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        List<List<Token>> tok = inputProcessor.ParseOneInputLine (raw);
                        
                        foreach (List<Token> lt in tok)
                            TokensForFileLines.Add (lt);
                    }
                }

                inputFile.Close ();

                //
                // print results to console and serialize to a file
                //
                foreach (List<Token> lt in TokensForFileLines)
                {
                    foreach (Token tok in lt)
                        Print (tok.ToString ());

                    Print ("=======================================");

                    SerializedTokens listOfTokens = new SerializedTokens (lt);

                    StreamWriter tokenWriter = new StreamWriter ("../../listOfTokens.bin");
                    BinaryFormatter b = new BinaryFormatter();
                    b.Serialize (tokenWriter.BaseStream, listOfTokens);
                    tokenWriter.Close();
                }
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
        }
    }
}
