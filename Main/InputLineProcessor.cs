
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

        private StringClassifier classifier = new StringClassifier (); 

        public InputLineProcessor (PrintFunction pr)
        {
            Print = pr;
            //Block.Print = pr;  
            CleanedStrings = new CleanStringQueue ();
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
            CleanedStrings.Add (rawString);

            while (CleanedStrings.Count > 0)
            {
                string       cleaned = CleanedStrings.GetOldest;
                NestedString nested  = new NestedString (cleaned);

                InputLineType lineType = classifier.Classify (nested);

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
