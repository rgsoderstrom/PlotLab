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

        //*****************************************************************************
        //
        // ParseMFile - serparate into:
        //      - input formal parameters
        //      - executable script
        //      - output format parameters
        //
        
        public static List<string> InputFormalParams  = new List<string> ();
        public static List<string> ExecutableScript   = new List<string> ();
        public static List<string> OutputFormalParams = new List<string> ();

        enum ParseState {LookingForStart, InExecutable};
        static ParseState ps = ParseState.LookingForStart;

        public static void ParseMFile (string funcName, string fullName)
        {
            InputFormalParams.Clear ();
            ExecutableScript.Clear ();
            OutputFormalParams.Clear ();
            ps = ParseState.LookingForStart;

            using (StreamReader reader = new StreamReader (fullName))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string [] tokens = line.Split (new char [] { ' ', ',', '[', ']', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens [0][0] == '%')
                            continue;

                        if (ps == ParseState.LookingForStart)
                        {
                            if (tokens [0] == "function")
                            {
                                // find token that is just an "equal" sign
                                // all before it are output formal parameters
                                // one immediately after is function name
                                // all after that are input parameter names

                                int equalIndex = -1;

                                for (int i = 0; i<tokens.Length; i++) { if (tokens [i] == "=") {equalIndex = i; break; } }

                                if (equalIndex == -1) // not found
                                    throw new Exception ("Function syntax error, equal sign not found in " + funcName);

                                if (tokens [equalIndex + 1].ToLower () != funcName.ToLower ()) // ignore case
                                    throw new Exception ("Function name doesn't match file name in file " + funcName);

                                for (int i = 1; i<equalIndex; i++)
                                    OutputFormalParams.Add (tokens [i]);

                                for (int i = equalIndex + 2; i<tokens.Length; i++)
                                    InputFormalParams.Add (tokens [i]);

                                ps = ParseState.InExecutable;
                            }
                        }

                        else if (ps == ParseState.InExecutable)
                        {
                            ExecutableScript.Add (line);
                        }
                    }
                }
            }
        }
    }
}




