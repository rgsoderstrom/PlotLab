using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace FrontEnd
{
    public static class CommandLineHistory
    {
     //   readonly string prompt;
        static List<string> history = new List<string> ();
        static public List<string> History {get {return history;}}

        static int NextFwd = 1;
        static int NextBack = -1;
        static int Newest {get {return history.Count - 1;}}

        static string historyFileName = "CommandHistory.txt";

        public static void Open ()
        {
            try
            {
                StreamReader file = new StreamReader (UserConsole.HistoryFileDirectory + "\\" + historyFileName);
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
        private static readonly CommandHistoryWriteOptions writeOption = CommandHistoryWriteOptions.WriteLatestUnique;
        private static readonly int maxLineCount = 100; // don't write more than this many lines

        static public void Close ()
        {
            try
            {
                StreamWriter file = new StreamWriter (UserConsole.HistoryFileDirectory + "\\" + historyFileName);
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
                    file.WriteLine (str);

                file.Close ();
            }

            catch (Exception)
            {
                EventLog.WriteLine ("Error writing command history to file " + historyFileName);
            }
        }

        static public void Clear ()
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

            if (history.Count == 0) AddFlag = true;
            else if (string.Compare (history [history.Count - 1], str) != 0) AddFlag = true; // don't add same string twice in a row

            if (AddFlag)
            {
                history.Add (str);
                NextBack = Newest;
                NextFwd = NextBack + 2;
            }
        }

        //*****************************************************************

        static bool IsValid (int index)
        {
            return (index >= 0) && (index < history.Count);
        }

        //*****************************************************************

        public static bool StepBack (ref string cmnd, string lookFor)
        {
            try
            { 
                while (IsValid (NextBack))
                {
                    cmnd = history [NextBack];
                    NextBack--;
                    NextFwd--;

                    if (lookFor == null || lookFor.Length == 0)
                        return true;

                    if (cmnd.Length >= lookFor.Length)
                        if (lookFor == cmnd.Substring (0, lookFor.Length))
                            return true;
                }
            }

            catch (Exception)
            {
                throw new Exception ("StepBack exception");
            }

            return false;
        }

        //*****************************************************************

        public static bool StepForward (ref string cmnd, string lookFor)
        {
            try
            {
                while (IsValid (NextFwd))
                {
                    cmnd = history [NextFwd];
                    NextBack++;
                    NextFwd++;

                    if (lookFor == null || lookFor.Length == 0)
                        return true;

                    if (cmnd.Length >= lookFor.Length)
                        if (lookFor == cmnd.Substring (0, lookFor.Length))
                            return true;
                }

                NextBack = Newest;
                NextFwd = NextBack + 2;
            }

            catch (Exception)
            {
                throw new Exception ("StepForward exception");
            }

            return false;
        }
    }
}
