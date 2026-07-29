
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

        internal ExpressionTree (AnnotatedString astr)
        {
            instanceCounter++;

            switch (astr.Plain)
            {
                case "A > B": 
                    Answer = new PLBool (instanceCounter < 15);
                    break;

                case "C > D": 
                    Answer = new PLBool (instanceCounter < 5);
                    break;

                case "E > F": 
                    Answer = new PLBool (instanceCounter < 10);
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
