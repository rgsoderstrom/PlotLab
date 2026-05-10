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

                InputLineProcessor inputLineProc = new InputLineProcessor ();
                AnnotatedStringClassifier classifier = new AnnotatedStringClassifier ();

                while ((inputString = inputFile.ReadLine ()) != null)
                {
                    if (inputString.Length > 0)
                    { 
                        AnnotatedStringSet annotatedSet = BuildExpressions (inputString);

                        while (annotatedSet.Count > 0)
                        {
                            AnnotatedString annotated = annotatedSet.GetOldest;
                            InputLineType lineType = classifier.Classify (annotated);

                            Console.WriteLine (annotated.Plain);
                            Console.WriteLine ("  " + lineType.ToString ());
                            Console.WriteLine ();

                        }
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

        //**********************************************************************

        static private readonly string continuationString = "...";
        static private string cumulative = "";

        private static readonly AnnotatedStringSet annStringSet = new AnnotatedStringSet ();

        private static AnnotatedStringSet BuildExpressions (string fileLine)
        {
            string cleanedInput = InputLineProcessor.PreprocessInputLine (fileLine);

            if (cleanedInput.Length == 0)
                return annStringSet;

            bool continues = false;

            if (cleanedInput.EndsWith (continuationString))
            {
                cleanedInput = cleanedInput.Remove (cleanedInput.Length - continuationString.Length);
                continues = true;
            }                

            cumulative += cleanedInput;

            if (continues == true)
                return annStringSet;

            annStringSet.Add (new AnnotatedString (cumulative));
            cumulative = "";

            return annStringSet;
        }

    }
}
