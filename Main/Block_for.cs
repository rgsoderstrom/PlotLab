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

        // test & code use "while" loop structures

        // test string
        //    get <= count;

        // code list
        //    a = cases (get);    
        //    <loop code>
        //    get = get + 1;

        // followed by Cleanup
        //    clear cases count get

        //******************************************************************************

        private readonly List<AnnotatedString> InitializationCode = new List<AnnotatedString> ();
        private readonly List<AnnotatedString> CleanupCode = new List<AnnotatedString> ();

        //******************************************************************************

        internal ForBlock (AnnotatedString astr) : base (new AnnotatedString ("while get <= count,"))
        {
            Console.WriteLine ("new ForBlock " + astr.Plain);

            // initialization code
            string loopArgs     = astr.Arguments; // a = 12 : 15, % in example
            int    index        = loopArgs.IndexOf ('=');
            string loopVariable = loopArgs.Substring (0, index - 1).Trim (); // a
            InitializationCode.Add (new AnnotatedString ("cases " + loopArgs.Substring (index) + ";"));
            InitializationCode.Add (new AnnotatedString ("count = size (cases, 2);"));
            InitializationCode.Add (new AnnotatedString ("get = 1;"));

            Add (new AnnotatedString (loopVariable + " = cases (get);"));

            CleanupCode.Add (new AnnotatedString ("clear cases count get;"));
        }

        //******************************************************************************

        internal override void Add (AnnotatedString astr)
        {
            base.Add (astr);
            Console.WriteLine ("Add " + astr.Plain);
        }

        //******************************************************************************

        internal override void Close ()
        {
            Add (new AnnotatedString ("get = get + 1;"));
            Console.WriteLine ("Close");
        }

        //******************************************************************************

        internal override TerminationReason Run ()
        {
            Console.WriteLine ("run");

            InputLineProcessor ilp = new InputLineProcessor ();

            // run initialization
            foreach (AnnotatedString astr in InitializationCode)
                ilp.ProcessString (astr.Plain);

            // run loop code
            TerminationReason reason = base.Run ();

            // run cleanup
            foreach (AnnotatedString astr in CleanupCode)
                ilp.ProcessString (astr.Plain);

            return reason;
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "ForBlock " + Name;

            str += "\n  Initialization: ";

            foreach (AnnotatedString astr in InitializationCode)
                str += "\n     " + astr.Plain;

            str += "\n  Test: " + base.BlockStatements.test;

            str += "\n  loop code: ";

            foreach (AnnotatedString astr in BlockStatements.code)
                str += "\n     " + astr.Plain;

            str += "\n  Cleanup: ";

            foreach (AnnotatedString astr in CleanupCode)
                str += "\n     " + astr.Plain;

            return str;
        }
      
    }
}
