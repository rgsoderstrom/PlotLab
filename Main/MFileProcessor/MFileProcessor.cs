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
                string fileLine;

                while ((fileLine = file.ReadLine ()) != null)
                {
                    if (fileLine.Length > 0)
                    {
                      //  fileLine = ReplaceTabs (fileLine);
                        fileLine = RemoveComments (fileLine);

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

        //// replace tabs with single space

        //private static string ReplaceTabs (string lineIn)
        //{
        //    if (lineIn.Length > 0)
        //    {
        //        int index, count = 0;

        //        while ((index = lineIn.IndexOf ('\t')) >= 0) 
        //        {
        //            lineIn = lineIn.Remove (index, 1);
        //            lineIn = lineIn.Insert (index, " ");

        //            if (count++ > 100) 
        //                throw new Exception ("Error removing tabs from .m line");
        //        }

        //        lineIn = lineIn.Trim ();
        //    }

        //    return lineIn;
        //}

        //***************************************************

        // remove lines that are just comments. Comments following code
        // are removed later

        private static string RemoveComments (string lineIn)
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
