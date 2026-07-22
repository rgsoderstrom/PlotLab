
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
        private static readonly Dictionary<string, PLVariable> IfBlockTests = new Dictionary<string, PLVariable> ()
        {
            ["A > B"]  = new PLBool (true),
            ["A > 19"] = new PLBool (false),
            ["true"]   = new PLBool (true),
            ["false"]  = new PLBool (false),
        };

        private static readonly Dictionary<string, PLVariable> CodeEvaluation = new Dictionary<string, PLVariable> ()
        {
            ["c = A * B"] = new PLDouble (5),
            ["c = 3"]     = new PLDouble (3),
            ["c = -1"]    = new PLDouble (-1),
            ["disp (c)"]  = new PLDouble (0),
        };

        private bool supressPrinting = false;
        //private bool supressPrinting = true;
        public  bool SupressPrinting {get {return supressPrinting;} private set {supressPrinting = value;}}

        private readonly PLVariable Answer = new PLNull ();

        internal ExpressionTree (AnnotatedString astr)
        {
            if (IfBlockTests.ContainsKey (astr.Plain))
                Answer = IfBlockTests [astr.Plain];

            else if (CodeEvaluation.ContainsKey (astr.Plain))
                Answer = CodeEvaluation [astr.Plain];

            else
                throw new Exception ("ExpressionTree stub can't evaluate " + astr.Plain);
        }

        internal PLVariable Evaluate ()
        {
            return Answer;
        }
    }
}
