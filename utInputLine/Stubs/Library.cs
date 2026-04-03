
/*
    Library.cs - model of the built-in function library for utInputLine
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLLibrary
{
    static public class LibraryManager
    {
        static readonly List<string> PlotCommands     = new List<string> () { "clear", "close", "hold"};
        static readonly List<string> MathFunctions    = new List<string> () { "sin", "log2", "sqrt"};
        static readonly List<string> SigProcFunctions = new List<string> () { "fft", "filter"};
        static readonly List<string> IOFunctions      = new List<string> () { "disp", "format", "sprintf"};
        static readonly List<string> PlotFunctions    = new List<string> () { "plot", "text", "arrow"};
        static readonly List<string> MFileFunctions   = new List<string> () { "ShowSamples", "Fill", "arrow"}; // ???????

        static public bool Contains (string funcName)
        {
            return WhatIs (funcName) != SymbolicNameTypes.Unknown;
        }

        public static SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = SymbolicNameTypes.Unknown;

            if      (PlotCommands.Contains     (str)) {type = SymbolicNameTypes.PlotCommand;}
            else if (MathFunctions.Contains    (str)) {type = SymbolicNameTypes.Function;}
            else if (SigProcFunctions.Contains (str)) {type = SymbolicNameTypes.Function;}
            else if (IOFunctions.Contains      (str)) {type = SymbolicNameTypes.Function;}
            else if (PlotFunctions.Contains    (str)) {type = SymbolicNameTypes.Function;}
            else if (MFileFunctions.Contains   (str)) {type = SymbolicNameTypes.FunctionFile;}

            return type;
        }
    }
}
