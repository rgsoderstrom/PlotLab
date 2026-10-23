using System;
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
    abstract public class MFileProcessor
    {
        static public PrintFunction Print = null;
        public enum TerminationReason {Completed, Failed};

        protected List<string> fileLines = new List<string> ();

        //*******************************************************************************
        //
        // Passed base file name
        //
        protected MFileProcessor (string baseName)
        {
            string fullName = "";

            if (FileSystem.NameSearch (baseName, ref fullName) == false)
                throw new Exception ("File " + baseName + " not found");

            fileLines = ReadInputFile (fullName);
        }

        //*******************************************************************************
        //
        // Execute each file line
        //
        public virtual void Run ()
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

        //*******************************************************************************
        //
        // Passed full file name
        //    - including path and extension
        //
        private List<string> ReadInputFile (string fullName)
        {
            try
            {
                StreamReader file = new StreamReader (fullName);
                string fileLine;

                while ((fileLine = file.ReadLine ()) != null)
                {
                    if (fileLine.Length > 0)
                    {
                        fileLine = RemoveCommentLines (fileLine);

                        if (fileLine.Length > 0) 
                            fileLines.Add (fileLine);
                    }
                }

                file.Close ();
                return fileLines;
            }

            catch (Exception ex)
            {
                throw new Exception ("Error reading script file: " + ex.Message);
            }
        }

        //***************************************************
        //
        // remove lines that are just comments. Comments following code
        // are removed later

        private static string RemoveCommentLines (string lineIn)
        {
            bool commentOnly = false;

            foreach (char c in lineIn)
            { 
                if (c == '%')
                {
                    commentOnly = true;
                    break;
                }

                if (char.IsWhiteSpace (c) == false)
                    break;
            }

            if (commentOnly == true)
                return "";

            return lineIn;
        }
    }
}
