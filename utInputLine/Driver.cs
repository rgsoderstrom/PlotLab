using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

using static Main.InputLineProcessor;

namespace utInputLine
{
    class Driver
    {
        static List<string> testCases = new List<string> () 
        { 
            //"a = 3; b = 7;",
            "a = (1, 2, 3);  b = [1 2 3 ; 4 5 6];    c = a +    a;     d = sqrt (c)  ",
            //"y = sin (7 + (8 * (9 + 10 + 'z')));", 
            //"x = 3; y = sin (7 + (8 * 9));", 
            //"a = [1, sqrt (2), 3];", 
            //"a = [10:2:30];",
            //"s = 'a b c \\' d e f';",
            //"s = sprintf ('%f', a);",
            //"z [5:7] = \t [1:3];"
        };

        static Workspace workspace   = new Workspace ();
        static Library   library     = new Library ();
        static FileSystem fileSystem = new FileSystem ();

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        static void Main (string [] args)
        {
            InputLineProcessor inputProcessor = new InputLineProcessor (workspace, library, fileSystem, Print);

            foreach (string str in testCases)
            {
                List<string> statements = new List<string> ();
                List<StatementTypes> stmtTypes = new List<StatementTypes> ();

                inputProcessor.ParseOneInputLine (str, ref statements, ref stmtTypes);

            }


        }
    }
}
