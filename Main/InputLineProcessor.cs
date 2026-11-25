
/*
    InputLineProcessor.cs -
*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Common;
using PLCommon;
using PLFileSystem;
using PLLibrary;
using PLWorkspace;

namespace PLMain
{
    public enum BlockTerminationReason {Completed, BreakEncountered, ContinueEncountered, ReturnEncountered};

    public partial class InputLineProcessor
    {
        static public PrintFunction Print;

        public bool SupressPrinting {get; protected set;}

        // queue of strings for processing
        private readonly CleanStringQueue CleanedStrings;
        //private readonly AnnotatedStringSet AnnotatedStrings;

        private StringClassifier classifier = new StringClassifier (); 

        public InputLineProcessor (PrintFunction pr) : this ()
        {
            Print = pr;
        }

        public InputLineProcessor ()
        {
            CleanedStrings = new CleanStringQueue ();
            //AnnotatedStrings = new AnnotatedStringSet ();
        }

        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************

        //
        // Passed a "raw" string entered by user, pasted in or read from a .m file
        //  - may contain prompt, a comment and extra spaces
        //

        public BlockTerminationReason ProcessString (ref PLVariable answer, string rawString)
        {
            try
            { 
                bool somethingAdded = CleanedStrings.Add (rawString);

                // if a blank line or a comment line was passed in just return
                if (somethingAdded == false) 
                    return BlockTerminationReason.Completed;

                AnnotatedStringSet AnnotatedStrings = new AnnotatedStringSet ();

                while (CleanedStrings.Count > 0)
                {
                    string          cleaned = CleanedStrings.GetOldest ();
                    AnnotatedString astr  = new AnnotatedString (cleaned);
                    AnnotatedStrings.Add (astr);

                    foreach (AnnotatedString astr2 in AnnotatedStrings)
                    {
                        SupressPrinting = astr2.SupressPrinting;

                        if (astr2 == null)
                            return BlockTerminationReason.Completed;

                        if (BlockManager.BlockCollectionInProgress)
                        {
                            BlockTerminationReason btr = BlockManager.Add (astr2);

                            if (btr == BlockTerminationReason.ReturnEncountered)
                                return btr;
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

                                case InputLineType.ZeroArgFunction:
                                    answer = LibraryManager.RunZeroArgFunction (astr2.Plain);
                                    break;

                                case InputLineType.VariableName:
                                    PLVariable v = Workspace.Get (astr2.Plain);
                                    if (SupressPrinting == false)
                                        Print?.Invoke (v.ToString ());
                                    break;

                                case InputLineType.SystemCommand:
                                    PLSystem.SystemFunctions.RunSystemCommand (astr2.FirstWord, astr2.Arguments);
                                    break;

                                case InputLineType.PlotCommand:
                                    PLLibrary.LibraryManager.RunPlotCommand (astr2.FirstWord, astr2.Arguments);
                                    break;

                                case InputLineType.IOCommand:
                                    PLLibrary.LibraryManager.RunIOCommand (astr2.FirstWord, astr2.Arguments);
                                    break;

                                case InputLineType.WorkspaceCommand: 
                                    Workspace.RunCommand (astr2.FirstWord, astr2.Arguments);
                                    break;

                                case InputLineType.ScriptFile:
                                    ScriptProcessor script = new ScriptProcessor (astr2.Plain);
                                    script.Run ();
                                    break;

                                case InputLineType.BlockName:
                                    answer = null;
                                    BlockTerminationReason status = BlockManager.RunBlock (astr2);
                                    //Console.WriteLine ("BlockExit status = " + status);

                                    if (status == BlockTerminationReason.ReturnEncountered)   
                                        return status;

                                    if (status == BlockTerminationReason.BreakEncountered)    return status;
                                    if (status == BlockTerminationReason.ContinueEncountered) return status;
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

            catch (Exception ex)
            {
                EventLog.WriteLine ("Error in ProcessString: " + ex.Message);
                Print (ex.Message);
            }

            return BlockTerminationReason.Completed;
        }
    }
}
