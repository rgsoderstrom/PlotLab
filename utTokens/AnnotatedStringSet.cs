
/*
    AnnotatedStringSet - list of AnnotatedStrings
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace PLMain
{
    public class AnnotatedStringSet
    {
        // queue of complete AnnotatedStrings,
        private readonly Queue<AnnotatedString> annotatedStrings = new Queue<AnnotatedString> ();

        // number of complete string ready for processing
        public int Count {get {return annotatedStrings.Count;}}

        //**************************************************************************

        public AnnotatedStringSet ()
        {
        }

        //*************************************************************************

        public AnnotatedString GetOldest {get {return annotatedStrings.Dequeue ();}}
        public void            Clear () {annotatedStrings.Clear ();}

        public void Add (AnnotatedString astr)
        {
            if (astr.IsCompound == false)
            {
                annotatedStrings.Enqueue (astr);
                return;
            }



            // If we get here str is a compound expression, e.g. a = 123; b = 456; c = 789;
            // It will be split into a list of annotated strings

            int startIndex = 0;
            List<int> indices = astr.Level0Semis; // these are indices of the breaks between expressions

            string str = astr.Plain;

            for (int i = 0; i<indices.Count; i++)
            {
                int endIndex = indices [i]; // stop copying after this character

                string partial = str.Substring (startIndex, endIndex - startIndex + 1);
                string trimmed = partial.Trim (new char [] { ' ' });

                annotatedStrings.Enqueue (new AnnotatedString (trimmed));
                startIndex = endIndex + 1;
            }

            // one more outside of loop if input string does not end in semicolon
            if (startIndex < str.Length - 1)
            {
                string partial = str.Substring (startIndex, str.Length - startIndex);
                string trimmed = partial.Trim (new char [] { ' ' });

                annotatedStrings.Enqueue (new AnnotatedString (trimmed));
            }
        }
    }
}
