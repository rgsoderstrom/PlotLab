using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLMain;

namespace utCommandLineHistory
{
    internal partial class Program
    {
        // commands to CommandHistory
        internal enum Command {Reset, Add, Clear, StepBackward, SearchBackward, StepFwd, SearchFwd};

        internal readonly struct TestStep
        {
            internal TestStep (Command c) : this (c, "")
            {
            }

            internal TestStep (Command c, string a)
            {
                cmnd = c;
                arg = a;
            }

            internal readonly Command cmnd;
            internal readonly string   arg;
        }

        //************************************************************

        static void LoadInitialHistory ()
        {
            CommandLineHistory.Add ("A Line 1");
            CommandLineHistory.Add ("A Line 2");
            CommandLineHistory.Add ("B Line 3");
            CommandLineHistory.Add ("B Line 3");
            CommandLineHistory.Add ("B Line 4");
            CommandLineHistory.Add ("C Line 5");
            CommandLineHistory.Add ("C Line 6");
        }

        //************************************************************

        static internal readonly List<TestStep> TestSequence1 = new List<TestStep> ()
        {
            new TestStep (Command.SearchBackward, "C L"),
            new TestStep (Command.StepBackward),
            new TestStep (Command.StepBackward),
            new TestStep (Command.StepFwd),
            new TestStep (Command.StepFwd),
            new TestStep (Command.StepFwd),
            new TestStep (Command.StepFwd),
        };


    }
}
