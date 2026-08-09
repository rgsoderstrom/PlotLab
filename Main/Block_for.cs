using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    internal class ForBlock : LoopBlock
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

        // Unique names for these required for nested "for" blocks to work as expected
        
        static string GetVarBase   = "get";
        static string CountVarBase = "count";
        static string CasesVarBase = "cases";

        private string GetVar;
        private string CountVar;
        private string CasesVar;

        private string GenerateNames ()
        {
            int N = instanceCounter - 1; // makes the number appended to variables match
                                         // the number appended to the block name
            GetVar   = GetVarBase   + N;
            CountVar = CountVarBase + N;
            CasesVar = CasesVarBase + N;

            return "while " + GetVar + " <= " + CountVar + ",";
        }

        internal ForBlock (AnnotatedString astr) : base ()
        {
            // set test in base class loop block
            SetBlockTest (GenerateNames ());

            // initialization code
            string loopArgs     = astr.Arguments; // a = 12 : 15, % in example
            int    index        = loopArgs.IndexOf ('=');

            // the equal sign and everything past it
            string cases = loopArgs.Substring (index); 
            InitializationCode.Add (new AnnotatedString (CasesVar + " " + cases + ";")); // Cases = 12 : 15;

            // count = 4; // size (cases, 2) == 4
            InitializationCode.Add (new AnnotatedString (CountVar + " = size (" + CasesVar + ", 2);"));

            // get = 1; 
            InitializationCode.Add (new AnnotatedString (GetVar + " = 1;"));

            // a = cases (get);
            string loopVariable = loopArgs.Substring (0, index - 1).Trim ();
            Add (new AnnotatedString (loopVariable + " = " + CasesVar + " (" + GetVar + ");"));

            CleanupCode.Add (new AnnotatedString ("clear " + CasesVar + " " + CountVar + " " + GetVar));
        }

        //******************************************************************************

        internal override void Close ()
        {
            Add (new AnnotatedString (GetVar + " = " + GetVar + " + 1;"));  // ("get = get + 1;"));
        }

        //******************************************************************************

        internal override TerminationReason Run ()
        {
            InputLineProcessor ilp = new InputLineProcessor ();
            PLVariable unused = null;            

            // run initialization
            foreach (AnnotatedString astr in InitializationCode)
                ilp.ProcessString (ref unused, astr.Plain);

            // run loop code
            TerminationReason reason = base.Run ();

            // run cleanup
            foreach (AnnotatedString astr in CleanupCode)
                ilp.ProcessString (ref unused, astr.Plain);

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
