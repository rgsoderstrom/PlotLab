using System;
using System.Collections.Generic;
using System.IO;

using Main;

using PLFileSystem;

using static Main.InputLineProcessor;

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
                string inputString;

                InputLineProcessor inputLineProc = new InputLineProcessor ();
                List<LineType> lineTypes = new List<LineType> ();
                List<AnnotatedString> annotStrings = new List<AnnotatedString> ();

                while ((inputString = inputFile.ReadLine ()) != null)
                {
                    if (inputString.Length > 0)
                    { 
                        Console.WriteLine (inputString);

                     //   lineTypes.Clear ();
                     //   annotStrings.Clear ();
                        inputLineProc.IdentifyInputLine (inputString, ref lineTypes, ref annotStrings);


                        Console.WriteLine ("==========================================");
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
