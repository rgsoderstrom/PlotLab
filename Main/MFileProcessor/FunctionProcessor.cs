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
        //*******************************************************

        // these are read from the .m file
        // function [formal output parameters] = functionName (formal input parameters)

        private List<string> formalInputParameters = new List<string> ();
        private List<string> formalOutputParameters = new List<string> ();

        //*******************************************************

        public FunctionProcessor (string scriptName) : base (scriptName)
        {
            EventLog.WriteLine ("M-File function " + scriptName);

            // get first line. extract formal input and output parameter names
            string firstLine = fileLines [0];


            AnnotatedString astr = new AnnotatedString (firstLine);
            TokenParsing Parser = new TokenParsing ();
            TokenSet tokens = Parser.StringToTokens (astr);

            TokenPair tp = tokens [3] as TokenPair;
            formalInputParameters  = Parser.SplitFunctionArgs (tp.Get2.AnnotatedText);
            formalOutputParameters = Parser.SplitBracketArgs_Comma (tokens [1].AnnotatedText);

            EventLog.WriteLine ("first: " + firstLine);
        }

        //*******************************************************

        public PLList Run (List<PLVariable> actualParameters)
        {
            // Create a variable for each formal input parameter and
            // initialize it with the corresponding actual parameter



            // Run the file lines like a script



            // Build a list of actual outputs in same order as formal outputs

            PLList actualOutputs = new PLList ();

            actualOutputs.Add (new PLDouble (123));
            actualOutputs.Add (new PLDouble (456));

            return actualOutputs;
        }



    }
}
