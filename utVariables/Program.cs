using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

    public delegate PLVariable PLFunction    (PLVariable var);

namespace utVariables
{
    class Program
    {
        static void Main (string [] args)
        {
            try
            {
                PLString str1 = new PLString ("ABC");
                Console.WriteLine (str1);

                str1.Add ("DEF");
                Console.WriteLine (str1);

                PLString str2 = str1.Add (new PLString ("GHI"));

                Console.WriteLine (str1);
                Console.WriteLine (str2);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }
    }
}
