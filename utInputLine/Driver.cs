using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Main;

using PLCommon;
using PLFileSystem;
using PLWorkspace;

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
                FileSystem.Open (Print);
                Workspace.Add (new PLDouble ("a", 1.23));

                StreamReader inputFile = new StreamReader (InputMFileName);
                string inputString;

                InputLineProcessor    inputLineProc        = new InputLineProcessor ();
                List<LineType>        statementTypes       = new List<LineType> ();
                List<AnnotatedString> individualStatements = new List<AnnotatedString> ();

                while ((inputString = inputFile.ReadLine ()) != null)
                {
                    if (inputString.Length > 0)
                    { 
                        statementTypes.Clear ();
                        individualStatements.Clear ();

                        inputLineProc.ClassifyInputLine (inputString, ref statementTypes, ref individualStatements);

                        for (int i=0; i<individualStatements.Count; i++)
                        {
                            Console.Write (statementTypes [i] + ":  ");
                            Console.WriteLine ("\n" + individualStatements [i]);
                            //Console.WriteLine (individualStatements [i].Plain);
                            Console.WriteLine ("");
                        }

                        if (individualStatements.Count > 0)
                            Console.WriteLine ("==========================================");
                    }
                }

                inputFile.Close ();
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
                Print (ex.StackTrace);
            }
        }
    }
}
