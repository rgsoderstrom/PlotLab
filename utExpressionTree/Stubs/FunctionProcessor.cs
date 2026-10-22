using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    internal class FunctionProcessor
    {
        static readonly PLList f1Output = new PLList () {new PLDouble (111)};
        static readonly PLList f2Output = new PLList () {new PLDouble (11), new PLDouble (22)};

        readonly PLList nextOutput = f2Output;

        internal FunctionProcessor (string functionName)
        {
            switch (functionName.ToLower ())
            {
                case "f1": nextOutput = f1Output; break;
                case "f2": nextOutput = f2Output; break;

                default: throw new Exception ("Function " + functionName + " not supported by FunctionProcessor stub");
            }
        }
            
        internal PLList Run (List<PLVariable> plst)
        {
            return nextOutput; 
        }
    }
}
