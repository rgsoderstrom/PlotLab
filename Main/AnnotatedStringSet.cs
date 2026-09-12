
/*
    AnnotatedStringSet - list of AnnotatedStrings
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedStringSet : IEnumerable<AnnotatedString>
    {
        //********************************************************************

        // list of complete AnnotatedStrings,
        private readonly List<AnnotatedString> annotatedStrings = new List<AnnotatedString> ();
        public int Count {get {return annotatedStrings.Count;}}

        //********************************************************************

        // "foreach" support. Google "yield" keyword for explanation

        public IEnumerator<AnnotatedString> GetEnumerator()
        {
            foreach (var astring in annotatedStrings)
            {
                yield return astring; // Yield elements one by one
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Simply reuse the generic method above
        }

        //**************************************************************************

        // Indexing support

        public AnnotatedString this [int index]
        {
            get => annotatedStrings [index];
            set => annotatedStrings [index] = value;
        }

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
                annotatedStrings.Add (astr);
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
                annotatedStrings.Add (aTrimmed);
                startIndex = endIndex + 2;
            }

            // one more outside of loop if input string does not end in semicolon
            if (startIndex < str.Length)// - 1)
            {
                string partial = str.Substring (startIndex, str.Length - startIndex);
                string trimmed = partial.Trim (new char [] {' '});

                AnnotatedString aTrimmed = new AnnotatedString (trimmed);
                aTrimmed.SupressPrinting = astr.SupressPrinting;
                annotatedStrings.Add (aTrimmed);
            }
        }
    }
}
