using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    internal class ForBlock : WhileBlock
    {
        // Typical supported syntax:
        //    for a = 12 : 15,
        //      <loop code>
        //    end

        // initialization list, runs before first iteration
        //    cases = 12 : 15;
        //    count = 4;
        //    get = 1;

        // remainder is same as "while" loop

        // test string
        //    get <= count;

        // code list
        //    a = cases (get);    
        //    <loop code>
        //    get = get + 1;

                                                                         /* will fail for nested for-loops */
        internal ForBlock (AnnotatedString astr) : base (new AnnotatedString ("get <= count"))
        {
        }

        internal override void Add (AnnotatedString astr)
        {

        }

        internal override TerminationReason Run ()
        {
            return TerminationReason.Completed;
        }
    }
}
