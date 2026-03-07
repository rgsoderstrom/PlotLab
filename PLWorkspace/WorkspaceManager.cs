
/*
    WorkspaceManager - container for the two work spaces in use at any time
                     - typically either
                        - global and base workspaces
                        - global and a function workspace

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLWorkspace
{
    public class WorkspaceManager
    {
        Workspace Primary;  // either base or a function worksace
        Workspace Seconday; // global 

        public WorkspaceManager (Workspace primary, Workspace secondary)
        {

        }
    }
}
