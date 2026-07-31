
/*
    ExpressionTree_Stub
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    internal class ExpressionTree
    {
        private bool supressPrinting = false;
        //private bool supressPrinting = true;
        public  bool SupressPrinting {get {return supressPrinting;} private set {supressPrinting = value;}}

        private readonly PLVariable Answer = new PLNull ();


        static PLDouble a = new PLDouble ("a", 0);
        static PLDouble b = new PLDouble ("b", 10);
        static PLDouble c = new PLDouble ("c", 20);

        internal ExpressionTree (AnnotatedString astr)
        {
            switch (astr.Plain)
            {
                case "a = a + 1":
                    a = new PLDouble (a.Data + 1);
                    Answer = a;
                    break;

                case "b = b + 1":
                    b = new PLDouble (b.Data + 1);
                    Answer = b;
                    break;

                case "c = c + 1":
                    c = new PLDouble (c.Data + 1);
                    Answer = c;
                    break;

                case "a < 8": 
                    Answer = new PLBool (a.Data < 8);
                    break;

                case "b > 12": 
                    Answer = new PLBool (b.Data > 12);
                    break;

                case "b <= 12": 
                    Answer = new PLBool (b.Data <= 12);
                    break;

                case "c > 22": 
                    Answer = new PLBool (c.Data > 22);
                    break;

                case "true":
                    Answer = new PLBool (true);
                    break;

                case "false":
                    Answer = new PLBool (false);
                    break;

                default:
                    throw new Exception ("ExpressionTree stub can't evaluate " + astr.Plain);

            }
        }

        internal PLVariable Evaluate ()
        {
            return Answer;
        }
    }
}
