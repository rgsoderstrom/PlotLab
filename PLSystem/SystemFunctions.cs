
/*
    PLSystem - created as an Empty Project (.Net Framework)
             - framework changed to .Net 4.8
             - output type changed to Class Library
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

using PLCommon;
using PLLibrary;
using PLFileSystem;

namespace PLSystem
{
    static public partial class SystemFunctions
    {
        static public PrintFunction Print = null;
        static public BSFunction UserConsoleRequests;

        static public Dictionary<string, BSFunction> SystemCommands = new Dictionary<string, BSFunction> ();

        static SystemFunctions ()
        {
            AddCommands (ref SystemCommands);
        }

        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static private void AddCommands (ref Dictionary<string, BSFunction> dst)
        {
            dst.Add ("cd",    Cd);
            dst.Add ("ls",    Ls);
            dst.Add ("pwd",   Pwd);
            dst.Add ("exit",  Exit);
            dst.Add ("edit",  Edit);
            dst.Add ("clc",   Clc);
            dst.Add ("path",  Path);
            dst.Add ("addpath", AddPath);
            dst.Add ("history", History);
            dst.Add ("help",    HelpWindow);
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************

        public static SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = SymbolicNameTypes.Unknown;

            if (SystemCommands.ContainsKey (str)) 
                type = SymbolicNameTypes.SystemCommand;

            return type;
        }

        public static List<string> PartialMatch (string str)
        {
            List<string> matches = new List<string> ();

            foreach (string cmd in SystemCommands.Keys)
            {                
                if (cmd.StartsWith (str))
                    matches.Add (cmd + " ");
            }

            //if (matches.Count > 0) matches.Add ("\n");
            return matches;
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************

        public static bool RunSystemCommand (string name, string args)
        {
            if (SystemCommands.ContainsKey (name))
            {
                BSFunction func = SystemCommands [name];
                return func (args);
            }
            else
                throw new Exception ("Command " + name + " not found");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        public static bool History (string _)
        {
            UserConsoleRequests?.Invoke ("history");
            return true;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        public static bool Exit (string _)
        {
            UserConsoleRequests?.Invoke ("shutdown");
            return true;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Open a file in its default editor
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>

        public static bool Edit (string filename)
        {
            if (filename.Contains (".m")) // NameSearch function assumes no extension
                filename = filename.Replace (".m", "");

            string fullName = "";

            bool found = FileSystem.NameSearch (filename, ref fullName);

            if (found)
                System.Diagnostics.Process.Start (fullName);
            else
                throw new Exception ("File " + filename + " not found");

            return true;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Change Directory. Invoked to handle "cd" command
        /// </summary>
        /// <param name="path">Relative or absolute path</param>
        /// <returns></returns>

        public static bool Cd (string path)
        {
            string nextCurrentDir;

            // look for leading and trailing single quote. silently eliminate if found
            int i0 = path.IndexOf ('\'');
            int i1 = path.LastIndexOf ('\'');

            if (i0 == 0 && i1 == path.Length - 1)
            {
                path = path.Remove (path.Length - 1, 1);
                path = path.Remove (0, 1);
            }

            if (path [0] == '\\') // absolute path on same disk
            {
                int i = FileSystem.CurrentDirectory.IndexOf ("\\");

                if (i == -1)
                    throw new Exception ("Error reading current disk");

                string disk = FileSystem.CurrentDirectory.Substring (0, i);

                nextCurrentDir = disk + path;
            }

            else
            {
                string [] tokens = path.Split (new string [] {"\\" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i=0; i<tokens.Length; i++)
                    tokens [i] = RemoveQuotes (tokens [i]); // remove leading or trailing single quotes


                if (tokens [0].EndsWith (":")) // absolute path with disk specified
                {
                    nextCurrentDir = path;
                }

                else // relative path
                {
                    nextCurrentDir = FileSystem.CurrentDirectory;

                    foreach (string tok in tokens)
                    {
                        switch (tok)
                        {
                            case ".":
                                break;

                            case "..":
                                nextCurrentDir = RemoveLastFolder (nextCurrentDir);
                                break;

                            default:
                                nextCurrentDir += "\\" + tok;
                                break;
                        }
                    }
                }
            }

            if (Directory.Exists (nextCurrentDir))
            {
                FileSystem.CurrentDirectory = nextCurrentDir;
             //   MFileFunctionMgr.CurrentDir = nextCurrentDir;
            }
            else
                throw new Exception ("Directory " + nextCurrentDir + " doesn't exist");

            return true;
        }

        /// <summary>
        /// Accepts a path string and returns a string  with last folder removed
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        
        static private string RemoveLastFolder (string path)
        {
            int i = path.LastIndexOf ('\\');

            if (i == -1)
                return path;

            return path.Substring (0, i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        
        static private string RemoveQuotes (string str)
        {
            if (str [str.Length - 1] == '\'')
                str = str.Substring (0, str.Length - 1);

            if (str [0] == '\'')
                str = str.Substring (1);

            return str;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Pwd - print working directory. Handles "pwd" command.
        /// </summary>
        /// <param name="_">None</param>
        /// <returns>PLString containing cwd</returns>
        
        public static bool Pwd (string _)
        {
            Print (FileSystem.CurrentDirectory);
            return true;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Ls - list directory contents. invoked to handle "ls" command. Prints content of current working directory
        /// </summary>
        /// <param name="arg">Optional pattern to search for</param>
        /// <returns>List of PLStrings</returns>

        public static bool Ls (string arg)
        {
            List<string> fileList = new List<string> ();
            string searchPattern = null;

            // see if there is a search pattern
            if (arg .Length > 0)
                searchPattern = arg;

            //*******************************************************************************

            // get list of all subdirectories in current directory
            string [] dirs = searchPattern == null ? System.IO.Directory.GetDirectories (FileSystem.CurrentDirectory)
                                                   : System.IO.Directory.GetDirectories (FileSystem.CurrentDirectory, searchPattern);
            for (int i = 0; i<dirs.Length; i++)
            {
                // strip off all but subdirs name           
                string str = dirs [i];
                int j = str.LastIndexOf ('\\');

                if (j > -1)
                    str = str.Remove (0, j + 1);

                // add what's left to print list, with a trailing backslash appended to indicate this is a directory
                fileList.Add (str + "\\");
            }

            //*******************************************************************************

            // get list of all regular files in current directory
            string [] files = searchPattern == null ? System.IO.Directory.GetFiles (FileSystem.CurrentDirectory)
                                                    : System.IO.Directory.GetFiles (FileSystem.CurrentDirectory, searchPattern);
            for (int i = 0; i<files.Length; i++)
            {
                // strip off all but file name and extension           
                string str = files [i];
                int j = str.LastIndexOf ('\\');

                if (j > -1)
                    str = str.Remove (0, j + 1);

                // add what's left to print list
                fileList.Add (str);
            }

            //return fileList;

            foreach (string str in fileList)
                Print (str + "\n");

            return true;
        }

        //*********************************************************************************************

        public static bool Clc (string _)
        {
            UserConsoleRequests?.Invoke ("ClearConsole");
            return true;
        }

        //*********************************************************************************************

        public static bool AddPath (string pathEntry)
        {
            if (pathEntry != null)
            {
                // remove any leading open paren
                if (pathEntry [0] == '(') pathEntry = pathEntry.Substring (1);

                // remove any leading backslash
                if (pathEntry [0] == '\'') pathEntry = pathEntry.Substring (1);

                // remove any trailing semicolon
                int last = pathEntry.Length - 1;

                if (pathEntry [last] == ';') pathEntry = pathEntry.Substring (0, pathEntry.Length - 1);

                // remove any closing paren
                last = pathEntry.Length - 1;

                if (pathEntry [last] == ')') pathEntry = pathEntry.Substring (0, last);
 
                // remove any trailing backslash
                last = pathEntry.Length - 1;

                if (pathEntry [last] == '\'') pathEntry = pathEntry.Substring (0, last);

                FileSystem.AddPath (pathEntry);
            //  MFileFunctionMgr.SearchPathCopy = FileSystem.GetPathCopy ();
            }

            return true;
        }

        //*********************************************************************************************

        // invoked when user requests path

        public static bool Path (string _)
        {
            List<string> pathStrings = FileSystem.GetPathCopy ();

            foreach (string str in pathStrings)
                Print (str + "\n");

            return true;
        }

        //*********************************************************************************************

        public static bool HelpWindow (string topic)
        {
            if (topic != null)
            {
                if (PLHelpWindow.HelpWindowManager.DisplayHelpTopic (topic) == false)
                    Print ("Not found");
            }

            else
            {
                PLHelpWindow.HelpWindowManager.LaunchNewHelpWindow ();
            }

            return true;
        }
    }
}

