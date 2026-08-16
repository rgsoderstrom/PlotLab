using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLFileSystem;
using PLWorkspace;

namespace PLMain
{
    public class ScriptProcessor
    {
        static public PrintFunction Print = null;

        public enum ScriptTerminationReason {Completed, Failed};

        public ScriptProcessor (string scriptName)
        {
            string fullName = "";

            if (FileSystem.NameSearch (scriptName, ref fullName) == false)
                throw new Exception ("Script " + scriptName + " not found");

            List<string> scriptLines = ReadInputFile (fullName);

            InputLineProcessor ip = new InputLineProcessor (Print);

            foreach (string raw in scriptLines)
            { 
                PLVariable ans = new PLNull ();
                ip.ProcessString (ref ans, raw);

                if (ans != null && ans is PLNull == false && ans is PLCanvasObject == false && ans is PLViewportObject == false)
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

        //*******************************************************************************
        //
        // Passed full name of a script
        //    - including path and extension
        //
        private List<string> ReadInputFile (string fullName)
        {
            try
            {
                List<string> scriptLines = new List<string> ();
                StreamReader file = new StreamReader (fullName);
                string raw;

                while ((raw = file.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        int index, count = 0;

                        while ((index = raw.IndexOf ('\t')) >= 0) // replace tabs with single space
                        {
                            raw = raw.Remove (index, 1);
                            raw = raw.Insert (index, " ");

                            if (count++ > 100) 
                                throw new Exception ("Error removing tabs from script line");
                        }

                        scriptLines.Add (raw);
                    }
                }

                file.Close ();
                return scriptLines;
            }

            catch (Exception ex)
            {
                throw new Exception ("Error reading script file: " + ex.Message);
            }
        }
    }
}











