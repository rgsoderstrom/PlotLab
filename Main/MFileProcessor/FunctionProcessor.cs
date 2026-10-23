using System;
using System.Collections.Generic;

using Common;
using PLCommon;
using PLWorkspace;

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

            // must deal with continued lines

            // function [a, b, c] = f1 (a,  % comment
            //                          b); % comment



            // get first line. extract formal input and output parameter names
            string firstLine = fileLines [0];

            // remove the header line
            fileLines.RemoveAt (0);


            AnnotatedString astr = new AnnotatedString (firstLine);
            TokenParsing Parser = new TokenParsing ();
            TokenSet tokens = Parser.ParsingPassOne (astr);

            if (tokens.Count != 5)
                throw new Exception ("Syntax error in function file header: " + firstLine);

            if (tokens [0].Type != TokenType.Alphanumeric && tokens [0].AnnotatedText.Plain != "function")
                throw new Exception ("Syntax error in function file header: " + firstLine);

            if (tokens [1].Type != TokenType.Brackets)
                throw new Exception ("Syntax error in function file header: " + firstLine);

            if (tokens [2].Type != TokenType.EqualSign)
                throw new Exception ("Syntax error in function file header: " + firstLine);

            if (tokens [3].AnnotatedText.Plain != scriptName)
                throw new Exception ("Function name in header line must match file name: " + scriptName);

            if (tokens [4].Type != TokenType.Parens)
                throw new Exception ("Syntax error in function file header: " + firstLine);

            formalInputParameters  = Parser.SplitFunctionArgs (tokens [4].AnnotatedText);
            formalOutputParameters = Parser.SplitBracketArgs_Comma (tokens [1].AnnotatedText);
        }

        //*******************************************************

        // Run the function in the Workspace provided by the caller

        public PLList Run (List<PLVariable> actualInputParameters)
        {
            // Create a variable for each formal input parameter and
            // initialize it with the corresponding actual parameter
            for (int i=0; i<formalInputParameters.Count; i++)
            {
                PLVariable oneFunctionInput = actualInputParameters [i];
                oneFunctionInput.Name       = formalInputParameters [i];

                Workspace.Add (oneFunctionInput);
            }

            // Run the file lines like a script
            Run ();

            // Build a list of actual outputs in same order as formal outputs
            PLList actualOutputs = new PLList ();

            foreach (string str in formalOutputParameters)
            {
                PLVariable plv = Workspace.Get (str);
                actualOutputs.Add (plv);
            }

            //actualOutputs.Add (new PLDouble (123));
            //actualOutputs.Add (new PLDouble (456));

            return actualOutputs;
        }



    }
}
