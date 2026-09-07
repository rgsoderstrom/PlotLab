
/*
    AnnotatedStringSet - queue of AnnotatedStrings
*/

using System;
using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedStringSet
    {
        // queue of complete AnnotatedStrings,
        private Queue<AnnotatedString> annotatedStrings = new Queue<AnnotatedString> ();

        // number of complete string ready for processing
        public int  Count   {get { return annotatedStrings.Count;}}
        public bool IsEmpty {get {return Count == 0;}}

        public AnnotatedString GetOldest () {return annotatedStrings.Dequeue ();}
        public void Clear () {annotatedStrings.Clear ();}

        //**************************************************************************

        public AnnotatedStringSet (AnnotatedString astr)
        {
            Add (astr);
        }

        public AnnotatedStringSet ()
        {
        }

        //*************************************************************************

        public void Add (AnnotatedString astr)
        {
            if (astr.IsCompound == false)
            {
                annotatedStrings.Enqueue (astr);
                return;
            }

            // If we get here astr is a compound expression, e.g.a = 123; b = 456; c = 789; all on one line.
            // It will be split into a list of annotated strings

            int startIndex = 0;
            List<int> boundries = astr.Level0Semis; // these are indices of the breaks between expressions
            boundries.AddRange (astr.Level0Commas);

            boundries.Sort ();


            string str = astr.Plain;

            // if input astr.SupressPrinting is true, restore trailing semicolon. this ensures
            // last string of the set will will also be marked SupressPrinting
            //if (astr.SupressPrinting)
            //    str += ';';

            for (int i = 0; i<boundries.Count; i++)
            {
                int endIndex = boundries [i] - 1; // stop copying after this character

                string partial = str.Substring (startIndex, endIndex - startIndex + 1);
                string trimmed = partial.Trim (new char [] {' '});

                AnnotatedString aTrimmed = new AnnotatedString (trimmed);
                aTrimmed.SupressPrinting = true;
                annotatedStrings.Enqueue (aTrimmed);
                startIndex = endIndex + 2;
            }

            // one more outside of loop if input string does not end in semicolon
            if (startIndex < str.Length)// - 1)
            {
                string partial = str.Substring (startIndex, str.Length - startIndex);
                string trimmed = partial.Trim (new char [] {' '});

                AnnotatedString aTrimmed = new AnnotatedString (trimmed);
                aTrimmed.SupressPrinting = astr.SupressPrinting;
                annotatedStrings.Enqueue (aTrimmed);
            }
        }
    }
}
