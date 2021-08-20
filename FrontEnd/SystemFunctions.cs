
using System;
using System.Collections.Generic;
using System.Windows;

using PLCommon;

namespace FrontEnd
{
    static public partial class SystemFunctions
    {
        public static Dictionary<string, PLFunction> SystemCommands = new Dictionary<string, PLFunction> ();

        static SystemFunctions ()
        {
            Dictionary<string, PLFunction> infCmnds = GetContents ();
            foreach (string str in infCmnds.Keys) SystemCommands.Add (str, infCmnds [str]);
        }

        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static public Dictionary<string, PLFunction> GetContents ()
        {
            return new Dictionary<string, PLFunction> 
            {
                {"exit", Exit},
                {"clc",  Clc },
                {"path", Path},
                {"addpath", AddPath},
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

        public static PLVariable Clc (PLVariable _)
        {
            UserConsole.thisConsole.TextPane.Clear (); 
            //UserConsole.ClearInputLine ();
            return new PLNull ();
        }

        //*********************************************************************************************

        public static PLVariable AddPath (PLVariable arg)
        {
            PLList lst = arg as PLList;

            if (lst != null)
            {
                PLString pstr = lst [0] as PLString;

                if (pstr != null)
                {
                    string path = pstr.Data;

                    if (path [0] == '(') path = path.Substring (1);
                    if (path [path.Length - 1] == ')') path = path.Substring (0, path.Length - 1);
 
                    if (path [0] == '\'') path = path.Substring (1);
                    if (path [path.Length - 1] == '\'') path = path.Substring (0, path.Length - 1);

                    FileSearch.AddPath (path);
                }
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
    }
}
