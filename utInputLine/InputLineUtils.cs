
/*
    InputLineUtils.cs - 
*/

using System;
using System.Collections.Generic;

namespace PLMain
{
    public partial class InputLineProcessor
    {
        //**************************************************************************************

        // keywords used to identify a statement's type


        

        static private readonly List<string> Keywords = new List<string> () 
                                                    {"function", "global",
                                                     "return", "switch", "case"};

        public static bool IsKeyword (string str)
        {
            return Keywords.Contains (str);
        }



        //**************************************************************************************

    }
}
