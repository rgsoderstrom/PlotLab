
/*
    AnnotatedStringClassifier 
        - classify a single AnnotatedString
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

namespace PLMain
{

    public class AnnotatedStringClassifier
    {
        private bool inBlock = false;
        public  bool InBlock {get {return inBlock;}}

        public AnnotatedStringClassifier ()
        {

        }

        public InputLineType Classify (AnnotatedString annotatedString)
        {
            if (annotatedString.AlphanumericOnly)
            {
                if (FileSystem.IsScriptFile (annotatedString.Plain))
                    return InputLineType.ScriptFile;
            }

            if (SystemFunctions.WhatIs (annotatedString.FirstWord) == SymbolicNameTypes.SystemCommand)
                return InputLineType.SystemCommand;

            switch (LibraryManager.WhatIs (annotatedString.FirstWord))
            {
                case SymbolicNameTypes.SystemCommand:
                    return InputLineType.SystemCommand;

                case SymbolicNameTypes.Function:
                    return InputLineType.ExpressionTree;

                case SymbolicNameTypes.FunctionFile:
                    return InputLineType.FunctionFile;

                default:
                    break;
            } 


            if (annotatedString.AlphanumericOnly)
                return InputLineType.Unknown;

            return InputLineType.ExpressionTree;
        }

    }
}
