using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    internal class WhileBlock : Block_loop
    {
        //************************************************************************

        internal WhileBlock (AnnotatedString astr)
        {
            BlockStatements = new TestCodePair (astr.Arguments);
        }

        internal override void Close ()
        {
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "WhileBlock " + Name;

            str += "\n  Test: " + BlockStatements.test;

            str += "\n  loop code: ";

            foreach (AnnotatedString astr in BlockStatements.code)
                str += "\n     " + astr.Plain;

            return str;
        }
    }
}
