
/*
    InputLineProcessor.cs -
*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using PLCommon;
using PLFileSystem;
using PLLibrary;
using PLWorkspace;


// all Block references commented out


namespace PLMain
{
    public partial class InputLineProcessor
    {
        static private PrintFunction Print;

        // queue of strings for processing
        private readonly CleanStringQueue CleanedStrings;
        private readonly AnnotatedStringSet AnnotatedStrings;

        private StringClassifier classifier = new StringClassifier (); 

        public InputLineProcessor (PrintFunction pr)
        {
            Print = pr;
            //Block.Print = pr;  
            CleanedStrings = new CleanStringQueue ();
            AnnotatedStrings = new AnnotatedStringSet ();
        }

        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************

        //
        // Passed a "raw" string entered by user, pasted in or read from a .m file
        //  - may contain prompt, a comment and extra spaces
        //

        public void ProcessString (string rawString)
        {
            bool somethingAdded = CleanedStrings.Add (rawString);

            if (somethingAdded == false) // a blank line or a comment line
                return;

            Console.WriteLine ("=====================");

            while (CleanedStrings.Count > 0)
            {
                AnnotatedString astr2 = null;
                string          cleaned = CleanedStrings.GetOldest;
                AnnotatedString astr  = new AnnotatedString (cleaned);
                AnnotatedStrings.Add (astr);

                while (AnnotatedStrings.Count > 0)
                {
                    astr2 = AnnotatedStrings.GetOldest ();
                    astr2.CheckForTrailingSemi ();

                    Console.WriteLine ("\n" + astr2);                    
                }

                if (astr2 == null)
                    return;

                InputLineType lineType = classifier.Classify (astr2);

                Console.WriteLine ("line type = " + lineType);


                //if (BlockManager.BlockCollectionInProgress)
                //{ 
                //    BlockManager.Add (cleaned, lineType);
                //}

                //else
                if (true)
                { 
                    switch (lineType)
                    {
                        case InputLineType.Unknown:
                        case InputLineType.ExpressionTree:
                        //    Print ("ExpressionTree: " + cleaned);
                            break;

                        case InputLineType.VariableName:
                            break;

                        case InputLineType.SystemCommand:
                            break;

                        case InputLineType.PlotCommand:
                            break;

                        case InputLineType.WorkspaceCommand:
                            break;

                        case InputLineType.ScriptFile:
                      //      Print ("Script: " + cleaned);
                            break;

                        case InputLineType.BlockStart:
                          //  BlockManager.StartNewBlock (cleaned);
                            break;

                        case InputLineType.BlockEnd:
                            throw new Exception ("Error: \"end\" outside of block not allowed");

                        default: throw new Exception ("Unsupported InputLineType: " + lineType);
                    }
                }


            }
        }

    }
}
