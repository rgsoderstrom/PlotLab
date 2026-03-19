
/*
    FunctionWorskace - created for use by a single function. 
                     - on function completion outputs are copied out and Wordspace is deleted
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLWorkspace
{
    internal class FunctionWorkspace : WorkspaceBase
    {
        //*************************************************************

        // Copy and rename input arguments into this function's workspace 

        internal FunctionWorkspace (string        functionName,
                                    WorkspaceBase callersWorkspace, 
                                    List<string>  callersNames,   // names in the source workspace
                                    List<string>  functionsNames) // parallel array of their names in this workspace
                                  : base (functionName)
        {
            if (callersNames.Count != functionsNames.Count)
                throw new Exception ("In " + functionName + " source and local name lists not same length");

            for (int i=0; i<callersNames.Count; i++)
            {
                PLVariable vvar = null;

                // true if caller local variable passed to a function
                if (callersWorkspace.Get (callersNames [i], ref vvar) == true) 
                { 
                    vvar.Name = functionsNames [i]; // change to local name
                    Add (vvar); // store in local function workspace
                }

                // check for a global variable passed to a function
                else if (Workspace.Global.Get (callersNames [i], ref vvar))
                {
                    vvar.Name = functionsNames [i]; // change to local name
                    Add (vvar); // store in local function workspace
                }

                else
                    throw new Exception ("Variable " + callersNames [i] + " undefined");
            }
        }

        //*************************************************************

        // Copy and rename function's output args into caller's workspace

        internal void GetOutputs (WorkspaceBase callersWorkspace,
                                  List<string>  callersNames,  // names in the caller's workspace
                                  List<string>  localNames)    // parallel array of their names in this workspace
        {
            if (callersNames.Count != localNames.Count)
                throw new Exception ("In " + Name + " caller's namees and local names lists not same length");

            for (int i=0; i<callersNames.Count; i++)
            {
                PLVariable var = null;
                
                if (Get (localNames [i], ref var))
                { 
                    var.Name = callersNames [i];
                    callersWorkspace.Add (var);
                }

                else
                    throw new Exception ("Error copying output " + callersNames [i] + " to caller's workspace");
            }
        }



    }
}
