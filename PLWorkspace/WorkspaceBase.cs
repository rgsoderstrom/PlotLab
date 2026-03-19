
/*
    WorkspaceBase.cs - abstract base class for:
                     - BaseWorkspace
                     - GlobalWorkspace
                     - FunctionWorkspace
*/

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLWorkspace
{
    abstract internal class WorkspaceBase
    {
        protected readonly Dictionary<string, PLVariable> Variables = new Dictionary<string, PLVariable> ();
        protected readonly Dictionary<string, PLFunction> Commands  = new Dictionary<string, PLFunction> ();
        protected readonly Dictionary<string, PLFunction> Functions = new Dictionary<string, PLFunction> ();

        internal readonly string Name;
        internal static PrintFunction Print = Console.Write;

        internal WorkspaceBase (string name)
        {
            Name = name;

            Dictionary<string, PLFunction> workCmnds = GetCommands ();
            foreach (string str in workCmnds.Keys) Commands.Add (str, workCmnds [str]);

            Dictionary<string, PLFunction> workFuncs = GetFunctions ();
            foreach (string str in workFuncs.Keys) Functions.Add (str, workFuncs [str]);
        }

        //***************************************************************************************************

        //
        // Workspace Commands - no arguments passed in
        //

        internal Dictionary<string, PLFunction> GetCommands ()
        {
            return new Dictionary<string, PLFunction> ()
            {
                {"clear",  Clear},
                {"who",    Who},
                {"whos",   Whos},
                {"dump",   Dump},
            };
        }
        
        //
        // Workspace Functions - require arguments passed in
        //

        internal Dictionary<string, PLFunction> GetFunctions ()
        {
            return new Dictionary<string, PLFunction> ()
            {
                {"exists", Exists},
                {"rows",   WorkspaceUtils.Rows},
                {"cols",   WorkspaceUtils.Cols},
                {"length", WorkspaceUtils.Length},
                {"size",   WorkspaceUtils.Size},
            };
        }

        //***************************************************************************************************

        internal PLVariable RunCommand (string cmnd, PLList args)
        {
            if (Commands.ContainsKey (cmnd))
            {
                PLFunction func = Commands [cmnd];
                return func (args);
            }

            return new PLNull ();
        }

        internal PLVariable RunCommand (PLString cmnd, PLList args)
        {
            return RunCommand (cmnd.Text, args);
        }

        //***************************************************************************************************

        internal PLVariable Evaluate (string funcName, PLVariable args)
        {
            if (Functions.ContainsKey (funcName))
            {
                PLFunction func = Functions [funcName];
                return func (args);
            }

            throw new Exception ("Workspace function " + funcName + " not found");
        }

        internal PLVariable Evaluate (PLString funcName, PLVariable args)
        {
            return Evaluate (funcName.Text, args);
        }

        //***************************************************************************************************

        // invoked by "who"

        internal void PrintKeys (PrintFunction pf)
        {
            string ostr = "";

            System.Collections.IDictionary dict = Variables;

            foreach (string str in dict.Keys)
            {
                ostr += str;
                ostr += "\t";

                if (ostr.Length > 40)
                {
                    pf (ostr + "\n");
                    ostr = "";
                }
            }

            if (ostr.Length > 0)
                pf (ostr + "\n");
        }

        //***************************************************************************************************

        // invoked by "whos"

        internal void PrintKeysAndSizes (PrintFunction pf)
        {
            foreach (string str in Variables.Keys)
            {
                string printString = "";
                object val = Variables [str];
                string typeStr = val.GetType ().Name;

                typeStr = typeStr.Replace ("PL", "");
                typeStr = typeStr.ToLower ();

                if (typeStr.Length < 8)
                    typeStr = typeStr.PadRight (8);  // pad short strings so all line up

                printString += (str + "\t" + typeStr + "\t");

                if (val is PLRMatrix)
                {
                    PLRMatrix var = val as PLRMatrix;
                    printString += (string.Format ("    {0} x {1}", var.Rows, var.Cols));
                }

                if (val is PLCMatrix)
                {
                    PLCMatrix var = val as PLCMatrix;
                    printString += (string.Format ("    {0} x {1}", var.Rows, var.Cols));
                }

                pf (printString);
                pf ("\n");
            }
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************
        
        /// <summary>
        /// WhatIs - determine whether a name represents a variable, a workspace operation or neither
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        
        internal virtual SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = SymbolicNameTypes.Unknown;

            if      (Variables.ContainsKey (str)) {type = SymbolicNameTypes.Variable;}
            else if (Commands.ContainsKey  (str)) {type = SymbolicNameTypes.WorkspaceCommand;}
            else if (Functions.ContainsKey (str)) {type = SymbolicNameTypes.Function;}  // WorkspaceFunction

            return type;
        }

        internal virtual List<string> PartialMatch (string str)
        {
            List<string> matches = new List<string> ();

            foreach (string cmd in Variables.Keys) {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in Commands.Keys)  {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}
            foreach (string cmd in Functions.Keys) {if (cmd.StartsWith (str)) matches.Add (cmd + " ");}

            return matches;
        }

        //*****************************************************************************************
        //
        // Add or change a variable
        //
        internal void Add (PLVariable var)
        {
            if (Variables.ContainsKey (var.Name)) Variables [var.Name] = var;
            else Variables.Add (var.Name, var);
        }

        internal void OverwriteSubmatrix (string name,            // name of matrix already in workspace
                                          int tlcRow, int tlcCol, // 1-based
                                          PLVariable newData)     // new data to overwrite some of old
        {
            WorkspaceUtils.OverwriteSubmatrix (this,
                                               name,           // name of matrix already in workspace
                                               tlcRow, tlcCol, // 1-based
                                               newData);       // new data to overwrite some of old

        }



        //*****************************************************************************************
        //
        // See whether workspace contains a variable
        //
        internal bool Contains (string var)
        {
            return Variables.ContainsKey (var);
        }


        //*****************************************************************************************
        //
        // Read a variable
        //
        internal virtual bool Get (string name, ref PLVariable var)
        {
            if (Variables.ContainsKey (name))
            {
                var = Variables [name];
                return true;
            }

            return false;
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************

        internal PLVariable Clear (PLVariable sel)
        {
            if (sel != null)
            {
                PLList lst = sel as PLList;

                if (lst.Count == 0)
                    Variables.Clear ();

                else
                {
                    foreach (PLString str in lst)
                    {
                        if (str.Data == "all")
                        {
                            Variables.Clear ();
                          //PLLibrary.MFileFunctionMgr.ClearCache ();
                            break;
                        }

                        try
                        {
                            Variables.Remove (str.Data);
                        }

                        catch (KeyNotFoundException)
                        {

                        }
                    }
                }
            }

            else
                Variables.Clear ();

            return new PLNull ();
        }

        //***********************************************************************************************

        internal virtual PLVariable Exists (PLVariable arg)
        {
            if (arg != null)
            {
                PLString str = arg as PLString;
                if (Variables.ContainsKey (str.Text))
                    return new PLBool (true);
            }

            return new PLBool (false);
        }

        internal virtual bool Exists (string str)
        {
            return Variables.ContainsKey (str);
        }

        //***********************************************************************************************

        internal PLVariable Who (PLVariable _)
        {
            PrintKeys (Print);
            return new PLNull ();
        }

        internal PLVariable Whos (PLVariable _)
        {
            PrintKeysAndSizes (Print);
            return new PLNull (); 
        }

        internal PLVariable Dump (PLVariable _)
        {
            return Dump ();
        }

        internal PLVariable Dump ()
        {
            if (Name != null) Print (Name + " Workspace contents:\n");
            else              Print ("Workspace contents:\n");

            System.Collections.IDictionary dict = Variables;

            foreach (string str in dict.Keys)
            {
                string printString = "";
                object val = dict [str];
                string typeStr = val.GetType ().Name;

                typeStr = typeStr.Replace ("PL", "");
                typeStr = typeStr.ToLower ();

                if (typeStr.Length < 8)
                    typeStr = typeStr.PadRight (8);  // pad short strings so all line up

                printString += (str + "\t" + typeStr + "\t");

                if (val is PLRMatrix)
                {
                    PLRMatrix var = val as PLRMatrix;
                    printString += (string.Format ("    {0} x {1}", var.Rows, var.Cols));
                }

                Print (printString);
                Print (val.ToString ());
                Print ("\n");
            }

            return null;
        }

    }
}


