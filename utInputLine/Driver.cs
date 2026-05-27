using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using PLMain;

using PLCommon;
using PLFileSystem;
using PLWorkspace;

using static PLMain.InputLineProcessor;

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

                InputLineProcessor inputLineProcessor = new InputLineProcessor (Print);

                while ((inputString = inputFile.ReadLine ()) != null)
                {
                    inputLineProcessor.ProcessString (inputString);
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
