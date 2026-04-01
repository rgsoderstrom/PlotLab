using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLFileSystem;

namespace utFileSystem
{
    internal class Program
    {
        static void Main (string [] args)
        {
            try
            { 
                FileSystem.Open (PrintFunction);

                Console.WriteLine ("Base: " + FileSystem.BaseDirectory);
                Console.WriteLine ("Scripts: " + FileSystem.ScriptDirectory);
                Console.WriteLine ("Current: " + FileSystem.CurrentDirectory);

                FileSystem.CurrentDirectory = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab";
                Console.WriteLine ("Current: " + FileSystem.CurrentDirectory);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        static void PrintFunction (string str)
        {
            Console.WriteLine ("PrintFunction: " + str);
        }
    }
}
