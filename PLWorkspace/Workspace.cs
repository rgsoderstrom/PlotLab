
/*
    Workspace - Two components:
                    - a single GlobalWorkspace
                    - a stack of:
                        - one BaseWorkspace
                        - possibly one or more FunctionWorkspaces 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLWorkspace
{
    static public class Workspace
    {
        private  static readonly Stack<WorkspaceBase> workSpaces = new Stack<WorkspaceBase> ();
        internal static readonly GlobalWorkspace Global;  // secondary for retrieval. Must be explicitly specified for storage 

        static Workspace ()
        {
            workSpaces.Push (new BaseWorkspace ("Base1"));
            Global = new GlobalWorkspace ();
        }

        static private WorkspaceBase Current 
        {
            get 
            {
                return workSpaces.Peek ();
            }
        }

        //************************************************************************************

        static public PrintFunction Print
        {
            set
            { 
                WorkspaceBase.Print = value;
            }
        }

        //************************************************************************************
        //************************************************************************************

        // stack management

        static public void PushNew (string name, List<string> callersNames, List<string> functionsNames)
        {
            if (workSpaces.Count > 100)
                throw new Exception ("Workspace stack overflow");

            WorkspaceBase caller = workSpaces.Peek();
            workSpaces.Push (new FunctionWorkspace (name, caller, callersNames, functionsNames));
        }

        //************************************************************************************

        static public void PopFunction (List<string> callersNames, List<string> functionsNames)
        {
            if (workSpaces.Count == 1)
                throw new Exception ("Workspace stack underflow, attempt to pop base workspace");

            FunctionWorkspace function = workSpaces.Pop () as FunctionWorkspace;

            function.GetOutputs (Current,
                                 callersNames,    // parallel array of their names in this workspace
                                 functionsNames); // names in the caller's workspace
        }

        //************************************************************************************
        //************************************************************************************

        // low-level functions and commands

        static public bool Contains  (string var) {return Current.Contains (var) || Global.Contains (var);}
        static public bool IsDefined (string var) {return Contains (var);}

        static public void Add       (PLVariable var)  {Current.Add (var);}
        static public void AddGlobal (PLVariable var)  {Global.Add (var);}


        static public Dictionary<string, PLFunction> Functions {get             {return Current.Functions;}}
        static public PLVariable Evaluate (string    funcName, PLVariable args) {return Current.Evaluate (funcName, args);}
        static public PLVariable Evaluate (PLString  funcName, PLVariable args) {return Current.Evaluate (funcName, args);}


        static public PLVariable RunCommand (string cmnd, PLList args) {return Current.RunCommand (cmnd, args);}


        // check Current first. if not there check global 
        static public PLVariable Get (string name) 
        {
            PLVariable plv = null;

            if (Current.Get (name, ref plv)) return plv;
            if (Global.Get  (name, ref plv)) return plv;

            throw new Exception ("Cannot find " + name + " in Workspace");
        }


        static public void Dump ()  {Current.Dump ();}

        static public void Clear (PLVariable lst)  {Current.Clear (lst);} // one or several variables


        static public List<string> PartialMatch (string str)
        {
            //List<string> matches = new List<string> ();
            //return matches;

            return Current.PartialMatch (str);

        }

        static public SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = Current.WhatIs (str);

            if (type == SymbolicNameTypes.Unknown) 
                type = Global.WhatIs (str);

            return type;
        }


        //**********************************************************************************************

        static public void OverwriteSubmatrix (string name,            // name of matrix already in workspace
                                               int tlcRow, int tlcCol, // 1-based
                                               PLVariable var)         // new data to overwrite some of old
        {
            Current.OverwriteSubmatrix (name, tlcRow, tlcCol, var);
        }




    }
}
