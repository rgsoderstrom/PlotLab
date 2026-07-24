using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    internal class WhileBlock : Block
    {
        internal WhileBlock (string str)
        {
            Print ("new \"while\" block");
        }

        internal override void Run ()
        {
            Print ("Running \"while\" block");
        }

        internal override void Add (AnnotatedString astr)
        {

        }
    }
}
