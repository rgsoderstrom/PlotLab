using System;
using System.Collections.Generic;
using System.IO;

using PLCommon;

using PLFileSystem;

using PLWorkspace;

namespace PLMain
{
    public class ScriptProcessor : MFileProcessor
    {
        public ScriptProcessor (string scriptName) : base (scriptName)
        {
            InputLineProcessor ip = new InputLineProcessor (Print);

            foreach (string raw in fileLines)
            { 
                if (raw.StartsWith ("return"))
                    break;

                PLVariable ans = new PLNull ();
                ip.ProcessString (ref ans, raw);

                bool t1 = ans != null;
                bool t2 = ans is PLNull == false;
                bool t3 = ans is PLCanvasObject == false;
                bool t4 = ans is PLViewportObject == false;

                if (t1 & t2 & t3 & t4)
                {
                    ans.Name = "ans";
                    Workspace.Add (ans);

                    if (ip.SupressPrinting == false)
                    {
                        Print (ans.ToString ());
                        Print ("\n");
                    }
                }
            }
        }
    }
}











