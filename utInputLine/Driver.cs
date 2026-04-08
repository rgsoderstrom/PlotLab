using System;
using System.Collections.Generic;
using System.IO;

using PLFileSystem;
using Main;

namespace utInputLine
{
    class Driver
    {
        static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\InputLineTests.m";

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
                    string pp = InputLineProcessor.Preprocess (raw);

                    if (pp.Length > 0)
                    { 
                        AnnotatedString annot = new AnnotatedString (pp);
                        Console.WriteLine (annot.ToString ());
                        Console.WriteLine ("==========================================");
                    }


                    if (raw.Length > 0)
                    {
                        //List<List<IToken>> tok = inputProcessor.ParseOneInputLine (raw);
                        
                        //foreach (List<IToken> lt in tok)
                        //    TokensForFileLines.Add (lt);
                    }
                }

                inputFile.Close ();

                //
                // print results to console
                //

                //foreach (List<IToken> lt in TokensForFileLines)
                //{
                //    foreach (IToken tok in lt)
                //        Print (tok.ToString ());

                //    Print ("=======================================");




                //}
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
                Print (ex.StackTrace);
            }
        }
    }
}
