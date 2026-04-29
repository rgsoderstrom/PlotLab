
/*
    CommandLineHistory
        - this version does not support "!" for command recall
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Common;
using PLFileSystem;

namespace PLMain
{
    public static class CommandLineHistory
    {
        private static readonly List<string> History = new List<string> ();

        private static int NextFwd = 1;
        private static int NextBack = -1;
        private static int Newest {get {return History.Count - 1;}}

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
                        History.Add (raw);
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

        enum FileWriteOptions {WriteAll, WriteUnique, WriteLatestUnique};
        private static readonly FileWriteOptions writeOption = FileWriteOptions.WriteUnique;
        private static readonly int maxLineCount = 100; // don't write more than this many lines

        public static void Close (bool editOnClose)
        {
            try
            {
                StreamWriter file = new StreamWriter (FileSystem.CommandHistoryFileDir + "\\" + historyFileName);
                List<string> writeList;

                if (writeOption == FileWriteOptions.WriteAll)
                {
                    writeList = History.ToList ();
                }

                else if (writeOption == FileWriteOptions.WriteUnique)
                {
                    writeList = History.Distinct ().ToList ();
                }

                else // write latest unique
                { 
                    List<string> hist = History.ToList ();
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
            History.Clear ();
            NextBack = Newest;
            NextFwd = NextBack + 2;
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

            if (History.Count == 0) 
                AddFlag = true;

            else if (string.Compare (History [History.Count - 1], str) != 0) 
                AddFlag = true; // don't add same string twice in a row

            if (AddFlag)
            {
                History.Add (str);
                NextBack = Newest;
                NextFwd = NextBack + 2;
            }
        }

        //*****************************************************************

        private static bool IsValid (int index)
        {
            return (index >= 0) && (index < History.Count);
        }

        //*****************************************************************
        //*****************************************************************
        //*****************************************************************

        public static bool StepBackward (out string nextBackCmnd)
        {
            bool valid = IsValid (NextBack);

            if (valid)
            {
                nextBackCmnd = History [NextBack];
                NextBack--;
                NextFwd--;
            }
            else
                nextBackCmnd = "";

            return valid;
        }

        //*****************************************************************

        public static bool SearchBackward (out string nextBackCmnd, string lookFor)
        {
            try
            { 
                while (StepBackward (out nextBackCmnd))
                {
                    if (nextBackCmnd.Length >= lookFor.Length)
                        if (lookFor == nextBackCmnd.Substring (0, lookFor.Length))
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
        //*****************************************************************
        //*****************************************************************

        public static bool StepForward (out string nextFwdCmnd)
        {
            bool valid = IsValid (NextFwd);

            if (valid)
            {
                nextFwdCmnd = History [NextFwd];
                NextBack++;
                NextFwd++;
            }
            else
                nextFwdCmnd = "";

            return valid;
        }

        //*****************************************************************

        public static bool SearchForward (out string nextFwdCmnd, string lookFor)
        {
            try
            {
                while (StepForward (out nextFwdCmnd))
                {
                    if (nextFwdCmnd.Length >= lookFor.Length)
                        if (lookFor == nextFwdCmnd.Substring (0, lookFor.Length))
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

        //*****************************************************************

        public static new string ToString ()
        {
            string str = "";

            for (int i=0; i<History.Count; i++)
                str += i.ToString () + ":" + History [i] + "\n";

            return str;
        }
    }
}
