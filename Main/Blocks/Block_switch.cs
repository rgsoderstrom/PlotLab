
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
        private static readonly List<string> SwitchBlockKeywords = new List<string> () {"case", "otherwise"};
	
        //switch day
        //    case 'Monday'
        //        disp('Start of the work week.')
        //    case 'Friday'
        //        disp('Last day of the work week.')
        //    case 'Saturday'
        //        disp('It is the weekend!')
        //    otherwise
        //        disp('Just another regular day.')
        //end
	
        private readonly string switchVar;

        internal SwitchBlock (AnnotatedString astr)
        {
            switchVar = astr.Arguments;
        }

        // Add to "code" section
        internal override void Add (AnnotatedString astr)
        {
            string firstWord = astr.FirstWord;

            if (SwitchBlockKeywords.Contains (firstWord))
            {
                switch (firstWord)
                {
                    case "case":
                        PartialBlock = new TestCodePair (switchVar + " == " + astr.Arguments);
                        SelectBlockSections.Add (PartialBlock);
                        break;

                    case "otherwise":
                        PartialBlock = new TestCodePair ();
                        SelectBlockSections.Add (PartialBlock);
                        break;

                    default:
                        throw new Exception ("Unsupported \"switch\" statement break: " + astr.Plain);
                }
            }

            else
                PartialBlock.Add (astr);            
        }

        //************************************************************************

        public override string ToString ()
        {
            string str = "SwitchBlock " + Name + " has " + SelectBlockSections.Count.ToString () + " sections";

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
