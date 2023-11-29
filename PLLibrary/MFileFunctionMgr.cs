using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



        // ENHANCEMENT: add a cache of previously found files so I don't have to
        //              search for the same file twice



namespace PLLibrary
{
    static public class MFileFunctionMgr
    {
        static string currentDir; // copy, passed down from Main

        static public string CurrentDir
        {
            get { return currentDir; }
            set { currentDir = value; }
        }

        static List<string> pathCopy; // copy, passed down from Main

        static public List<string> SearchPathCopy
        {
            get {return pathCopy;}
            set {pathCopy = value;}
        }

        //*****************************************************************************
        //*****************************************************************************
        //*****************************************************************************

        static List<MFileFunctionProcessor> MFiles = new List<MFileFunctionProcessor> (); // SHOUD BE A DICTIONARY<>

        public static MFileFunctionProcessor ParseMFile (string funcName, string fullName)
        {
            MFileFunctionProcessor proc = new MFileFunctionProcessor (funcName, fullName);
            MFiles.Add (proc);
            return proc;
        }




        //*****************************************************************************
        //
        // IsMFile - search current dir and all path dirs for a function file
        //           of this name
        //
        public static bool IsMFileFunction (string name)
        {
            string unused = null;
            return IsMFileFunction (name, ref unused);
        }

        public static bool IsMFileFunction (string name, ref string fullPath)
        {
            string full = CurrentDir + "\\" + name + ".m";
            fullPath = null;

            if (File.Exists (full))
            {
                if (IsFunctionFile (full))
                {
                    fullPath = full;
                    return true;
                }
            }

            if (SearchPathCopy != null)
            {
                foreach (string d in SearchPathCopy)
                {
                    full = d + "\\" + name + ".m";

                    if (File.Exists (full))
                    {
                        if (IsFunctionFile (full))
                        {
                            fullPath = full;
                            return true;
                        }
                    }
                }
            }
           
            return false;
        }

        //*****************************************************************************
        //
        // IsFunctionFile - opens passed-in file and checks for
        //                  correct "function" syntax
        //
        private static bool IsFunctionFile (string fullName)
        {
            bool isFunction = false;

            StreamReader file = new StreamReader (fullName);
            string raw;

            while ((raw = file.ReadLine ()) != null)
            {
                if (raw.Length > 0)
                {
                    string [] tokens = raw.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens [0] [0] == '%')
                        continue;

                    if (tokens [0] == "function")
                        isFunction = true;

                    break;
                }
            }

            file.Close ();
            return isFunction;
        }

    }
}




