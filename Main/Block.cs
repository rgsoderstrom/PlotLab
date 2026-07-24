
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

        // Ready to execute when all statements have been read in
        //private bool ready = false;
        //public bool Ready {get {return ready;} protected set {ready = value;}}

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
        //{
        //    //Print?.Invoke ("adding statement to block: " + astr.Plain);
        //    //BodyStatements.Add (astr);
        //}

        //internal void Add (List<string> lst)
        //{
        //    foreach (string str in lst)
        //        Add (str);
        //}

        //internal void Add (string [] arr)
        //{
        //    foreach (string str in arr)
        //        Add (str);
        //}

        //internal virtual void Close ()
        //{
        //    Print?.Invoke ("Closing block");
        //    //Ready = true;
        //}

        internal abstract void Run ();
        //{
        //}


        //************************************************************************
        //************************************************************************
        //************************************************************************

    }
}


