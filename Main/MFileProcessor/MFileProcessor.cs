using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

using PLFileSystem;

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
        // Passed full file name
        //    - including path and extension
        //
        private List<string> ReadInputFile (string fullName)
        {
            try
            {
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
                                throw new Exception ("Error removing tabs from .m line");
                        }

                        fileLines.Add (raw);
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
    }
}
