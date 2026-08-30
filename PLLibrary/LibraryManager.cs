using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunctionLibrary;

using PLCommon;
//using PLFileSystem;

namespace PLLibrary
{
    static public class LibraryManager
    {
        static readonly Dictionary<string, PLFunction> SigProcFunctions = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, PLFunction> MathFunctions    = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, PLFunction> IOFunctionsDict  = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, PLFunction> PlotFunctions    = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, BSFunction> PlotCommands     = new Dictionary<string, BSFunction> ();
        static readonly Dictionary<string, PZFunction> ZeroArgFunctions = new Dictionary<string, PZFunction> ();

        static public PrintFunction Print {set {IOFunctions.Print = value;}}

        static LibraryManager ()
        {
            //
            // Signal Processing functions
            //
            SignalProcessing.AddSignalProcessingFunctions (ref SigProcFunctions);

            //
            // Math functions
            //
            FunctionLibrary.MathFunctions.AddBuiltInMathFunctions (ref MathFunctions);
            FunctionLibrary.MathFunctions.AddUserDefinedContents (ref MathFunctions);

            //
            // IO functions
            //
             IOFunctions.AddIOFunctions (ref IOFunctionsDict);

            //
            // Plot functions
            //
            FunctionLibrary.PlotFunctions.AddPlotFunctions (ref PlotFunctions);

            //
            // Zero Argument functions
            //
            FunctionLibrary.IOFunctions.AddZeroArgIOFunctions     (ref ZeroArgFunctions);
            FunctionLibrary.MathFunctions.AddZeroArgMathFunctions (ref ZeroArgFunctions);
            FunctionLibrary.PlotFunctions.AddZeroArgFunctions     (ref ZeroArgFunctions);

            //
            // Plot commands
            //
            FunctionLibrary.PlotFunctions.AddPlotCommands (ref PlotCommands);
        }

        //***************************************************************************************************

        //public static bool IsZeroArgFunction (string fname)
        //{
        //    return ZeroArgFunctions.Contains (fname);
        //}

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************

        /// <summary>
        /// WhatIs - test for plot command or function, math function, IO function
        /// </summary>
        /// <param name="str">string containing a single word</param>
        /// <returns>The type or unknown</returns>

        public static SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = SymbolicNameTypes.Unknown;

            if      (false) /*(PlotCommands.ContainsKey         (str))*/ {type = SymbolicNameTypes.PlotCommand;}
            else if (MathFunctions.ContainsKey        (str)) {type = SymbolicNameTypes.Function;}
            else if (SigProcFunctions.ContainsKey     (str)) {type = SymbolicNameTypes.Function;}
            else if (IOFunctionsDict.ContainsKey      (str)) {type = SymbolicNameTypes.Function;}
            else if (PlotFunctions.ContainsKey        (str)) {type = SymbolicNameTypes.Function;}
       //   else if (MFileFunctionMgr.IsMFileFunction (str)) {type = SymbolicNameTypes.FunctionFile;}

            return type;
        }

        public static bool IsFunctionWithArgs (string str)
        {
            if (MathFunctions.ContainsKey    (str)) return true;
            if (SigProcFunctions.ContainsKey (str)) return true;
            if (IOFunctionsDict.ContainsKey  (str)) return true;
            if (PlotFunctions.ContainsKey    (str)) return true;

            return false;
        }

        public static bool IsZeroArgFunction (string str)
        {
            return ZeroArgFunctions.ContainsKey (str);
        }

        public static bool IsPlotCommand (string str)
        {
            return PlotCommands.ContainsKey (str);
        }

        public static PLVariable RunZeroArgFunction (string fname)
        {
            if (ZeroArgFunctions.ContainsKey (fname))
            { 
                PZFunction func = ZeroArgFunctions [fname];
                return func ();
            }

            else
                throw new Exception ("Function " + fname + " can't be invoked with no arguments");
        }

        public static List<string> PartialMatch (string str)
        {
            List<string> matches = new List<string> ();

            foreach (string cmd in PlotCommands.Keys)     {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in MathFunctions.Keys)    {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in SigProcFunctions.Keys) {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in IOFunctionsDict.Keys)  {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in PlotFunctions.Keys)    {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}

            //if (matches.Count > 0) matches.Add ("\n");
            return matches;
        }


        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************
        //
        // Contains - looks for a named function in a dictionary and if found, returns "true"
        // Evaluate - looks for a named function in a dictionary and if found, calls it
        //
        
        //public static bool Contains (string name) 
        //{
        //    throw new Exception ("Contains - obsolete");

        //    //if (SigProcFunctions.ContainsKey (name))
        //    //    return true;

        //    //if (MathFunctions.ContainsKey (name))
        //    //    return true;

        //    //if (IOFunctionsDict.ContainsKey (name))
        //    //    return true;

        //    //if (PlotFunctions.ContainsKey (name))
        //    //    return true;

        //    //return false;
        //}

        public static PLVariable Evaluate (string name, PLVariable args)
        {
            if (SigProcFunctions.ContainsKey (name))
            {
                PLFunction func = SigProcFunctions [name];
                return func (args);
            }

            if (MathFunctions.ContainsKey (name))
            {
                PLFunction func = MathFunctions [name];
                return func (args);
            }

            if (IOFunctionsDict.ContainsKey (name))
            {
                PLFunction func = IOFunctionsDict [name];
                return func (args);
            }

            if (PlotFunctions.ContainsKey (name))
            {
                PLFunction func = PlotFunctions [name];
                return func (args);
            }

            return new PLNull (); // throw new Exception (string.Format ("Unknown function: {0}", name));
        }

        public static PLFunction GetFunctionDelegate (string name)
        {
            if (MathFunctions.ContainsKey (name))
            {
                PLFunction func = MathFunctions [name];
                return func;
            }

            if (PlotFunctions.ContainsKey (name))
            {
                PLFunction func = PlotFunctions [name];
                return func;
            }

            throw new Exception ("Function " + name + " not found");
        }

        //***************************************************************************************************

        public static bool RunPlotCommand (string name, string args)
        {
            if (PlotCommands.ContainsKey (name))
            {
                BSFunction func = PlotCommands [name];
                return func (args);
            }
            else
                throw new Exception ("Plot Command " + name + " not found");
        }
    }
}
