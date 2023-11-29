using System;
using System.Collections.Generic;
using System.IO;

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

        static Dictionary<string, MFileFunctionProcessor> MFileCache = new Dictionary<string, MFileFunctionProcessor> ();

        public static MFileFunctionProcessor ParseMFile (string funcName, string fullName)
        {
            MFileFunctionProcessor proc = new MFileFunctionProcessor (funcName, fullName);
            MFileCache.Add (funcName, proc);
            return proc;
        }

        public static void ClearCache ()
        {
            MFileCache.Clear ();
        }

        //*****************************************************************************
        //
        // IsMFile - search:
        //            - MFileCache
        //            - current dir
        //            - path dirs
        //           for a function file of this name
        //

        public static bool IsMFileFunction (string name)
        {
            string unused = null;
            MFileFunctionProcessor unused2 = null;
            return IsMFileFunction (name, ref unused, ref unused2);
        }

        //
        // IsMFileFunction
        //      - fills in fullPath or proc if "name" is found
        //
        public static bool IsMFileFunction (string name, ref string fullPath, ref MFileFunctionProcessor proc)
        {
            fullPath = null;
            proc = null;

            // check cache
            if (MFileCache.ContainsKey (name))
            {
                proc = MFileCache [name];
                return true;
            }

            // check current directory
            string full = CurrentDir + "\\" + name + ".m";

            if (File.Exists (full))
            {
                if (IsFunctionFile (full))
                {
                    fullPath = full;
                    return true;
                }
            }

            // check all directories in search path
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

                    if (tokens [0][0] == '%')
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




