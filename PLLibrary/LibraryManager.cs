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
        static readonly Dictionary<string, PLFunction>  MathFunctions = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, PLFunction>  IOFunctions   = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, PLFunction>  PlotFunctions = new Dictionary<string, PLFunction> ();
        static readonly Dictionary<string, PLFunction>  PlotCommands  = new Dictionary<string, PLFunction> ();

        static readonly List<string> ZeroArgFunctions = new List<string> (); // functions that can be invoked with no arguments

        static LibraryManager ()
        {
            //
            // Math functions
            //
            Dictionary<string, PLFunction> mathFuncts = FunctionLibrary.MathFunctions.GetBuiltInContents ();
            foreach (string str in mathFuncts.Keys) MathFunctions.Add (str, mathFuncts [str]);

            FunctionLibrary.MathFunctions.GetZeroArgNames (ZeroArgFunctions);

            mathFuncts = FunctionLibrary.MathFunctions.GetUserDefinedContents ();
            foreach (string str in mathFuncts.Keys) MathFunctions.Add (str, mathFuncts [str]);

            //
            // IO functions
            //
            Dictionary<string, PLFunction> ioFuncts = FunctionLibrary.IOFunctions.GetContents ();
            foreach (string str in ioFuncts.Keys) IOFunctions.Add (str, ioFuncts [str]);

            FunctionLibrary.IOFunctions.GetZeroArgNames (ZeroArgFunctions);

            //
            // Plot functions
            //
            Dictionary<string, PLFunction> plotFuncts = FunctionLibrary.PlotFunctions.GetFunctionContents ();
            foreach (string str in plotFuncts.Keys) PlotFunctions.Add (str, plotFuncts [str]);

            FunctionLibrary.PlotFunctions.GetZeroArgNames (ZeroArgFunctions);

            //
            // Plot commands
            //
            Dictionary<string, PLFunction> plotCmnds = FunctionLibrary.PlotFunctions.GetPlotCommands ();
            foreach (string str in plotCmnds.Keys) PlotCommands.Add (str, plotCmnds [str]);
        }

        //***************************************************************************************************

        public static bool IsZeroArgFunction (string fname)
        {
            return ZeroArgFunctions.Contains (fname);
        }

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

            if      (PlotCommands.ContainsKey         (str)) {type = SymbolicNameTypes.PlotCommand;}
            else if (MathFunctions.ContainsKey        (str)) {type = SymbolicNameTypes.Function;}
            else if (IOFunctions.ContainsKey          (str)) {type = SymbolicNameTypes.Function;}
            else if (PlotFunctions.ContainsKey        (str)) {type = SymbolicNameTypes.Function;}
            else if (MFileFunctionMgr.IsMFileFunction (str)) {type = SymbolicNameTypes.FunctionFile;}

            return type;
        }


        public static List<string> PartialMatch (string str)
        {
            List<string> matches = new List<string> ();

            foreach (string cmd in PlotCommands.Keys)  {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in MathFunctions.Keys) {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in IOFunctions.Keys)   {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in PlotFunctions.Keys) {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}

            //if (matches.Count > 0) matches.Add ("\n");
            return matches;
        }


        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************
        //
        // Evaluate - looks for a named function in a dictionary and if found, calls it
        //
        
        public static bool Contains (PLString name) 
        {
            if (MathFunctions.ContainsKey (name.Text))
                return true;

            if (IOFunctions.ContainsKey (name.Text))
                return true;

            if (PlotFunctions.ContainsKey (name.Text))
                return true;

            return false;
        }

        public static PLVariable Evaluate (PLString name, PLVariable args, ref bool forcePrint)
        {
            if (MathFunctions.ContainsKey (name.Text))
            {
                PLFunction func = MathFunctions [name.Text];
                return func (args);
            }

            if (IOFunctions.ContainsKey (name.Text))
            {
                if (name.Text == "disp")
                    forcePrint = true;

                PLFunction func = IOFunctions [name.Text];
                return func (args);
            }

            if (PlotFunctions.ContainsKey (name.Text))
            {
                PLFunction func = PlotFunctions [name.Text];
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

        public static PLVariable RunPlotCommand (PLString name, PLVariable args)
        {
            if (PlotCommands.ContainsKey (name.Text))
            {
                PLFunction func = PlotCommands [name.Text];
                return func (args);
            }
            else
                throw new Exception ("Plot Command " + name + " not found");
        }
    }
}
