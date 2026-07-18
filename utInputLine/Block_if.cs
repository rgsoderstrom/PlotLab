using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    internal class IfBlock : Block
    {
        private static readonly List<string> IfBlockCloseKeywords = new List<string> () {"elseif", "else", "end"};

        static int instanceCounter = 1;

        //************************************************************************

        private class TestCodePair
        {
            readonly string test;
            readonly List<AnnotatedString> code;
        
            internal TestCodePair (string str) {test = str; code = new List<AnnotatedString> ();}        
            internal TestCodePair () : this ("true") {} 
            internal void Add (AnnotatedString astr) {code.Add (astr);}

            public override string ToString ()
            {
                string str = "Test: " + test;

                foreach (AnnotatedString astr in code)
                    str += "\n      " + astr.Plain;

                str += "\n";
                return str;
            }
        }

        private readonly List<TestCodePair> IfBlockSections = new List<TestCodePair> ();
        private TestCodePair PartialBlock = null;

        //************************************************************************

        internal IfBlock (AnnotatedString astr)
        {
            Print?.Invoke ("new \"if\" block");
            Name = "IF" + instanceCounter++.ToString ();
            Complete = false;
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
            Print?.Invoke ("Running \"if\" block " + Name);

            foreach (TestCodePair tcp in IfBlockSections)
                Print?.Invoke (tcp.ToString ());
        }

    }
}
