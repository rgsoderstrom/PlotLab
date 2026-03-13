
/*
    Workspace - actually a stack of Workspaces 
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
        static private readonly Stack<WorkspaceBase> workSpaces = new Stack<WorkspaceBase> ();

        static Workspace ()
        {
            WorkspaceBase.Print = Console.Write;
            workSpaces.Push (new BaseWorkspace ("Base1"));
        }


        static private WorkspaceBase Current 
        {
            get 
            {
                if (workSpaces.Count == 0)
                    throw new Exception ("Workspace stack underflow");

                return workSpaces.Peek ();
            }
        }

        static public bool Contains (string var)
        {
            return Current.Contains (var);
        }

        static public void Add (PLVariable var)
        {
            Current.Add (var);
        }

        static public PLVariable Get (string name)
        {
            return Current.Get (name);
        }

        static public void Dump ()
        {
            Current.Dump ();
        }

        static public void PushNew (string name, List<string> callersNames, List<string> functionsNames)
        {
            if (workSpaces.Count > 100)
                throw new Exception ("Workspace stack overflow");

            WorkspaceBase caller = workSpaces.Peek();
            workSpaces.Push (new FunctionWorkspace (name, caller, callersNames, functionsNames));
        }

        static public void Pop (List<string> callersNames, List<string> functionsNames)
        {
            FunctionWorkspace function = workSpaces.Pop () as FunctionWorkspace;

            function.GetOutputs (Current,
                                 callersNames,    // parallel array of their names in this workspace
                                 functionsNames); // names in the caller's workspace
        }


    }
}
