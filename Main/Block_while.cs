using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    internal class WhileBlock : Block
    {
        protected TestCodePair BlockStatements;// = new TestCodePair ();

        //************************************************************************

        internal WhileBlock (AnnotatedString astr)
        {
            BlockStatements = new TestCodePair (astr.Arguments);
        }

        protected internal WhileBlock ()
        {
        }

        protected void SetBlockTest (string arguments)
        {
            BlockStatements = new TestCodePair (arguments);
        }

        //************************************************************************

        internal override void Add (AnnotatedString astr)
        {
            BlockStatements.Add (astr);
        }

        //************************************************************************

        internal override void Close ()
        {
        }

        //************************************************************************

        internal override TerminationReason Run ()
        {
            // extract and format while block "test"
            string testString = BlockStatements.test.Trim ();

            if (testString.EndsWith (","))
                testString = testString.Remove (testString.Length - 1, 1);

            if (testString [0] == '(' && testString [testString.Length-1] == ')')
            {
                testString = testString.Remove (0, 1);
                testString = testString.Remove (testString.Length-1, 1);
            }

            bool done = false;

            while (done == false)
            { 
                // run the test before each loop
                ExpressionTree tree = new ExpressionTree (new AnnotatedString (testString));
                PLVariable answer = tree.Evaluate ();

                if ((answer as PLBool) == null)
                    throw new Exception ("while block " + Name + " test not a boolean: " + testString);

                if ((answer as PLBool).Data == false)
                    return TerminationReason.Completed;

                // if we get here, test has passed so run all satements in "code" section
                InputLineProcessor ilp = new InputLineProcessor ();

                foreach (AnnotatedString astr2 in BlockStatements.code)
                { 
                    string str = astr2.Plain;

                    // if "break" executed, this block terminates but any containg block does
                    // not need to take any special action
                    if (str == "break")
                        return TerminationReason.Completed;

                    // on "continue" just skip to end and iterate again
                    if (str == "continue")
                        break;

                    TerminationReason reason = ilp.ProcessString (str);

                    // if BreakEncountered passed up from the block just completed, his block terminates but
                    // any containing block does not need to take any special action
                    //    - typically an "if" block 
                    if (reason == TerminationReason.BreakEncountered) 
                        return TerminationReason.Completed;

                    // if ContinueEncountered passed up from the block just completed, this remaining statements
                    // in the code block of are skipped
                    if (reason == TerminationReason.ContinueEncountered)
                        break;
                }
            }

            return TerminationReason.Completed;
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "WhileBlock " + Name;

            str += "\n  Test: " + BlockStatements.test;

            str += "\n  loop code: ";

            foreach (AnnotatedString astr in BlockStatements.code)
                str += "\n     " + astr.Plain;

            return str;
        }
    }
}
