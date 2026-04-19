
/*
    PLFileSystem - created as an Empty Project (.Net Framework)
                 - framework changed to .Net 4.8
                 - output type changed to Class Library
*/

using System;
using System.Collections.Generic;
using System.IO;

using PLCommon;

namespace PLFileSystem
{
    public enum FileTypes {Unknown, ScriptFile, FunctionFile, TextFile}

    //************************************************************************************

    public static class FileSystem
    {
        private static PrintFunction Print;
        private static readonly string baseFolder = "PlotLabV2";
        private static readonly string startupScript = "startup.m";
        public  static          string StartupScript {get {return startupScript;}}

        // base directory is one level below user's Documents
        private static string baseDirectory = "";
        public  static string BaseDirectory {get {return baseDirectory;}
                                             private set {baseDirectory = value;}}
        
        // scripts directory is one level below base, contains startup script
        private static string scriptDirectory = "";
        public  static string ScriptDirectory {get {return scriptDirectory;}
                                               private set {scriptDirectory = value;}}

        private static string currentDirectory = "";
        public  static string CurrentDirectory {get {return currentDirectory;}
                                                set {if (Directory.Exists (value)) currentDirectory = value;
                                                         else throw new Exception ("Dir " + value + "doesn't exist");}}

        private static List<string> searchPath = new List<string> ();
        public  static List<string> SearchPath {get {return searchPath;}}

        public static string LogFileDir            {get {return BaseDirectory;}}
        public static string CommandHistoryFileDir {get {return BaseDirectory;}}

        //***********************************************************************************
        //***********************************************************************************

        // constructor & initialization

        static FileSystem ()
        {
        }

        public static void Open (PrintFunction pf)
        { 
            try
            { 
                Print = pf;

                string myDocs = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments)
                             ?? throw new Exception ("Can't find user's \"Documents\" folder");

                BaseDirectory   = myDocs + "\\" + baseFolder;
                ScriptDirectory = BaseDirectory + "\\" + "Scripts";

                if (Directory.Exists (BaseDirectory) == false)
                {
                    Print ("Creating folder " + baseFolder + " in user's Documents folder");
                    Directory.CreateDirectory (BaseDirectory);

                    Directory.CreateDirectory (ScriptDirectory);

                    StreamWriter str = File.CreateText (ScriptDirectory + "\\" + StartupScript);
                    str.Write ("\n% startup.m - this script runs automatically on start up\n");
                    str.Close ();
                }

                CurrentDirectory = BaseDirectory;
                AddPath (ScriptDirectory);
            }

            catch (Exception ex)
            {
                throw new Exception ("Error opening file system: " + ex.Message);
            }
        }

        //*******************************************************************************************
        //
        // Add directory to search path
        //  - if a directory is already in the path remove the old entry and re-add to the head. this
        //    gives highest priority to newest additions
        //
        public static bool AddPath (string path)
        {
            if (searchPath.Contains (path) == true)
                searchPath.Remove (path);

            searchPath.Insert (0, path);

            return true;
        }

        //*******************************************************************************************
        //
        // Check cwd and search path to see if file exists. if so, return its "path + name + ext"
        //   - cwd is searched first, then SearchPath, starting with newest addition (at the
        //     front of the list) and back to startup directory (at the end of the list)
        //
        public static bool NameSearch (string name, ref string fullName)
        {
            string full = currentDirectory + "\\" + name + ".m";

            if (File.Exists (full))
            {
                fullName = full;
                return true;
            }

            foreach (string d in searchPath)
            {
                full = d + "\\" + name + ".m";

                if (File.Exists (full))
                {
                    fullName = full;
                    return true;
                }
            }
           
            return false;
        }

        //*******************************************************************************
        //
        //  PartialDirectoryNameSearch - look for directories in the current directory that
        //                               match what has been typed
        //
        public static List<string> PartialDirectoryNameSearch (string name)
        {
            List<string> matches = new List<string> ();

            DirectoryInfo dir = new DirectoryInfo (currentDirectory);
            DirectoryInfo [] files = dir.GetDirectories (name + "*", SearchOption.TopDirectoryOnly);

            foreach (DirectoryInfo di in files)
            {
                matches.Add (di.Name);
            }

            return matches;
        }

        //*******************************************************************************
        //
        //  PartialNameSearch - look for .m files in the current directory and on the 
        //                      search path that match what has been typed
        //
        public static List<string> PartialNameSearch (string name)
        {
            List<string> matches = new List<string> ();

            string wildCardName = name + "*.m";
           // string wildCardName = name.ToLower () + "*.m";

            DirectoryInfo dir = new DirectoryInfo (currentDirectory);
            FileInfo [] files = dir.GetFiles (wildCardName, SearchOption.TopDirectoryOnly);

            foreach (FileInfo fi in files)
            {
                int index = fi.Name.IndexOf (".m");
                matches.Add (fi.Name.Remove (index) + " ");
            }

            foreach (string d in searchPath)
            {
                try
                {
                    dir = new DirectoryInfo (d);
                    files = dir.GetFiles (wildCardName, SearchOption.TopDirectoryOnly);

                    foreach (FileInfo fi in files)
                    {
                        int index = fi.Name.IndexOf (".m");
                        matches.Add (fi.Name.Remove (index) + " ");
                    }
                }

                catch (Exception) // probably an invalid name or dir doesn't exist
                {
                }
            }

            //if (matches.Count > 0) matches.Add ("\n");
            return matches;
        }

        //******************************************************************************************

        // return a copy, for client use, so actual path is protected
        public static List<string> GetPathCopy ()
        {
            List<string> pathCopy = new List<string> ();

            foreach (string str in searchPath)
                pathCopy.Add (new string (str.ToCharArray ()));

            return pathCopy;
        }

        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************

        public static FileTypes WhatIs (string name)
        {
            string fullName = "";
            MFileFunctionProcessor proc = null;
            FileTypes fileType = FileTypes.Unknown;

            if (NameSearch (name, ref fullName) == true)
            {
                if (MFileFunctionMgr.IsMFileFunction (name, ref fullName, ref proc))
                    fileType = FileTypes.FunctionFile;
                else
                    fileType = FileTypes.ScriptFile;
            }

            return fileType;
        }
    
        public static bool IsScriptFile (string name)
        {
            return WhatIs (name) == FileTypes.ScriptFile;
        }
    
        public static bool IsFunctionFile (string name)
        {
            return WhatIs (name) == FileTypes.FunctionFile;
        }
    }
}
