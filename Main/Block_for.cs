using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    internal class ForBlock : WhileBlock
    {
        // Typical supported syntax:
        //    for a = 12 : 15,
        //      <loop code>
        //    end

        // initialization list, runs before first iteration
        //    cases = 12 : 15;
        //    count = 4;
        //    get = 1;

        // remainder is same structure as "while" loop

        // test string
        //    get <= count;

        // code list
        //    a = cases (get);    
        //    <loop code>
        //    get = get + 1;

        //******************************************************************************

        List<AnnotatedString> InitializationCode = new List<AnnotatedString> ();

        //******************************************************************************

        internal ForBlock (AnnotatedString astr) : base (new AnnotatedString ("get <= count"))
        {
            Console.WriteLine ("new ForBlock " + astr.Plain);

            string loopArgs     = astr.Arguments; // a = 12 : 15,
            int    index        = loopArgs.IndexOf ('=');
            string loopVariable = loopArgs.Substring (0, index - 1).Trim (); // a
            InitializationCode.Add (new AnnotatedString ("cases " + loopArgs.Substring (index) + ";"));
            InitializationCode.Add (new AnnotatedString ("count = size (cases, 2);"));
            InitializationCode.Add (new AnnotatedString ("get = 1;"));

            Add (new AnnotatedString (loopVariable + " = cases (get);"));
        }

        internal override void Add (AnnotatedString astr)
        {
            base.Add (astr);
            Console.WriteLine ("Add " + astr.Plain);

        }

        internal override void Close ()
        {
            Add (new AnnotatedString ("get = get + 1;"));
            Console.WriteLine ("Close");
        }

        internal override TerminationReason Run ()
        {
            Console.WriteLine ("run");
            return TerminationReason.Completed;
        }

      
    }
}
