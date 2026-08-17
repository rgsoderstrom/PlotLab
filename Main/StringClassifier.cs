/*
    StringClassifier 
        - classify a single input string
*/

using PLCommon;
using PLSystem;
using PLFileSystem;
using PLLibrary;
using PLWorkspace;

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
            string FirstWord = astr.FirstWord;

            // anything remaining unknown will be passed to the expression tree
            InputLineType defaultType = InputLineType.Unknown;

            // for lines with alphanumeric only
            if (astr.AlphanumericOnly)
            {
                // if it's a single word, check variables and scripts
                if (astr.SingleWord)
                {
                    if (Workspace.WhatIs (astr.Plain) == SymbolicNameTypes.Variable)
                        return InputLineType.VariableName;

                    if (FileSystem.IsScriptFile (astr.Plain))
                        return InputLineType.ScriptFile;

                    if (BlockManager.IsBlockName (astr.Plain))
                        return InputLineType.BlockName;
                }

                else
                {
                    if (LibraryManager.WhatIs (FirstWord) == SymbolicNameTypes.PlotCommand)
                        return InputLineType.PlotCommand;
                }
            }


            // see if the first word is a system command
            if (SystemFunctions.WhatIs (FirstWord) == SymbolicNameTypes.SystemCommand)
                return InputLineType.SystemCommand;

            //if (LibraryManager.WhatIs (FirstWord) == SymbolicNameTypes.PlotCommand)
            //    return InputLineType.PlotCommand;

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
