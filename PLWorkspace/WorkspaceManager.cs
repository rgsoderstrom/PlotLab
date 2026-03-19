
/*
    WorkspaceManager - container for the two work spaces in use at any time
                     - typically either
                        - global and base workspaces
                        - global and a function workspace

*/

/******************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLWorkspace
{
    internal class WorkspaceManager
    {
        private readonly WorkspaceBase Working; // primary, either base or a local workspace created for a function
        private static   WorkspaceBase Global;  // secondary for retrieval. Must be explicitly specified for storage 

        //**************************************************************************

        internal static void SetGlobalWorkspace (GlobalWorkspace ws)
        {
            Global = ws;
        }

        //**************************************************************************

        internal WorkspaceManager (WorkspaceBase primary)
        {
            Working = primary;
        }

        //**************************************************************************

        internal PLVariable Get (string name)
        { 
            if (Working.Exists (name))   
                return Working.Get (name);// as PLDouble;

            else if (Global.Exists (name)) 
                return Global.Get (name);// as PLDouble;
                
            else throw new Exception (name + " not found");
        }

        //*****************************************************************************************
        //
        // Add or change a variable
        //
        internal void Add (PLVariable var)
        {
            Working.Add (var);
        }

        internal void AddGlobal (PLVariable var)
        {
            Global.Add (var);
        }


    }
}

**************************/


