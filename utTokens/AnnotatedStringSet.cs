
/*
    AnnotatedStringSet - list of AnnotatedStrings
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Main
{
    public class AnnotatedStringSet
    {
        private List<AnnotatedString> annotatedStrings = new List<AnnotatedString> ();
        public int Count {get {return annotatedStrings.Count;}}

        //**************************************************************************

        // ctors

        public AnnotatedStringSet (string str)
        {
            AnnotatedString astr = new AnnotatedString (str);

            // if input str is only a single expression, e.g. a = 123; annotate and we're done
            if (astr.IsCompound == false)
            {
                annotatedStrings.Add (astr);
                return;
            }

            // if we get here str is a compound expression, e.g. a = 123; b = 456; c = 789;
            // it will be split into a list of annotated strings
            
            int startIndex = 0;
            List<int> indices = astr.Level0Semis; // these are the breaks between expressions

            for (int i = 0; i<indices.Count; i++)
            {
                int endIndex = indices [i]; // stop copying after this character

                string partial = str.Substring (startIndex, endIndex - startIndex + 1);
                string trimmed = partial.Trim (new char [] {' '});

                annotatedStrings.Add (new AnnotatedString (trimmed));
                startIndex = endIndex + 1;
            }

            // one more outside of loop if input string does not end in semicolon
            if (startIndex < str.Length - 1)
            {
                string partial = str.Substring (startIndex, str.Length - startIndex);
                string trimmed = partial.Trim (new char [] {' '});

                annotatedStrings.Add (new AnnotatedString (trimmed));
            }
        }

        //**************************************************************************

        public AnnotatedStringSet (AnnotatedString astr)
        {
            throw new NotImplementedException ("ctor");
        }

        // Add - add a string or an annotated string to existing list

        public void Add (string str)
        {
            throw new NotImplementedException ("Add");
        }

        public void Add (AnnotatedString astr)
        {
            throw new NotImplementedException ("Add");
        }

        //*******************************************************************
        //
        // Indexer
        //
        public AnnotatedString this [int index]
        {
            get
            {
                if (index >= 0 && index < annotatedStrings.Count)
                    return annotatedStrings [index];

                throw new IndexOutOfRangeException ("Index is out of range in AnnotatedStringSet indexer get.");
            }

            set
            {
                if (index >= 0 && index < annotatedStrings.Count)
                    annotatedStrings [index] = value;

                else
                    throw new IndexOutOfRangeException ("Index is out of range in AnnotatedStringSet indexer set.");
            }
        }

    }
}
