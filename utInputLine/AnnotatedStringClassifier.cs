
///*
//    AnnotatedStringClassifier 
//        - classify a single AnnotatedString
//*/

//using PLCommon;
//using PLSystem;
//using PLFileSystem;
//using PLLibrary;
//using PLWorkspace;

//namespace PLMain
//{
//    public class AnnotatedStringClassifier
//    {
//        public AnnotatedStringClassifier ()
//        {

//        }

//        //***************************************************************************
//        //
//        // Classify () - return InputLineType for a single AnnotatedString
//        //
//        public InputLineType Classify (AnnotatedString annotatedString)
//        {
//            // anything we don't know what to do with will be passed to the expression tree
//            InputLineType defaultType = InputLineType.ExpressionTree;
            
//            // for lines with alphanumeric only
//            if (annotatedString.AlphanumericOnly)
//            {
//                // if it's a single word, check variables and scripts
//                if (annotatedString.SingleWord)
//                {
//                    if (Workspace.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.Variable)
//                        return InputLineType.VariableName;
            
//                    if (FileSystem.IsScriptFile (annotatedString.Plain))
//                        return InputLineType.ScriptFile;
//                }
//            }

//            // see if the first word is a system command
//            if (SystemFunctions.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.SystemCommand)
//                return InputLineType.SystemCommand;

//            if (LibraryManager.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.PlotCommand)
//                return InputLineType.PlotCommand;

//            if (Workspace.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.WorkspaceCommand)
//                return InputLineType.WorkspaceCommand;

//            //if (BlockManager.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.BlockStart)
//            //    return InputLineType.BlockStart;

//            //if (BlockManager.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.BlockEnd)
//            //    return InputLineType.BlockEnd;

//            return defaultType;
//        }

//    }
//}
