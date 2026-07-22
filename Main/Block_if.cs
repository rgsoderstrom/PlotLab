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
        private static readonly List<string> IfBlockCloseKeywords = new List<string> () {"elseif", "else", "end"};

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

            Print?.Invoke ("new \"if\" block");
            PartialBlock = new TestCodePair (astr.Arguments);
        }

        // Add to "code" section
        internal override void Add (AnnotatedString astr)
        {
            if (IfBlockCloseKeywords.Contains (astr.FirstWord))
            {
                IfBlockSections.Add (PartialBlock);
                PartialBlock = null;

                switch (astr.FirstWord)
                {
                    case "elseif":
                        PartialBlock = new TestCodePair (astr.Arguments);
                        break;

                    case "else":
                        PartialBlock = new TestCodePair ();
                        break;

                    case "end":
                        Complete = true;
                        break;

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

            Print?.Invoke ("Running \"if\" block " + Name);

            foreach (TestCodePair oneBlock in IfBlockSections)
            { 
                Print?.Invoke (oneBlock.ToString ());

                //
                // evaluate this blocks "Test"
                //

                string str = oneBlock.test.Trim ();

                if (str.EndsWith (","))
                    str = str.Remove (str.Length - 1, 1);

                if (str [0] == '(' && str [str.Length-1] == ')')
                {
                    str = str.Remove (0, 1);
                    str = str.Remove (str.Length-1, 1);
                }

                AnnotatedString astr = new AnnotatedString (str);
                ExpressionTree tree = new ExpressionTree (astr);
                PLVariable answer = tree.Evaluate ();
                PLBool TF = answer as PLBool;

                if (TF == null)
                    throw new Exception ("if block test not a boolean: " + str);

                if (TF.Data == true)
                { 
                    foreach (AnnotatedString astr2 in oneBlock.code)
                    { 
                        tree = new ExpressionTree (astr2);
                        answer = tree.Evaluate ();
                        if (tree.SupressPrinting == false)
                            Print?.Invoke (answer.ToString ());
                    }

                    break; // don't run any more tests
                }
            }
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
