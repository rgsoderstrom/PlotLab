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
        private static readonly List<string> IfBlockCloseKeywords = new List<string> () {"elseif", "else"};//, "end"};

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
            string str = astr.Plain;

            //Print?.Invoke ("new \"if\" block");
            PartialBlock = new TestCodePair (astr.Arguments);
            IfBlockSections.Add (PartialBlock);
        }

        // Add to "code" section
        internal override void Add (AnnotatedString astr)
        {
            //Print?.Invoke ("Adding statement " + astr.Plain + " to " + Name);

            if (IfBlockCloseKeywords.Contains (astr.FirstWord))
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

                    //case "end":
                    //    Complete = true;
                    //    break;

                    default:
                        throw new Exception ("Unsupported \"if\" statement break: " + astr.Plain);
                }
            }

            else
                PartialBlock.Add (astr);            
        }

        internal override void Run ()
        {
            InputLineProcessor proc = new InputLineProcessor ();

            foreach (TestCodePair oneBlock in IfBlockSections)
            { 
                //
                // evaluate this blocks "Test"
                //

                string testSTring = oneBlock.test.Trim ();

                if (testSTring.EndsWith (","))
                    testSTring = testSTring.Remove (testSTring.Length - 1, 1);

                if (testSTring [0] == '(' && testSTring [testSTring.Length-1] == ')')
                {
                    testSTring = testSTring.Remove (0, 1);
                    testSTring = testSTring.Remove (testSTring.Length-1, 1);
                }

                ExpressionTree tree = new ExpressionTree (new AnnotatedString (testSTring));
                PLVariable answer = tree.Evaluate ();

                if ((answer as PLBool) == null)
                    throw new Exception ("if block test not a boolean: " + testSTring);

                if ((answer as PLBool).Data == true)
                { 
                    InputLineProcessor ilp = new InputLineProcessor (Console.WriteLine);

                    foreach (AnnotatedString astr2 in oneBlock.code)
                    { 
                        string str = astr2.Plain;
                        ilp.ProcessString (astr2.Plain);

                        //tree = new ExpressionTree (astr2); // what about scripts, commands, etc??????????????????
                        //answer = tree.Evaluate ();
                        //if (tree.SupressPrinting == false)
                        //    Print?.Invoke (answer.ToString ());
                    }

                    break; // don't run any more tests
                }
            }
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = Name + " has " + IfBlockSections.Count.ToString () + " sections";

            foreach (TestCodePair tp in IfBlockSections)
            {
                str += "\n  Test: " + tp.test;

                foreach (AnnotatedString astr in tp.code)
                    str += "\n     " + astr.Plain;
            }

            return str;
        }


        //************************************************************************
        //************************************************************************
        //************************************************************************

        // TestCodePair - "if" source code is split into a list of these

        private class TestCodePair
        {
            readonly internal string test;
            readonly internal List<AnnotatedString> code;
        
            internal TestCodePair (string str) 
            {
                test = str; 
                code = new List<AnnotatedString> ();
            }   
            
            internal TestCodePair () : this ("true") 
            {
            } 

            internal void Add (AnnotatedString astr) 
            {
                code.Add (astr);
            }

            public override string ToString ()
            {
                string str = "Test: " + test;

                foreach (AnnotatedString astr in code)
                    str += "\n      " + astr.Plain;

                str += "\n";
                return str;
            }
        }
    }

}
