
using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

using PLCommon;

namespace FrontEnd
{
    static public partial class SystemFunctions
    {
        public static Dictionary<string, PLFunction> SystemCommands = new Dictionary<string, PLFunction> ();

        static SystemFunctions ()
        {
            Dictionary<string, PLFunction> infCmnds = GetContents ();

            foreach (string str in infCmnds.Keys)
                SystemCommands.Add (str, infCmnds [str]);
        }

        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static public Dictionary<string, PLFunction> GetContents ()
        {
            return new Dictionary<string, PLFunction> 
            {
                {"cd",    Cd  },
                {"ls",    Ls  },
                {"pwd",   Pwd },
                {"exit",  Exit},
                {"edit",  Edit},
                {"clc",   Clc },
                {"path",  Path},
                {"addpath", AddPath},
                {"help",    HelpWindow},
            };
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************

        public static SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = SymbolicNameTypes.Unknown;

            if (SystemCommands.ContainsKey (str)) {type = SymbolicNameTypes.SystemCommand;}

            return type;
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************

        public static PLVariable RunSystemCommand (PLString name, PLVariable args)
        {
            if (SystemCommands.ContainsKey (name.Text))
            {
                PLFunction func = SystemCommands [name.Text];
                return func (args);
            }
            else
                throw new Exception ("Command " + name + " not found");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        public static PLVariable Exit (PLVariable _)
        {
            Application.Current.Shutdown();           
            return new PLNull ();
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Open a file in its default editor
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        
        public static PLVariable Edit (PLVariable filename)
        {
            PLString pstr = filename as PLString;
            string str = pstr.Data;

            if (str.Contains (".m")) // NameSearch function assumes no extension
                str = str.Replace (".m", "");

            string fullName = "";

            bool found = FileSearch.NameSearch (str, ref fullName);

            if (found)
                System.Diagnostics.Process.Start (fullName);
            else
                throw new Exception ("File " + filename + " not found");

            return new PLNull ();
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Change Directory. Invoked to handle "cd" command
        /// </summary>
        /// <param name="arg">Relative or absolute path</param>
        /// <returns></returns>

        public static PLVariable Cd (PLVariable arg)
        {
            if (arg is PLString pstr)
            {
                string nextCurrentDir;
                string path = pstr.Data;

                if (path [0] == '\\') // absolute path on same disk
                {
                    int i = FileSearch.CurrentDirectory.IndexOf ("\\");

                    if (i == -1)
                        throw new Exception ("Error reading current disk");

                    string disk = FileSearch.CurrentDirectory.Substring (0, i);

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
                        nextCurrentDir = FileSearch.CurrentDirectory;

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
                    FileSearch.CurrentDirectory = nextCurrentDir;
                else
                    throw new Exception ("Directory " + nextCurrentDir + " doesn't exist");
            }

            else
                throw new Exception ("cd - argument error");

            return new PLNull ();
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
        
        public static PLVariable Pwd (PLVariable _)
        {
            return new PLString (FileSearch.CurrentDirectory);
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Ls - list directory contents. invoked to handle "ls" command. Prints content of current working directory
        /// </summary>
        /// <param name="arg">Optional pattern to search for</param>
        /// <returns>List of PLStrings</returns>

        public static PLVariable Ls (PLVariable arg)
        {
            PLList fileList = new PLList ();
            string searchPattern = null;

            // see if there is a search patterm
            if (arg is PLString pstr && pstr.Data.Length > 0)
            {
                searchPattern = pstr.Data;
            }

            //*******************************************************************************

            // get list of all subdirectories in current directory
            string [] dirs = searchPattern == null ? System.IO.Directory.GetDirectories (FileSearch.CurrentDirectory)
                                                   : System.IO.Directory.GetDirectories (FileSearch.CurrentDirectory, searchPattern);
            for (int i = 0; i<dirs.Length; i++)
            {
                // strip off all but subdirs name           
                string str = dirs [i];
                int j = str.LastIndexOf ('\\');

                if (j > -1)
                    str = str.Remove (0, j + 1);

                // add what's left to print list, with a trailing backslash appended to indicate this is a directory
                fileList.Add (new PLString (str + "\\"));
            }

            //*******************************************************************************

            // get list of all regular files in current directory
            string [] files = searchPattern == null ? System.IO.Directory.GetFiles (FileSearch.CurrentDirectory)
                                                    : System.IO.Directory.GetFiles (FileSearch.CurrentDirectory, searchPattern);
            for (int i = 0; i<files.Length; i++)
            {
                // strip off all but file name and extension           
                string str = files [i];
                int j = str.LastIndexOf ('\\');

                if (j > -1)
                    str = str.Remove (0, j + 1);

                // add what's left to print list
                fileList.Add (new PLString (str));
            }

            return fileList;
        }

        //*********************************************************************************************

        public static PLVariable Clc (PLVariable _)
        {
            UserConsole.thisConsole.TextPane.Clear (); 
            //UserConsole.ClearInputLine ();
            return new PLNull ();
        }

        //*********************************************************************************************

        public static PLVariable AddPath (PLVariable arg)
        {
            PLString pstr = arg as PLString;

            if (pstr != null)
            {
                string path = pstr.Data;

                if (path [0] == '(') path = path.Substring (1);
                if (path [path.Length - 1] == ')') path = path.Substring (0, path.Length - 1);
 
                if (path [0] == '\'') path = path.Substring (1);
                if (path [path.Length - 1] == '\'') path = path.Substring (0, path.Length - 1);

                FileSearch.AddPath (path);
            }

            return new PLNull ();
        }

        //*********************************************************************************************

        public static PLVariable Path (PLVariable _)
        {
            List<string> pathStrings = FileSearch.GetPathCopy ();
            PLList copy = new PLCommon.PLList ();

            foreach (string str in pathStrings)
            {
                PLString pls = new PLString (str);
                copy.Add (pls);
            }

            return copy;
        }

        //*********************************************************************************************

        public static PLVariable HelpWindow (PLVariable _topic)
        {
            PLVariable status = new PLNull ();

            if ((_topic is PLString) && (_topic as PLString).Text != null && (_topic as PLString).Text.Length > 0)
            {
                string topic = (_topic as PLString).Text;

                if (PLHelpWindow.HelpWindowManager.DisplayHelpTopic (topic) == false)
                    status = new PLString ("Not found");
            }

            else
            {
                PLHelpWindow.HelpWindowManager.LaunchNewHelpWindow ();
            }

            return status;
        }
    }
}
