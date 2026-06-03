using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLMain;

namespace utAnnotatedStringUtils
{
    internal class Driver
    {
        static void Main (string [] _)
        {
            //AnnotatedString annotated = new AnnotatedString ("a\"bc\"de");
            //annotated = AnnotatedString.Append (annotated, "(22)");

            //AnnotatedString annotated = new AnnotatedString ("clear a b c");
            AnnotatedString annotated = new AnnotatedString ("Script12");
            //AnnotatedString annotated = new AnnotatedString ("figure (1)");

            Console.WriteLine ("Alpha only = " + annotated.AlphanumericOnly);
            Console.WriteLine ("First Word = " + annotated.FirstWord);
            Console.WriteLine ("Args:");
            foreach (string str in annotated.Arguments) Console.WriteLine (str);
            Console.WriteLine ();

            //AnnotatedString annotated = new AnnotatedString ("-");
            //annotated = AnnotatedString.Append (annotated, '1');
            //annotated = AnnotatedString.Append (annotated, '2');
            //annotated = AnnotatedString.Append (annotated, "3;");

            Console.WriteLine (annotated.Plain.ToString ());
            Console.WriteLine (annotated.ToString ());
            Console.WriteLine ("\n-----------------------------------------\n");

            //AnnotatedString annotatedString = annotated.TrimmedSubstring (3, 4);
            //AnnotatedString ann2 = annotatedString;
            //Console.WriteLine (ann2.ToString ());

        }
    }
}
