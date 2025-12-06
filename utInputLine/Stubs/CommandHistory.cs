using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal class CommandHistory : ICmndHistory
    {
        static List<string> history = new List<string> () 
        {
            "a = b + c" , 
            "plot (a)", 
            "d = [1:10:100]",
            "LocalOsc = cos (2 * pi * f * t)",
        };

        int get = history.Count - 1;

        //**************************************************************************

        // !43 or !12:p
        
        public string RecallBang (string bangCmnd, ref bool printOnly, ref bool error)  
        {
            printOnly = false; // both may be revised below
            error = false;

            try
            { 
                string [] tokens = bangCmnd.Split (new char [] {'!', ':'}, StringSplitOptions.RemoveEmptyEntries);
                int index = Convert.ToInt16 (tokens [0]);

                if (tokens.Length > 1)
                {
                    if (tokens [1] [0] == 'p')
                    {
                        printOnly = true;
                    }
                    else
                    {
                        error = true;
                        return "Only history option supported is \'p\', for Print, e.g.: !12:p";
                    }
                }

                return history [index - 1]; // 1-based index
            }

            catch (Exception )
            {
                error = true;
            }

            return "Error";
        }

        //**************************************************************************

        // user types first few characters followed by up-arror

        public string RecallText (string first)
        {
            return "Not implemented";
        }

        public string RecallPrevious () // up arrow
        {
            return "Not implemented";
        }

        public string RecallNext ()  // down arror
        {
            return "Not implemented";
        }


    }
}
