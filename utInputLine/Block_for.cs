using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    internal class ForBlock : Block
    {
        internal ForBlock (AnnotatedString astr)
        {
            Print ("new \"for\" block");

            // Extract loop control
            //
            //    Look for first comma.
            //       for a = 1:9, b = a ^ 2;
            //
            //    if none found, use everything after "for"
            //

            //int startIndex = "for".Length; // start looking here
            //int i;

            //for (i=startIndex; i<str.Length; i++)
            //{
            //    if (str [i] == ',')
            //        break;
            //}

            //loopControl = str.Substring (startIndex, i - startIndex).Trim ();
            //Print ("loop control: " + loopControl);

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

        //internal override void Run ()
        //{
        //    Print ("Running for " + loopControl + " block");

        //    InputLineProcessor ip = new InputLineProcessor (Print);


        //    foreach (string str in blockStatements)
        //    {
        //        Print ("  " + str);
        //        ip.ProcessString (str);
        //    }
        //}
    }
}
