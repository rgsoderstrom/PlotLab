
/*
    Block.cs
        - a "Block" is a list of statements starting with one of (for, while, if)
          and ending with "end"
*/

using System.Collections.Generic;
using PLCommon;

namespace PLMain
{
    abstract internal class Block
    {
        // a "Block" is a list of statements starting with one of (for, while, if) and ending with "end"

        protected string name = "";
        public string Name {get {return name;} protected set {name = value;}}

        static protected int instanceCounter = 1;

        // All statements have been read in
        private bool complete = false;
        public bool Complete {get {return complete;} protected set {complete = value;}}

        // debug printing
        static protected PrintFunction Print = null;

        static public void SetPrintFunction (PrintFunction pr)
        {
            Print = pr;
        }

        //************************************************************************

        protected Block ()
        {
            Name = "BLOCK_" + instanceCounter++.ToString ();
        }

        internal abstract void Add (AnnotatedString astr);
        internal abstract TerminationReason Run ();
    }
}


