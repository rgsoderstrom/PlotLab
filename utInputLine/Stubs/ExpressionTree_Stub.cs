
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
        private static int instanceCounter = 0;

        private bool supressPrinting = false;
        //private bool supressPrinting = true;
        public  bool SupressPrinting {get {return supressPrinting;} private set {supressPrinting = value;}}

        private readonly PLVariable Answer = new PLNull ();


        static PLDouble a = new PLDouble ("a", 0);
        static PLDouble b = new PLDouble ("b", 0);
        static PLDouble c = new PLDouble ("c", 0);

        internal ExpressionTree (AnnotatedString astr)
        {
            instanceCounter++;

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

                case "A > B": 
                    Answer = new PLBool (instanceCounter < 25);
                    break;

                case "C > D": 
                    Answer = new PLBool (instanceCounter < 20);
                    break;

                case "E > F": 
                    Answer = new PLBool (instanceCounter < 15);
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
