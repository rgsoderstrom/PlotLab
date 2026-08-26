
/*
    Workspace - Container and public interface
              - Two components:
                    - a stack of:
                        - DefaultWorkspace
                        - possibly one or more FunctionWorkspaces 
                        - top-of-stack is "current" workspace
                    - a single GlobalWorkspace
                        - searched if a variable is not found in current workspace
*/

using System;
using System.Collections.Generic;

using PLCommon;

namespace PLWorkspace
{
    static public class Workspace
    {
        private  static readonly Stack<WorkspaceBaseClass> workSpaceStack = new Stack<WorkspaceBaseClass> ();
        internal static readonly GlobalWorkspace Global;  // secondary for retrieval. Must be explicitly specified for storage 

        static Workspace ()
        {
            workSpaceStack.Push (new DefaultWorkspace ("Default"));
            Global = new GlobalWorkspace ();
        }

        static private WorkspaceBaseClass Current {get {return workSpaceStack.Peek ();}}
        static public  PrintFunction Print   {set {WorkspaceBaseClass.Print = value;}}

        //************************************************************************************
        //
        // Write or read variables
        //

        // Add () - add to Current workspace
        static public void Add (PLVariable var)  
        {
            Current.Add (var);
        }

        // Contains () - test whether a variable is defined
        static public bool IsVariable (string str) 
        {
            return Current.Contains (str) || Global.Contains (str);
        }

        // AddGlobal () - add to global workspace
        static public void AddGlobal (PLVariable var) 
        {
            Global.Add (var);
        }

        // Get () - read and return a variable
        static public PLVariable Get (string name) 
        {
            PLVariable plv = null;

            if (Current.Get (name, ref plv)) return plv; // check Current first. if not there check global 
            if (Global.Get  (name, ref plv)) return plv;

            throw new Exception ("Cannot find " + name + " in Workspace");
        }


        //************************************************************************************
        //************************************************************************************
        //************************************************************************************

        // stack management

        static public void Push (string name, 
                                 List<string> currentNames, 
                                 List<string> localNames)
        {
            if (workSpaceStack.Count > 100)
                throw new Exception ("Workspace stack overflow");

            workSpaceStack.Push (new FunctionWorkspace (name, Current, currentNames, localNames));
        }

        //************************************************************************************

        static public void Pop (List<string> callersNames, List<string> functionsNames)
        {
            if (workSpaceStack.Count == 1)
                throw new Exception ("Workspace stack underflow, attempt to pop base workspace");

            FunctionWorkspace function = workSpaceStack.Pop () as FunctionWorkspace;

            function.GetOutputs (Current,         // copy into Current
                                 callersNames,    // names in Current
                                 functionsNames); // names in function
        }

        //************************************************************************************
        //************************************************************************************
        //************************************************************************************



        // static public Dictionary<string, PLFunction> Functions {get             {return Current.Functions;}}


        static public PLVariable EvaluateFunction(string funcName, PLVariable args) 
        {
            return Current.Evaluate (funcName, args);
        }

        //static public PLVariable EvaluateFunction (PLString funcName, PLVariable args) 
        //{
        //    return Current.Evaluate (funcName, args);
        //}

        //static public SymbolicNameTypes WhatIs (string str)
        //{
        //    SymbolicNameTypes type = Current.WhatIs (str);

        //    if (type == SymbolicNameTypes.Unknown) 
        //        type = Global.WhatIs (str);

        //    return type;
        //}




        // Workspace commands print information on things in the
        // workspace, e.g. whos

        static public bool RunCommand (string cmnd, string args) 
        {
            return Current.RunCommand (cmnd, args);
        }

        static public bool RunCommand (string cmnd) 
        {
            return Current.RunCommand (cmnd);
        }



        static public List<string> PartialMatch (string str)
        {
            return Current.PartialMatch (str);
        }





        //**********************************************************************************************

        //static public void OverwriteSubmatrix (string name,            // name of matrix already in workspace
        //                                       int tlcRow, int tlcCol, // 1-based
        //                                       PLVariable var)         // new data to overwrite some of old
        //{
        //    Current.OverwriteSubmatrix (name, tlcRow, tlcCol, var);
        //}




    }
}
