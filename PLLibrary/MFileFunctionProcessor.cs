using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLLibrary
{
    public class MFileFunctionProcessor
    {
        public string functionName;
        public string functionPathAndName;

        public List<string> InputFormalParams  = new List<string> ();
        public List<string> ExecutableScript   = new List<string> ();
        public List<string> OutputFormalParams = new List<string> ();

        enum ParseState {LookingForStart, InExecutable};
        ParseState ps = ParseState.LookingForStart;

        public MFileFunctionProcessor (string funcName, string fullName)
        {
            functionName = funcName;
            functionPathAndName = fullName;

            //*****************************************************************************
            //
            // Parse m-file - serparate into:
            //      - input formal parameters
            //      - executable script
            //      - output format parameters
            //
  
            using (StreamReader reader = new StreamReader (fullName))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string [] tokens = line.Split (new char [] { ' ', ',', '[', ']', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens [0][0] == '%')
                            continue;

                        if (ps == ParseState.LookingForStart)
                        {
                            if (tokens [0] == "function")
                            {
                                // find token that is just an "equal" sign
                                // all before it are output formal parameters
                                // one immediately after is function name
                                // all after that are input parameter names

                                int equalIndex = -1;

                                for (int i = 0; i<tokens.Length; i++) { if (tokens [i] == "=") {equalIndex = i; break; } }

                                if (equalIndex == -1) // not found
                                    throw new Exception ("Function syntax error, equal sign not found in " + funcName);

                                if (tokens [equalIndex + 1].ToLower () != funcName.ToLower ()) // ignore case
                                    throw new Exception ("Function name doesn't match file name in file " + funcName);

                                for (int i = 1; i<equalIndex; i++)
                                    OutputFormalParams.Add (tokens [i]);

                                for (int i = equalIndex + 2; i<tokens.Length; i++)
                                    InputFormalParams.Add (tokens [i]);

                                ps = ParseState.InExecutable;
                            }
                        }

                        else if (ps == ParseState.InExecutable)
                        {
                            ExecutableScript.Add (line);
                        }
                    }
                }
            }
        }
    }
}
