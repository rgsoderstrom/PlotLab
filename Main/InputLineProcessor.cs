
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
    public enum TerminationReason {Completed, BreakEncountered, ContinueEncountered};

    public partial class InputLineProcessor
    {
        static public PrintFunction Print;

        public bool SupressPrinting {get; protected set;}

        // queue of strings for processing
        private readonly CleanStringQueue CleanedStrings;
        private readonly AnnotatedStringSet AnnotatedStrings;

        private StringClassifier classifier = new StringClassifier (); 

        public InputLineProcessor (PrintFunction pr) : this ()
        {
            Print = pr;
        }

        public InputLineProcessor ()
        {
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

        public TerminationReason ProcessString (ref PLVariable answer, string rawString)
        {
            bool somethingAdded = CleanedStrings.Add (rawString);

            // if a blank line or a comment line was passed in just return
            if (somethingAdded == false) 
                return TerminationReason.Completed;

            while (CleanedStrings.Count > 0)
            {
                string          cleaned = CleanedStrings.GetOldest ();
                AnnotatedString astr  = new AnnotatedString (cleaned);
                AnnotatedStrings.Add (astr);

                while (AnnotatedStrings.Count > 0)
                {
                    AnnotatedString astr2 = AnnotatedStrings.GetOldest ();
                    astr2.CheckForTrailingSemi ();

                    SupressPrinting = astr2.SupressPrinting;

                    if (astr2 == null)
                        return TerminationReason.Completed;

                    if (BlockManager.BlockCollectionInProgress)
                    {
                        BlockManager.Add (astr2);
                    }

                    else
                    { 
                        InputLineType lineType = classifier.Classify (astr2);

                        switch (lineType)
                        {
                            case InputLineType.Unknown:
                            case InputLineType.ExpressionTree:
                                ExpressionTree tree = new ExpressionTree (astr2);
                                answer = tree.Evaluate ();
                                break; 

                            case InputLineType.VariableName:
                                PLVariable v = Workspace.Get (astr2.Plain);
                                //Print?.Invoke (v.ToString () + "\n");
                                break;

                            case InputLineType.SystemCommand:
                                PLSystem.SystemFunctions.RunSystemCommand (astr2.FirstWord, astr2.Arguments);
                                //Print?.Invoke ("\n");
                                break;

                            case InputLineType.PlotCommand:
                                PLLibrary.LibraryManager.RunPlotCommand (astr2.FirstWord, astr2.Arguments);
                                //Print?.Invoke ("\n");
                                break;

                            case InputLineType.WorkspaceCommand: 
                                Workspace.RunCommand (astr2.FirstWord, astr2.Arguments);
                                break;

                            case InputLineType.ScriptFile:
                                //Print?.Invoke ("Script: " + astr2.Plain);
                                ScriptProcessor script = new ScriptProcessor (astr2.Plain);
                                //Print?.Invoke ("\n");
                                break;

                            case InputLineType.BlockName:
                                TerminationReason status = BlockManager.RunBlock (astr2);
                                Console.WriteLine ("BlockExit status = " + status);

                                if (status == TerminationReason.BreakEncountered)    return status;
                                if (status == TerminationReason.ContinueEncountered) return status;
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

            return TerminationReason.Completed;
        }
    }
}
