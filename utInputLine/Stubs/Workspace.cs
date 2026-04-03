
/*
    Workspace.cs - model of a worksoace for utInputLine
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLWorkspace
{
    static public class Workspace
    {
        static private List<string> definedVariables = new List<string> () { "a1", "b12", "c123" };

        static public bool IsDefined (string varName)
        {
            return definedVariables.Contains (varName);
        }
    }
}
