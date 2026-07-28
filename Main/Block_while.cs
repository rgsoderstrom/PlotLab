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
        private readonly TestCodePair BlockStatements = new TestCodePair ();

        //************************************************************************

        internal WhileBlock (AnnotatedString astr)
        {
            BlockStatements = new TestCodePair (astr.Arguments);
        }

        internal override void Add (AnnotatedString astr)
        {
            BlockStatements.Add (astr);
        }

        internal override void Run ()
        {
            bool done = false;

            while (done == false)
            { 
                string testString = BlockStatements.test.Trim ();

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
                    throw new Exception ("while block " + Name + " test not a boolean: " + testString);

                if ((answer as PLBool).Data == false)
                    return;

                InputLineProcessor ilp = new InputLineProcessor ();

                foreach (AnnotatedString astr2 in BlockStatements.code)
                { 
                    string str = astr2.Plain;

                    if (str == "break")
                    {
                        done = true;
                        break;
                    }

                    if (str == "continue")
                    {
                        break;
                    }

                    ilp.ProcessString (str);
                }
            }
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "WhileBlock " + Name;

            str += "\n  Test: " + BlockStatements.test;

            foreach (AnnotatedString astr in BlockStatements.code)
                str += "\n     " + astr.Plain;

            return str;
        }
    }
}
