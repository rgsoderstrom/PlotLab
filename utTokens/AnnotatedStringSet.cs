
/*
    AnnotatedStringSet - list of AnnotatedStrings
*/

using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedStringSet
    {
        // queue of complete AnnotatedStrings,
        private readonly Queue<AnnotatedString> annotatedStrings = new Queue<AnnotatedString> ();

        // number of complete string ready for processing
        public int Count {get {return annotatedStrings.Count;}}

        public AnnotatedString GetOldest {get {return annotatedStrings.Dequeue ();}}
        public void            Clear () {annotatedStrings.Clear ();}

        static private readonly string continuationString = "...";

        //**************************************************************************

        public AnnotatedStringSet ()
        {
        }

        //**********************************************************************

        private string cumulative = "";

        public void Add (string fileLine)
        {
            string cleanedInput = InputLineProcessor.PreprocessInputLine (fileLine);

            if (cleanedInput.Length == 0)
                return;

            bool continues = false;

            if (cleanedInput.EndsWith (continuationString))
            {
                cleanedInput = cleanedInput.Remove (cleanedInput.Length - continuationString.Length);
                continues = true;
            }                

            cumulative += cleanedInput;

            if (continues == true)
                return;

            Add (new AnnotatedString (cumulative));
            cumulative = "";

            return;
        }

        //*************************************************************************

        public void Add (AnnotatedString astr)
        {
            if (astr.IsCompound == false)
            {
                annotatedStrings.Enqueue (astr);
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
