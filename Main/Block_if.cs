using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    internal class IfBlock : Block
    {
        private static readonly List<string> IfBlockKeywords = new List<string> () {"elseif", "else"};

        private readonly List<TestCodePair> IfBlockSections = new List<TestCodePair> ();
        private TestCodePair PartialBlock = null;

        //************************************************************************
        //************************************************************************
        //************************************************************************

        // Supported syntax:
        //   Single line
        //     if A > B, c = A * B; ... end
        //     if (A > B), c = A * B; ... end
        //   Multi line
        //     if A > B
        //     if (A > B)
        //     if A > B,
        //     if (A > B),
        //
        //   All must terminate with "end"

        // also elseif and else

        internal IfBlock (AnnotatedString astr)
        {
            PartialBlock = new TestCodePair (astr.Arguments);
            IfBlockSections.Add (PartialBlock);
        }

        // Add to "code" section
        internal override void Add (AnnotatedString astr)
        {
            if (IfBlockKeywords.Contains (astr.FirstWord))
            {
                switch (astr.FirstWord)
                {
                    case "elseif":
                        PartialBlock = new TestCodePair (astr.Arguments);
                        IfBlockSections.Add (PartialBlock);
                        break;

                    case "else":
                        PartialBlock = new TestCodePair ();
                        IfBlockSections.Add (PartialBlock);
                        break;

                    default:
                        throw new Exception ("Unsupported \"if\" statement break: " + astr.Plain);
                }
            }

            else
                PartialBlock.Add (astr);            
        }

        //***************************************************************************

        internal override void Close ()
        {
        }

        //***************************************************************************

        internal override TerminationReason Run ()
        {
            foreach (TestCodePair oneBlock in IfBlockSections)
            { 
                //
                // evaluate this blocks "Test"
                //

                string testString = oneBlock.test.Trim ();

                if (testString.EndsWith (","))
                    testString = testString.Remove (testString.Length - 1, 1);

                if (testString [0] == '(' && testString [testString.Length-1] == ')')
                {
                    testString = testString.Remove (0, 1);
                    testString = testString.Remove (testString.Length-1, 1);
                }

                ExpressionTree tree = new ExpressionTree (new AnnotatedString (testString));
                PLVariable answer = tree.Evaluate ();

                if ((answer as PLBool) == null)
                    throw new Exception ("if block " + Name + " test not a boolean: " + testString);

                if ((answer as PLBool).Data == true)
                { 
                    InputLineProcessor ilp = new InputLineProcessor ();

                    foreach (AnnotatedString astr2 in oneBlock.code)
                    { 
                        string str = astr2.Plain;

                        if (str == "break")
                            return TerminationReason.BreakEncountered;

                        if (str == "continue")
                            return TerminationReason.ContinueEncountered;

                        TerminationReason status = ilp.ProcessString (str);

                        // "if" blocks pass these up to parent block
                        if (status == TerminationReason.ContinueEncountered || status == TerminationReason.BreakEncountered)
                            return status;
                    }

                    break; // one test passed, so don't run any more
                }
            }

            return TerminationReason.Completed;
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "IfBlock " + Name + " has " + IfBlockSections.Count.ToString () + " sections";

            foreach (TestCodePair tp in IfBlockSections)
            {
                str += "\n  Test: " + tp.test;

                foreach (AnnotatedString astr in tp.code)
                    str += "\n     " + astr.Plain;
            }

            return str;
        }
    }
}
