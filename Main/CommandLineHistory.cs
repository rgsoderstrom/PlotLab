using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using PLFileSystem;

namespace PLMain
{
    public static class CommandLineHistory
    {
        private static readonly List<string> history = new List<string> ();
        public  static List<string> History {get {return history;}}

        private static int NextFwd = 1;
        private static int NextBack = -1;
        private static int Newest {get {return history.Count - 1;}}

        private static readonly string historyFileName = "CommandHistory.txt";

        public static void Open ()
        {
            try
            {
                StreamReader file = new StreamReader (FileSystem.CommandHistoryFileDir + "\\" + historyFileName);
                string raw;

                while ((raw = file.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        history.Add (raw);
                    }
                }

                file.Close ();

                NextBack = Newest;
                NextFwd = NextBack + 2;
            }

            catch (FileNotFoundException)
            {

            }

            catch (Exception)
            {
                EventLog.WriteLine ("Error reading command history file " + historyFileName);
            }
        }

        //*****************************************************************************************

        enum CommandHistoryWriteOptions {WriteAll, WriteUnique, WriteLatestUnique};
        private static readonly CommandHistoryWriteOptions writeOption = CommandHistoryWriteOptions.WriteUnique;
        private static readonly int maxLineCount = 100; // don't write more than this many lines

        public static void Close (bool editOnClose)
        {
            try
            {
                StreamWriter file = new StreamWriter (FileSystem.CommandHistoryFileDir + "\\" + historyFileName);
                List<string> writeList;

                if (writeOption == CommandHistoryWriteOptions.WriteAll)
                {
                    writeList = history.ToList ();
                }

                else if (writeOption == CommandHistoryWriteOptions.WriteUnique)
                {
                    writeList = history.Distinct ().ToList ();
                }

                else // write latest unique
                { 
                    List<string> hist = history.ToList ();
                    hist.Reverse ();
                    writeList = hist.Distinct ().ToList ();
                    writeList.Reverse ();
                }

                if (writeList.Count > maxLineCount)
                    writeList.RemoveRange (0, writeList.Count - maxLineCount);

                foreach (string str in writeList)
                    if (str [0] != '!')   // don't write history recalls to file
                        file.WriteLine (str);

                file.Close ();

                // if requested edit history file after Plotlab exits
                if (editOnClose)
                {
                    try
                    {
                        System.Diagnostics.Process.Start (FileSystem.CommandHistoryFileDir + "\\" + historyFileName);
                    }

                    catch (Exception ex)
                    {
                        EventLog.WriteLine ("Exception editing history file: " + ex.Message);
                    }
                }
            }

            catch (Exception)
            {
                EventLog.WriteLine ("Error writing command history to file " + historyFileName);
            }
        }

        public static void Clear ()
        {
            history.Clear ();
        }

        public static void ResetIndices ()
        {
            NextBack = Newest;
            NextFwd = Newest + 2;
        }

        //*****************************************************************

        public static void Add (string str)
        {
            bool AddFlag = false;

            if (history.Count == 0) 
                AddFlag = true;

            else if (string.Compare (history [history.Count - 1], str) != 0) 
                AddFlag = true; // don't add same string twice in a row

            if (AddFlag)
            {
                history.Add (str);
                NextBack = Newest;
                NextFwd = NextBack + 2;
            }
        }

        //*****************************************************************

        private static bool IsValid (int index)
        {
            return (index >= 0) && (index < history.Count);
        }

        //*****************************************************************

        public static bool StepBack (ref string cmnd)
        {
            bool valid = IsValid (NextBack);

            if (valid)
            {
                cmnd = history [NextBack];
                NextBack--;
                NextFwd--;
            }

            return valid;
        }

        //*****************************************************************

        public static bool SearchBack (ref string cmnd, string lookFor)
        {
            try
            { 
                while (StepBack (ref cmnd))
                {
                    if (cmnd.Length >= lookFor.Length)
                        if (lookFor == cmnd.Substring (0, lookFor.Length))
                            return true;
                }
            }

            catch (Exception)
            {
                throw new Exception ("SearchBack exception");
            }

            return false;
        }

        //*****************************************************************

        public static bool StepForward (ref string cmnd)
        {
            bool valid = IsValid (NextFwd);

            if (valid)
            {
                cmnd = history [NextFwd];
                NextBack++;
                NextFwd++;
            }

            return valid;
        }

        public static bool SearchForward (ref string cmnd, string lookFor)
        {
            try
            {
                while (StepForward (ref cmnd))
                {
                    if (cmnd.Length >= lookFor.Length)
                        if (lookFor == cmnd.Substring (0, lookFor.Length))
                            return true;
                }

                NextBack = Newest;
                NextFwd = NextBack + 2;
            }

            catch (Exception)
            {
                throw new Exception ("SearchForward exception");
            }

            return false;
        }
    }
}
