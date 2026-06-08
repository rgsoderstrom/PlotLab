
/*
    NestedChar - an object containing one character and the depth it is
                 nested by parens, square brackets and quotes
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    public class NestedChar
    {



        //**************************************************************************
        //
        // use this ctor for single char or the first character of a string
        //
        public NestedChar (char c)
        {
        }

        //**************************************************************************
        //
        // use this ctor for 2nd and following characters of a string
        //
        public NestedChar (NestedChar prev, char ch)
        {
        }
    }
}
