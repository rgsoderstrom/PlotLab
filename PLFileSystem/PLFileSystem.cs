using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

/*
    PLFileSystem - created as an Empty Project (.Net Framework)
                 - framework changed to .Net 4.8
                 - output type changed to Class Library
*/

namespace PLFileSystem
{
    public enum FileTypes {Unknown, ScriptFile, FunctionFile, TextFile}

    //************************************************************************************

    public static class FileSystem
    {
        private static PrintFunction Print;
        private static string baseFolder = "PlotLabV1";
        private static string StartupScript = "utStartup"; //

        // base directory is one level below user's Documents
        private static string baseDirectory = "";
        public  static string BaseDirectory {get {return baseDirectory;}
                                             private set {if (Directory.Exists (value)) currentDirectory = value;}}

        private static string currentDirectory = "";
        public  static string CurrentDirectory {get {return currentDirectory;}
                                                set {if (Directory.Exists (value)) currentDirectory = value;}}

        private static List<string> searchPath = new List<string> ();
        public  static List<string> SearchPath {get {return searchPath;}}

        public static string LogFileDirectory     {get {return BaseDirectory;}}
        public static string HistoryFileDirectory {get {return BaseDirectory;}}

        //***********************************************************************************
        //***********************************************************************************

        // constructor

        static FileSystem ()
        {
        }

        public static bool Open (PrintFunction pf)
        { 
            Print = pf;

            string myDocs = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments)
                         ?? throw new Exception ("Can't find user's \"Documents\" folder");
            BaseDirectory = myDocs + "\\" + baseFolder;

            if (Directory.Exists (BaseDirectory) == false)
            {
                Print ("Creating folder " + baseFolder + " in user's Documents folder");
                Directory.CreateDirectory (BaseDirectory);

                StreamWriter str = File.CreateText (StartupScript);
                str.Write ("% startup.m - this script runs automatically on start up");
                str.Close ();
            }

            SearchPath.Add (BaseDirectory);


            return true;
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

        //public static List<string> GetPathCopy ()
        //{
        //    List<string> pathCopy = new List<string> ();

        //    foreach (string str in searchPath)
        //        pathCopy.Add (new string (str.ToCharArray ()));

        //    return pathCopy;
        //}

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
    }
    
}
