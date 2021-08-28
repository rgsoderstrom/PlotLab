using System;
using System.Collections.Generic;
using System.IO;

using PLCommon;

namespace FrontEnd
{
    public static class FileSearch
    {
        static List<string> searchPath = new List<string> () 
        {
            @"C:\Users\rgsod\Documents\Visual Studio 2019\Projects\PlotLab\Examples",
        };

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
        // Check path to see if file exists. if so, return its "path + name + ext"
        //
        public static bool NameSearch (string name, ref string fullName)
        {
            foreach (string d in searchPath)
            {
                string full = d + "\\" + name + ".m";

                if (File.Exists (full))
                {
                    fullName = full;
                    return true;
                }
            }
           
            return false;
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
            SymbolicNameTypes fileType = SymbolicNameTypes.Unknown;

            foreach (string d in searchPath)
            {
                string fullName = d + "\\" + name + ".m";

                if (File.Exists (fullName))
                {
                    // find first word outside of comments
                    string line;

                    StreamReader file = new StreamReader (fullName);

                    while ((line = file.ReadLine ()) != null)
                    {
                        if (line.Length > 0)
                        {
                            string [] words = line.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (words.Length > 0)
                            {
                                if (words [0].Contains ("%") == false)
                                {
                                    if (words [0] == "function")
                                        fileType = SymbolicNameTypes.FunctionFile;
                                    else
                                        fileType = SymbolicNameTypes.ScriptFile;

                                    break;
                                }
                            }
                        }
                    }

                    file.Close ();
                }
            }

            return fileType;
        }
    }
}
