using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Common;
using PLCommon;
using PLFileSystem;
using PLMain;

namespace utCommandLineHistory
{
    internal partial class Program
    {
        static void Main (string [] args)
        { 
            string fromHistory;

            FileSystem.Open (Print);
            EventLog.Open (@"..\..\log.txt");

            CommandLineHistory.Open ();
            //LoadInitialHistory ();

            PrintLine (CommandLineHistory.ToString ());

            foreach (TestStep thisStep in TestSequence1)
            {
                fromHistory = null;

                switch (thisStep.cmnd)
                {
                    case Command.Reset:
                        CommandLineHistory.ResetIndices ();
                        break; 
                        
                    case Command.Add:
                        CommandLineHistory.Add (thisStep.arg);
                        break; 
                        
                    case Command.Clear:
                        CommandLineHistory.Clear ();
                        break; 
                        
                    case Command.StepBackward:
                        Print ("StepBackward:   ");
                        CommandLineHistory.StepBackward (out fromHistory);
                        break; 
                        
                    case Command.SearchBackward:
                        Print ("SearchBackward: ");
                        CommandLineHistory.SearchBackward (out fromHistory, thisStep.arg);
                        break; 
                                                
                    case Command.StepFwd:
                        Print ("StepForward:    ");
                        CommandLineHistory.StepForward (out fromHistory);
                        break; 
                        
                    case Command.SearchFwd:
                        Print ("SearchForward:  ");
                        CommandLineHistory.SearchForward (out fromHistory, thisStep.arg);
                        break; 
                        
                    default:
                        PrintLine ("Unexpected command: " + thisStep.cmnd);
                        break;
                }

                if (fromHistory != null)
                    PrintLine (fromHistory);
            }

            CommandLineHistory.Close (false);
        }

        static void PrintLine (string str)
        {
            Console.WriteLine (str);
        }

        static void Print (string str)
        {
            Console.Write (str);
        }

    }
}
