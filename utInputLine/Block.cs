
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
        protected List<string> blockStatements = new List<string> ();

        // Complete when "end" encountered
        private bool complete = false;
        internal bool Complete {get {return complete;} set {complete = value;}}

        // debug printing
        static public PrintFunction Print = null;

        //************************************************************************

        protected Block ()
        {
            Complete = false;
        }

        internal void Add (string str)
        {
            Print ("adding statement to block: " + str);
            blockStatements.Add (str);
        }

        internal void Add (List<string> lst)
        {
            foreach (string str in lst)
                Add (str);
        }

        internal void Add (string [] arr)
        {
            foreach (string str in arr)
                Add (str);
        }

        internal virtual void Close ()
        {
            Print ("Closing block");
            Complete = true;
        }

        internal virtual void Run ()
        {
            Print ("Running block");
        }
    }

    //************************************************************************
    //************************************************************************
    //************************************************************************

    internal class ForBlock : Block
    {
        // the text following the word "for"
        internal string loopControl = "";

        internal ForBlock (string str)
        {
            Print ("new \"for\" block");

            // Extract loop control
            //
            //    Look for first comma.
            //       for a = 1:9, b = a ^ 2;
            //
            //    if none found, use everything after "for"
            //

            int startIndex = "for".Length; // start looking here
            int i;

            for (i=startIndex ; i<str.Length; i++)
            {
                if (str [i] == ',')
                    break;
            }

            loopControl = str.Substring (startIndex, i - startIndex).Trim ();
            Print ("loop control: " + loopControl);

            // anything after iterator string goes into blockStatements
            //if (i < str.Length)
            //{
            //    string remaining = str.Substring (i+1);
            //    AnnotatedString astr = new AnnotatedString (remaining);

            //    AnnotatedStringSet aset = new AnnotatedStringSet ();
            //    aset.Add (astr);

            //    while (aset.Count > 0)
            //        Add (aset.GetOldest.Plain);


            //}
        }

        //************************************************************************

        internal override void Run ()
        {
            Print ("Running for " + loopControl + " block");

            InputLineProcessor ip = new InputLineProcessor (Print);

            foreach (string str in blockStatements)
            {
                Print ("  " + str);
                ip.ProcessString (str);
            }
        }
    }

    //************************************************************************
    //************************************************************************
    //************************************************************************

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

    }

    //************************************************************************
    //************************************************************************
    //************************************************************************

    internal class IfBlock : Block
    {
        private static readonly List<string> IfBlockKeywords = new List<string> () {"elseif", "else"};


        internal IfBlock (string str)
        {
            Print ("new \"if\" block");
        }

        internal override void Run ()
        {
            Print ("Running \"if\" block");
        }

    }
}


