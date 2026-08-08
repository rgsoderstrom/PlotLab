
/*
    Block_switch.cs - 
*/

using System;
using System.Collections.Generic;

using PLCommon;

namespace PLMain
{
    internal class SwitchBlock : SelectBlock
    {
	
	
	
	
	

        private static readonly List<string> IfBlockKeywords = new List<string> () {"elseif", "else"};

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
            SelectBlockSections.Add (PartialBlock);
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
                        SelectBlockSections.Add (PartialBlock);
                        break;

                    case "else":
                        PartialBlock = new TestCodePair ();
                        SelectBlockSections.Add (PartialBlock);
                        break;

                    default:
                        throw new Exception ("Unsupported \"if\" statement break: " + astr.Plain);
                }
            }

            else
                PartialBlock.Add (astr);            
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "IfBlock " + Name + " has " + SelectBlockSections.Count.ToString () + " sections";

            foreach (TestCodePair tp in SelectBlockSections)
            {
                str += "\n  Test: " + tp.test;

                foreach (AnnotatedString astr in tp.code)
                    str += "\n     " + astr.Plain;
            }

            return str;
        }
    }
}
