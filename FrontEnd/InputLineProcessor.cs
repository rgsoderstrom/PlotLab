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

        /// <summary>
        /// Process a single statement. Input lines like:
        /// a = 2; b = 3;
        /// must be broken up into two calls to this function
        /// </summary>
        /// <param name="results"></param>
        /// <param name="text">a single PlotLab statement</param>
        /// <param name="forcePrint">Output, tells caller to print results even if statement end in semicolon</param>
        
        public void ProcessOneStatement (ref PLVariable results, string text, ref bool forcePrint)
        {
            text = text.Trim ();

            //
            // isolate the first word and try to figure out what to do with it
            //
            string firstWord;    // characters up to the first space
            PLString arguments;  // all characters after the first space

            int i = text.IndexOf (' ');

            if (i == -1) // no space found
            {
                firstWord = text;
                arguments = new PLString ("");
            }

            else
            {
                firstWord = text.Substring (0, i);
                arguments = new PLString (text.Substring (i + 1));
            }

            // forcePrint overrides the usual convention of not printing results
            // of statements that end with a semicolon
            if (firstWord == "disp")                 // OTHER "FORCE PRINT" CASES?
            {
                forcePrint = true;
            }

            //
            // Test for system command (e.g. cd, ls, path)
            //
            if (SystemFunctions.WhatIs (firstWord) == SymbolicNameTypes.SystemCommand)
            {
                results = SystemFunctions.RunSystemCommand (new PLString (firstWord), arguments);
                return;
            }

            //
            // Search the path to see if it's the name of a script file
            //
            if (FileSearch.WhatIs (firstWord) == SymbolicNameTypes.ScriptFile)
            {
                ScriptProcessor sp = new ScriptProcessor (workspace, Print, ResumeButton);
                ScriptProcessor.ScriptTerminationReason reason = sp.FindAndRunScript (firstWord);

                if (reason == ScriptProcessor.ScriptTerminationReason.Paused)
                {
                    if (ResumeButton != null) ResumeButton.IsEnabled = true;
                    ScriptProcessor.PausedScripts.Add (sp);
                    Print ("Paused, click \"Resume Script\" to continue\n");
                }

                else if (reason != ScriptProcessor.ScriptTerminationReason.Complete)
                    throw new Exception ("Script error");

                return;
            }

            //
            // Parse remaining words in the input text. This is a remnant of an earlier version
            // and will eventually moved down to the RunCOmmand () functions
            //
            TokenParsing tp = new TokenParsing ();
            List<Token> tok = tp.ParsingPassOne (text);

            if (tok.Count == 0)
                return;

            PLList  args  = new PLList ();
            for (i = 1; i<tok.Count; i++)
                args.Add (new PLString (tok [i].text));
            
            //
            // PlotCommand act on figures, they don't plat any data
            //
            if (LibraryManager.WhatIs (firstWord) == SymbolicNameTypes.PlotCommand)
            {
                results = LibraryManager.RunPlotCommand (new PLString (firstWord), args);
                return;
            }

            //
            // Workspace commands return information on things in the
            // workspace, e.g. length (a)
            // 
            if (workspace.WhatIs (firstWord) == SymbolicNameTypes.WorkspaceCommand)
            {
                results = workspace.RunCommand (new PLString (firstWord), args);
                return;
            }

            //
            // Most arithmetic, plotting and function statements will come here
            //
            PLKernel.EntryPoint kernel = new PLKernel.EntryPoint ();
            kernel.ProcessArithmeticExpression (ref results, text, workspace);
        }
    }
}
