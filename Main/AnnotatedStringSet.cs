
/*
    AnnotatedStringSet - list of AnnotatedStrings
*/

using System.Collections.Generic;

namespace PLMain
{
    public class AnnotatedStringSet
    {
//        // queue of complete AnnotatedStrings,
//        private Queue<AnnotatedString> annotatedStrings = new Queue<AnnotatedString> ();

//        // number of complete string ready for processing
//        public int Count {get {return annotatedStrings.Count;}}



//     // public AnnotatedString GetOldest {get {return annotatedStrings.Dequeue ();}}
//        public AnnotatedString GetOldest () {return annotatedStrings.Dequeue ();}

//        public AnnotatedString PeekOldest {get {return annotatedStrings.Peek ();}}
//        public void Pop () {annotatedStrings.Dequeue ();}







//        public void Clear () {annotatedStrings.Clear ();}


//        //**************************************************************************

//        public AnnotatedStringSet ()
//        {
//        }


//        //*************************************************************************

//        public void Add (AnnotatedString astr)
//        {
//            if (astr.IsCompound == false)
//            {
//                annotatedStrings.Enqueue (astr);
//                return;
//            }

//            // If we get here astr is a compound expression, e.g. a = 123; b = 456; c = 789; all on one line.
//            // It will be split into a list of annotated strings

//            // if input astr.SuppressOutput is true, restore trailing semicolon
//            //if (astr.SuppressOutput)
//            //    astr = AnnotatedString.Append (astr, ';');


//            int startIndex = 0;
//            List<int> indices = astr.Level0Semis; // these are indices of the breaks between expressions

//            string str = astr.Plain;

//            for (int i = 0; i<indices.Count; i++)
//            {
//                int endIndex = indices [i]; // stop copying after this character

//                string partial = str.Substring (startIndex, endIndex - startIndex + 1);
//                string trimmed = partial.Trim (new char [] { ' ' });

//                annotatedStrings.Enqueue (new AnnotatedString (trimmed));
//                startIndex = endIndex + 1;
//            }

//            // one more outside of loop if input string does not end in semicolon
//            if (startIndex < str.Length - 1)
//            {
//                string partial = str.Substring (startIndex, str.Length - startIndex);
//                string trimmed = partial.Trim (new char [] { ' ' });

//                annotatedStrings.Enqueue (new AnnotatedString (trimmed));
//            }
//        }
    }
}
