
/*
    WorkspaceBase.cs - abstract base class for:
                     - DefaultWorkspace
                     - GlobalWorkspace
                     - FunctionWorkspace
*/

using System;
using System.Collections.Generic;

using PLCommon;

namespace PLWorkspace
{
    abstract internal partial class WorkspaceBaseClass
    {
        protected readonly Dictionary<string, PLVariable> Variables = new Dictionary<string, PLVariable> ();
        internal  readonly Dictionary<string, PLFunction> Functions = new Dictionary<string, PLFunction> ();
        protected readonly Dictionary<string, BSFunction> Commands  = new Dictionary<string, BSFunction> ();

        internal readonly string Name;
        internal static PrintFunction Print = Console.Write;

        internal WorkspaceBaseClass (string name)
        {
            Name = name;
            AddCommands (Commands);
            AddFunctions (Functions);
        }

        //***************************************************************************************************

        //
        // Workspace Commands - invoked from command line or script but not
        //                      part of an expression
        //

        internal void AddCommands (Dictionary<string, BSFunction> dst)
        {
            dst.Add ("clear",  Clear);
            dst.Add ("who",    Who);
            dst.Add ("whos",   Whos);
            dst.Add ("dump",   Dump);
        }

        //
        // Workspace Functions - require an argument passed in, can be part
        //                       of a general expression
        //

        internal void AddFunctions (Dictionary<string, PLFunction> dst)
        { 
            dst.Add ("exists", Exists);
            dst.Add ("rows",   Rows);
            dst.Add ("cols",   Cols);
            dst.Add ("length", Length);
            dst.Add ("size",   Size);
        }

        //***************************************************************************************************

        internal bool RunCommand (string cmnd, string args)
        {
            if (Commands.ContainsKey (cmnd))
            {
                BSFunction func = Commands [cmnd];
                return func (args);
            }

            return false;
        }

        internal bool RunCommand (string cmnd)
        {
            if (Commands.ContainsKey (cmnd))
            {
                BSFunction func = Commands [cmnd];
                return func ("");
            }

            return false;
        }

        //internal PLVariable RunCommand (PLString cmnd, PLList args)
        //{
        //    return RunCommand (cmnd.Text, args);
        //}

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

        //*****************************************************************************************

        //internal void OverwriteSubmatrix (string name,            // name of matrix already in workspace
        //                                  int tlcRow, int tlcCol, // 1-based
        //                                  PLVariable newData)     // new data to overwrite some of old
        //{
        //    OverwriteSubmatrix (this,
        //                        name,           // name of matrix already in workspace
        //                        tlcRow, tlcCol, // 1-based
        //                        newData);       // new data to overwrite some of old

        //}



        //*****************************************************************************************
        //
        // See whether workspace contains a variable
        //
        internal virtual bool Contains (string var)
        {
            return Variables.ContainsKey (var);
        }

        internal virtual bool FunctionsContains (string var)
        {
            return Functions.ContainsKey (var);
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

        internal bool Clear (string select)
        {
            if (select != null)
            {
                string [] lst = select.Split (new char [] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);

                if (lst.Length == 0)
                    Variables.Clear ();

                else
                {
                    foreach (string str in lst)
                    {
                        if (str == "all")
                        {
                            Variables.Clear ();
                          //PLLibrary.MFileFunctionMgr.ClearCache ();
                            break;
                        }

                        if (Variables.ContainsKey (str))
                            Variables.Remove (str);
                    }
                }
            }

            else
                Variables.Clear ();

            return true;
        }

        //***********************************************************************************************

        internal bool Who (string _)
        {
            PrintKeys (Print);
            return true;
        }

        internal bool Whos (string _)
        {
            PrintKeysAndSizes (Print);
            return true; 
        }

        //internal PLVariable Dump (PLVariable _)
        //{
        //    return Dump ();
        //}

        internal bool Dump (string _)
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

            return true;
        }

    }
}


