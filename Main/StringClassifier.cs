/*
    StringClassifier 
        - classify a single input string
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLSystem;
using PLFileSystem;
using PLLibrary;
using PLWorkspace;


// all Block references commented out


namespace PLMain
{
    public class StringClassifier
    {
        public StringClassifier ()
        {

        }

        //***************************************************************************
        //
        // Classify () - return InputLineType for a single string
        //
        public InputLineType Classify (AnnotatedString astr)
        {
            // anything we don't know what to do with will be passed to the expression tree
            InputLineType defaultType = InputLineType.Unknown;//ExpressionTree;

            //Console.WriteLine ("A");

            // for lines with alphanumeric only
            if (astr.AlphanumericOnly)
            {
                //Console.WriteLine ("B");

                // if it's a single word, check variables and scripts
                if (astr.SingleWord)
                {
                    //Console.WriteLine ("C");

                    if (Workspace.WhatIs (astr.Plain) == SymbolicNameTypes.Variable)
                        return InputLineType.VariableName;

                    if (FileSystem.IsScriptFile (astr.Plain))
                        return InputLineType.ScriptFile;

                    //Console.WriteLine ("ask BM");

                    if (BlockManager.IsBlockName (astr.Plain))
                        return InputLineType.BlockName;
                }
            }

            // alphanumeric only but more than one word
            string FirstWord = astr.FirstWord;

            // see if the first word is a system command
            if (SystemFunctions.WhatIs (FirstWord) == SymbolicNameTypes.SystemCommand)
                return InputLineType.SystemCommand;

            if (LibraryManager.WhatIs (FirstWord) == SymbolicNameTypes.PlotCommand)
                return InputLineType.PlotCommand;

            if (Workspace.WhatIs (FirstWord) == SymbolicNameTypes.WorkspaceCommand)
                return InputLineType.WorkspaceCommand;

            if (BlockManager.WhatIs (FirstWord) == SymbolicNameTypes.BlockStart)
                return InputLineType.BlockStart;

            if (BlockManager.WhatIs (FirstWord) == SymbolicNameTypes.BlockEnd)
                return InputLineType.BlockEnd;

            return defaultType;
        }
    }
}
