using System;
using System.Collections.Generic;
using System.Windows.Controls;

using PLCommon;
using PLLibrary;
using PLWorkspace;
using PLKernel;

namespace FrontEnd
{
    public class InputLineProcessor
    {
        Workspace workspace;
        PrintFunction Print;
        Button ResumeButton;

        public InputLineProcessor (Workspace ws, PrintFunction pr, Button res)
        {
            workspace = ws;
            Print = pr;
            ResumeButton = res;
        }

        public InputLineProcessor (Workspace ws)
        {
            workspace = ws;
            Print = null;
            ResumeButton = null;
        }

        public void ProcessOneLine (ref PLVariable results, string text, ref bool forcePrint)
        {
            results = new PLNull ();



            TokenParsing tp = new TokenParsing ();
            List<Token> tok = tp.ParsingPassOne (text);

            if (tok.Count == 0)
                return;

            string [] words = new string [tok.Count];

            for (int i=0; i<tok.Count; i++)
                words [i] = tok [i].text;


            //// get first word, see what it is
            //string [] words = text.Split (new char [] { ' ', '(', ')', '='}, StringSplitOptions.RemoveEmptyEntries);

            //if (words.Length == 0)
            //    return;

            SymbolicNameTypes firstWordType = SystemFunctions.WhatIs (words [0]);

            if (firstWordType == SymbolicNameTypes.Unknown) 
                firstWordType = LibraryManager.WhatIs (words [0]);

            if (firstWordType == SymbolicNameTypes.Unknown) 
                firstWordType = workspace.WhatIs (words [0]);

            if (firstWordType == SymbolicNameTypes.Unknown)
                firstWordType = FileSearch.WhatIs (words [0]);

            PLString cmnd = new PLString (words [0]);
            PLList  args  = new PLList ();

            if (cmnd.Text == "disp")                 // OTHER "FORCE PRINT" CASES?
                forcePrint = true;

            //if (words.Length == 1)
            //    args.Add (new PLString (""));  

            //else
                for (int i = 1; i<words.Length; i++)
                    args.Add (new PLString (words [i]));

            switch (firstWordType)
            {
                case SymbolicNameTypes.ScriptFile:
                {
                    if (args.Count > 0)
                        throw new Exception ("Error: script name " + words [0] + " cannot be used as a variable");

                    ScriptProcessor sp = new ScriptProcessor (workspace, Print, ResumeButton);
                    ScriptProcessor.ScriptTerminationReason reason = sp.FindAndRunScript (words [0]);

                    if (reason == ScriptProcessor.ScriptTerminationReason.Paused)
                    {
                        if (ResumeButton != null) ResumeButton.IsEnabled = true;
                        ScriptProcessor.PausedScripts.Add (sp);
                        Print ("Paused, click \"Resume Script\" to continue\n");
                    }

                    else if (reason == ScriptProcessor.ScriptTerminationReason.Complete)
                    {
                        //if (Print != null)
                            //Print ("Script complete\n");
                    }

                    else
                        throw new Exception ("Unrecognized script termination reason");
                }                 
                break;

                case SymbolicNameTypes.PlotCommand:
                    results = LibraryManager.RunPlotCommand (cmnd, args);
                    break;

                case SymbolicNameTypes.WorkspaceCommand:
                    results = workspace.RunCommand (cmnd, args);
                    break;

                case SymbolicNameTypes.SystemCommand:
                    results = SystemFunctions.RunSystemCommand (cmnd, args);
                    break;

                default:
                {
                    PLKernel.EntryPoint kernel = new PLKernel.EntryPoint ();
                    kernel.ProcessArithmeticExpression (ref results, text, workspace);
                }
                break;
            }
        }
    }
}
