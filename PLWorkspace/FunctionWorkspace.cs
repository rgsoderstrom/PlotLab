
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
                PLVariable var = callersWorkspace.Get (callersNames [i]);
                var.Name = functionsNames [i]; // change to local name
                Add (var); // store in local function workspace
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
                PLVariable var = Get (localNames [i]);
                var.Name = callersNames [i];
                callersWorkspace.Add (var);
            }
        }



    }
}
