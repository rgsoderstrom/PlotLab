
/*
    FunctionWorskace - created for use by a single function. 
                     - on function completion outputs are copied out and Wordspace is deleted
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLWorkspace
{
    public class FunctionWorkspace : Workspace
    {
        //*************************************************************

        // Copy and rename function input arguments

        public FunctionWorkspace (Workspace src, 
                                  List<string> sourceVariables,  // names in the source workspace
                                  List<string> localVariables)   // parallel array of their names in this workspace
        {

        }

        //*************************************************************

        // Copy and rename function output arguments

        public void Get (Workspace dst,
                         List<string> destVariables,  // names in the destination workspace
                         List<string> localVariables)   // parallel array of their names in this workspace
        {

        }

    }
}
