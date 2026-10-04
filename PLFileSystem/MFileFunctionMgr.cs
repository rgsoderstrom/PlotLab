using System;
using System.Collections.Generic;
using System.IO;

namespace PLFileSystem
{
    static public class MFileFunctionMgr
    {

     ////   static Dictionary<string, MFileFunctionProcessor> MFileCache = new Dictionary<string, MFileFunctionProcessor> ();

     //   public static MFileFunctionProcessor ParseMFile (string funcName, string fullName)
     //   {
     //       MFileFunctionProcessor proc = new MFileFunctionProcessor (funcName, fullName);
     ////     MFileCache.Add (funcName, proc);
     //       return proc;
     //   }

     //   public static void ClearCache ()
     //   {
     // //      MFileCache.Clear ();
     //   }

        //*****************************************************************************
        //
        // IsMFileFunction - opens passed-in file and checks for correct function syntax
        //                     - first non-blank and non-comment line must start with "function"
        //
        public static bool IsMFileFunction (string fullName)
        {
            bool isFunction = false;

            StreamReader file = new StreamReader (fullName);
            string raw;

            while ((raw = file.ReadLine ()) != null)
            {
                if (raw.Length > 0)
                {
                    string [] tokens = raw.Split (new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length > 0)
                    {
                        if (tokens [0] [0] == '%')
                            continue;

                        if (tokens [0] == "function")
                            isFunction = true;

                        else
                            isFunction = false;
                    }

                    break;
                }
            }

            file.Close ();
            return isFunction;
        }
    }
}




