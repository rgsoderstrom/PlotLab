
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
            CleanedStrings = new CleanStringQueue ();
            AnnotatedStrings = new AnnotatedStringSet ();
        }

        public InputLineProcessor () : this (null)
        {
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

            if (somethingAdded == false) // if a blank line or a comment line was passed in
                return;                  // just return

            while (CleanedStrings.Count > 0)
            {
                AnnotatedString astr2 = null;
                string          cleaned = CleanedStrings.GetOldest ();
                AnnotatedString astr  = new AnnotatedString (cleaned);
                AnnotatedStrings.Add (astr);

                while (AnnotatedStrings.Count > 0)
                {
                    astr2 = AnnotatedStrings.GetOldest ();
                    astr2.CheckForTrailingSemi ();

                    if (astr2 == null)
                        return;

                    if (BlockManager.BlockCollectionInProgress)
                    {
                        BlockManager.Add (astr2);
                    }

                    else
                    { 
                        string str = astr2.Plain;
                        InputLineType lineType = classifier.Classify (astr2);

                        switch (lineType)
                        {
                            case InputLineType.Unknown:
                            case InputLineType.ExpressionTree:
                                Print?.Invoke ("ExpressionTree: " + astr2.Plain);
                                break;

                            case InputLineType.VariableName:
                                Print?.Invoke ("Variable: " + astr2.Plain);
                                break;

                            case InputLineType.SystemCommand:
                                Print?.Invoke ("SystemCommand: " + astr2.Plain);
                                break;

                            case InputLineType.PlotCommand:
                                Print?.Invoke ("PlotCommand: " + astr2.Plain);
                                break;

                            case InputLineType.WorkspaceCommand:
                                Print?.Invoke ("WorkspaceCommand: " + astr2.Plain);
                                break;

                            case InputLineType.ScriptFile:
                                Print?.Invoke ("Script: " + astr2.Plain);
                                break;

                            case InputLineType.BlockStart:
                                BlockManager.StartNewBlock (astr2);
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
}
