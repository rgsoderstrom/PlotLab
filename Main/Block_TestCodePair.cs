using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    // TestCodePair - a component of various blocks
    //  - "if" source code is split into a list of these

    internal class TestCodePair
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
