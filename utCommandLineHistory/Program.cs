using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using PLCommon;
using PLFileSystem;

using PLMain;

namespace utCommandLineHistory
{
    internal class Program
    {
        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        static void Main (string [] args)
        { 
            string fromHistory = "";

            FileSystem.Open (Print);
            EventLog.Open (@"..\..\log.txt");

            CommandLineHistory.Open ();

            CommandLineHistory.Add ("Line 1");
            CommandLineHistory.Add ("Line 2");
            CommandLineHistory.Add ("Line 3");
            CommandLineHistory.Add ("Another Line 3");
            CommandLineHistory.Add ("Line 4");
            CommandLineHistory.Add ("Line 5");
            CommandLineHistory.Add ("Line 6");

            //while (CommandLineHistory.StepBack (ref fromHistory))
            //    Print (fromHistory);

            //if (CommandLineHistory.SearchBack (ref fromHistory, "Ano"))
            //    Print ("Found: " + fromHistory);
            //else
            //    Print ("Not found");

            if (CommandLineHistory.SearchBack (ref fromHistory, "Ano"))
            { 
                if (CommandLineHistory.StepForward (ref fromHistory))
                {
                    CommandLineHistory.StepForward (ref fromHistory);
                    Print ("Found: " + fromHistory);
                }
                else
                    Print ("StepFwd not valid");
            }
            else
                Print ("Not found");
        }
    }
}
