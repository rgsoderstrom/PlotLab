
/*
    FunctionWorskace - created for use by a single function. 
                     - on function completion outputs are copied out and Wordspace is deleted
*/

using System;
using System.Collections.Generic;

using PLCommon;

namespace PLWorkspace
{
    internal class FunctionWorkspace : WorkspaceBaseClass
    {
        //*************************************************************

        // Creates workspace, then copies and renames source variables into this workspace 

        internal FunctionWorkspace (string             name,
                                    WorkspaceBaseClass sourceWorkspace, 
                                    List<string>       sourceNames, // names in the source workspace
                                    List<string>       destNames)   // parallel array of their names in this workspace
                                  : base (name)
        {
            if (sourceNames.Count != destNames.Count)
                throw new Exception ("In " + name + " source and local name lists not same length");

            for (int i=0; i<sourceNames.Count; i++)
            {
                PLVariable plv = null;

                // true if caller local variable passed to a function
                if (sourceWorkspace.Get (sourceNames [i], ref plv) == true) 
                { 
                    plv.Name = destNames [i]; // change to local name
                    Add (plv); // store in local function workspace
                }

                // check for a global variable passed to a function
                else if (Workspace.Global.Get (sourceNames [i], ref plv))
                {
                    plv.Name = destNames [i]; // change to local name
                    Add (plv); // store in local function workspace
                }

                else
                    throw new Exception ("Variable " + sourceNames [i] + " undefined");
            }
        }

        //*************************************************************

        // Copy and rename variables from this workspace to destination workspace

        internal void GetOutputs (WorkspaceBaseClass dstWorkspace,
                                  List<string>       dstNames,   // names in the caller's workspace
                                  List<string>       localNames) // parallel array of their names in this workspace
        {
            if (dstNames.Count != localNames.Count)
                throw new Exception ("In " + Name + " caller's namees and local names lists not same length");

            for (int i=0; i<dstNames.Count; i++)
            {
                PLVariable var = null;
                
                if (Get (localNames [i], ref var))
                { 
                    var.Name = dstNames [i];
                    dstWorkspace.Add (var);
                }

                else
                    throw new Exception ("Error copying output " + dstNames [i] + " to caller's workspace");
            }
        }
    }
}
