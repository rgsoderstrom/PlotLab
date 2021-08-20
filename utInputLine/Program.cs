using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FrontEnd;

namespace utInputLine
{
    class Program
    {
        static List<string> testCases = new List<string> () 
        { 
            "x = 3;", 
            "a = [1, 2, 3]", 
        //    "h = sprintf ('%d\n', aa); \r\n disp (h)",
            "a = 11;\r\nb = 22;\r\nc = 33;"
        };

        static void Main (string [] args)
        {
            string [] words = testCases [2].Split (new string [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);



        }
    }
}
