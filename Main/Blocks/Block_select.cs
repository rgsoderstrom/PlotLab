
/*
    Block_select.cs - abstract base class for "if" and "switch" blocks    
*/

using System;
using System.Collections.Generic;

using PLCommon;

namespace PLMain
{
    internal abstract class SelectBlock : Block
    {
        protected readonly List<TestCodePair> SelectBlockSections = new List<TestCodePair> ();
        protected TestCodePair PartialBlock = null;

        internal override void Close ()
        {
        }

        //***************************************************************************

        internal override BlockTerminationReason Run ()
        {
            foreach (TestCodePair oneBlock in SelectBlockSections)
            { 
                //
                // evaluate this blocks "Test"
                //

                string testString = oneBlock.test.Trim ();

                // remove optional comma
                if (testString.EndsWith (","))
                    testString = testString.Remove (testString.Length - 1, 1);

                // remove any optional parens

                // this caused error evaluating exresssions like:
                // if (size (zValues, 1) ~= NR) || (size (zValues, 2) ~= NC)

                //if (testString [0] == '(' && testString [testString.Length-1] == ')')
                //{
                //    testString = testString.Remove (0, 1);
                //    testString = testString.Remove (testString.Length-1, 1);
                //}

                ExpressionTree tree = new ExpressionTree (new AnnotatedString (testString));
                PLVariable answer = tree.Evaluate ();

                if ((answer as PLBool) == null)
                    throw new Exception ("if block " + Name + " test not a boolean: " + testString);

                if ((answer as PLBool).Data == true)
                { 
                    InputLineProcessor ilp = new InputLineProcessor ();

                    foreach (AnnotatedString astr in oneBlock.code)
                    { 
                        string str = astr.Plain;

                        if (str == "return")
                            return BlockTerminationReason.ReturnEncountered;

                        if (str == "break")
                            return BlockTerminationReason.BreakEncountered;

                        if (str == "continue")
                            return BlockTerminationReason.ContinueEncountered;

                        PLVariable unused = null;
                        BlockTerminationReason status = ilp.ProcessString (ref unused, str);

                        // "if" blocks pass these up to parent block
                        if (status == BlockTerminationReason.ContinueEncountered || status == BlockTerminationReason.BreakEncountered)
                            return status;
                    }

                    break; // one test passed, so don't run any more
                }
            }

            return BlockTerminationReason.Completed;
        }

    }
}
