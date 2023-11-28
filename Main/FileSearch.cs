using System;
using System.Collections.Generic;
using System.IO;

using PLCommon;
using PLLibrary;

namespace Main
{
    public static class FileSearch
    {
        private static string currentDirectory = "";

        public static string CurrentDirectory
        {
            get {return currentDirectory;}
            set 
            {
                if (Directory.Exists (value))
                    currentDirectory = value;
            }
        }

        static List<string> searchPath = new List<string> () 
        {
            
        };

        public static void Open ()
        {
            currentDirectory = UserConsole.ScriptsDirectory;
        }

        //*******************************************************************************************
        //
        // Add directory to search path
        //
        public static bool AddPath (string path)
        {
            if (searchPath.Contains (path) == false)
                searchPath.Add (path);

            return true;
        }

        //*******************************************************************************************
        //
        // Check cwd and search path to see if file exists. if so, return its "path + name + ext"
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

        public static List<string> PartialNameSearch (string name)
        {
            List<string> matches = new List<string> ();

            string wildCardName = name + "*.m";
           // string wildCardName = name.ToLower () + "*.m";

            DirectoryInfo dir = new DirectoryInfo (currentDirectory);
            FileInfo [] files = dir.GetFiles(wildCardName, SearchOption.TopDirectoryOnly);

            foreach (FileInfo fi in files)
            {
                int index = fi.Name.IndexOf (".m");
                matches.Add (fi.Name.Remove (index) + " ");
            }

            foreach (string d in searchPath)
            {
                dir = new DirectoryInfo (d);
                files = dir.GetFiles(wildCardName, SearchOption.TopDirectoryOnly);

                foreach (FileInfo fi in files)
                {
                    int index = fi.Name.IndexOf (".m");
                    matches.Add (fi.Name.Remove (index) + " ");
                }
            }

            //if (matches.Count > 0) matches.Add ("\n");
            return matches;
        }

        //******************************************************************************************

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

        public static SymbolicNameTypes WhatIs (string name)
        {
            string fullName = "";
            SymbolicNameTypes fileType = SymbolicNameTypes.Unknown;

            if (NameSearch (name, ref fullName) == true)
            {
                if (MFileFunctionMgr.IsMFileFunction (name, ref fullName))
                    fileType = SymbolicNameTypes.FunctionFile;
                else
                    fileType = SymbolicNameTypes.ScriptFile;
            }

            return fileType;
        }
    }
}
