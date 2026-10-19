using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using PLCommon;

namespace PLMain
{
    internal class FunctionProcessor : MFileProcessor
    {
        // these are read from the .m file
        // [formal output parameters] = functionName (formal input parameters)
        private List<string> formalInputParameters = new List<string> ();
        private List<string> formalOutputParameters = new List<string> ();

        public FunctionProcessor (string scriptName) : base (scriptName)
        {
            // get first line. extract formal input and output parameters

            EventLog.WriteLine ("M-File function");
        }

        //*******************************************************

        public PLList Run (PLList actualParameters)
        {
            // Create a variable for each formal input parameter and
            // initialize it with the corresponding actual parameters



            PLList actualOutputs = new PLList ();


            return actualOutputs;
        }



    }
}
