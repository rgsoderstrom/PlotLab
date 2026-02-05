
/*
    Workspace.cs - model of a worksoace for utInputLine
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal class Workspace : IWorkspace
    {
        List<string> definedVariables = new List<string> () {"a1" , "b12", "c123"};

        public bool IsDefined (string varName)
        {
            return definedVariables.Contains (varName);
        }
    }
}
