
/*
    NestedStringSet - queue of NestedStrings
*/

using System.Collections.Generic;

namespace PLMain
{
    public class NestedStringSet
    {
        // queue of complete NestedStrings,
        private Queue<NestedString> nestedStrings = new Queue<NestedString> ();

        // number of complete string ready for processing
        public int Count {get {return nestedStrings.Count;}}
        public bool IsEmpty {get {return Count == 0;}}


     // public NestedString GetOldest {get {return nestedStrings.Dequeue ();}}
        public NestedString GetOldest () {return nestedStrings.Dequeue ();}

        public NestedString PeekOldest {get {return nestedStrings.Peek ();}}
        public void Pop () {nestedStrings.Dequeue ();}

        public void Clear () {nestedStrings.Clear ();}

        //**************************************************************************

        public NestedStringSet ()
        {
        }


        //*************************************************************************

        public void Add (NestedString astr)
        {
            if (astr.IsCompound == false)
            {
                nestedStrings.Enqueue (astr);
                return;
            }

            // If we get here astr is a compound expression, e.g. a = 123; b = 456; c = 789; all on one line.
            // It will be split into a list of annotated strings

            int startIndex = 0;
            List<int> indices = astr.Level0Semis; // these are indices of the breaks between expressions

            string str = astr.Plain;

            for (int i = 0; i<indices.Count; i++)
            {
                int endIndex = indices [i]; // stop copying after this character

                string partial = str.Substring (startIndex, endIndex - startIndex + 1);
                string trimmed = partial.Trim (new char [] { ' ' });

                nestedStrings.Enqueue (new NestedString (trimmed));
                startIndex = endIndex + 1;
            }

            // one more outside of loop if input string does not end in semicolon
            if (startIndex < str.Length - 1)
            {
                string partial = str.Substring (startIndex, str.Length - startIndex);
                string trimmed = partial.Trim (new char [] { ' ' });

                nestedStrings.Enqueue (new NestedString (trimmed));
            }
        }
    }
}
