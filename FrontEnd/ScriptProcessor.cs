using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;
using ScriptPreprocess;

namespace FrontEnd
{
    public class ScriptProcessor
    {
        PrintFunction print = null;
        Workspace workspace = null;
        Button resume = null;

        NumberedScript expanded = null; // preprocessor output

        public enum ScriptTerminationReason {Complete, Paused};

        public ScriptProcessor (Workspace ws, PrintFunction pf, Button res)
        {
            workspace = ws;
            print = pf;
            resume = res;
        }
        
        //***************************************************************************************************************

        static internal List<ScriptProcessor> PausedScripts = new List<ScriptProcessor> ();

        static internal bool ResumePausedScript ()
        {
            try
            {
                if (PausedScripts.Count > 0)
                {
                    ScriptProcessor sp = PausedScripts [PausedScripts.Count - 1];
                    PausedScripts.RemoveAt (PausedScripts.Count - 1);

                    ScriptTerminationReason reason = sp.ResumeScript ();

                    if (reason == ScriptTerminationReason.Paused)
                        PausedScripts.Add (sp);
                }
            }

            catch (Exception ex)
            {
                PausedScripts.Clear ();
                throw ex;
            }

            return (PausedScripts.Count > 0); // "true" will keep "resume" button enabled
        }

        //***************************************************************************************************************

        // passed lines to run as a script. 

        internal ScriptTerminationReason RunScriptLines (List<string> scriptLines)
        {
            try
            {
                ScriptPreprocessor spp = new ScriptPreprocessor ();
                expanded = spp.Run (scriptLines);

                int start = expanded.FirstLineNumber;
                int finish = expanded.LastLineNumber;

                return RunScriptLines (expanded, start, finish);
            }

            catch (Exception ex)
            {
                throw new Exception ("Script processor error: " + ex.Message);
            }
        }

        //***************************************************************************************************************

        // passed script name only, no path and no extension
        
        public ScriptTerminationReason FindAndRunScript (string scriptName)
        {
            string fullName = "";

            if (FileSearch.NameSearch (scriptName, ref fullName))
                return RunScript (fullName);

            else
                throw new Exception ("Script " + scriptName + " not found");
        }

        //***************************************************************************************************************
        /****
        //
        // passed in name of expanded script --- INTENDED FOR DEBUG ONLY
        //
        public void RunExpandedScript (string fullName)
        {
            try
            {
                expanded = new NumberedScript (fullName);

                int start = expanded.FirstLineNumber;
                int finish = expanded.LastLineNumber;

                RunScriptLines (expanded, start, finish);
            }

            catch (Exception ex)
            {
                throw new Exception ("Script processor error: " + ex.Message);
            }
        }
        ****/

        ScriptTerminationReason RunScript (string fullName)
        {
            try
            {
                ScriptPreprocessor spp = new ScriptPreprocessor ();
                bool writeDebugFile = false;
                expanded = spp.Run (fullName, writeDebugFile);

                int start = expanded.FirstLineNumber;
                int finish = expanded.LastLineNumber;

                return RunScriptLines (expanded, start, finish);
            }

            catch (Exception ex)
            {
                throw new Exception ("Script processor error: " + ex.Message);
            }
        }

        //***********************************************************************************

        NumberedScript PausedScript;
        int ResumeFromLineNumber = 0;
        int ResumeToLineNumber = 0;

        //***********************************************************************************

        public ScriptTerminationReason ResumeScript ()
        {           
            return RunScriptLines (PausedScript, ResumeFromLineNumber, ResumeToLineNumber);
        }

        //***********************************************************************************

        ScriptTerminationReason RunScriptLines (NumberedScript script, int from, int to)
        {
            if (from == 0 || to == 0)
                return ScriptTerminationReason.Complete;

            int lineNumber = from;

            while (lineNumber <= to)
            {
                string text = script.GetTextForLineNumber (lineNumber);

                string[] words = text.Split (new char [] { '{', '}'}, StringSplitOptions.RemoveEmptyEntries);

                switch (words [0].Trim ())
                {
                    case "return":
                        return ScriptTerminationReason.Complete;



                    case "PAUSE":
                        PausedScript = script;
                        ResumeFromLineNumber = lineNumber + NumberedScript.lineNumberIncr;
                        ResumeToLineNumber = to;
                        return ScriptTerminationReason.Paused;
                        

                    case "ASSN": // assignment
                    {
                        // get everything between outer parens
                        int index1 = text.IndexOf ('{');
                        int index2 = text.LastIndexOf ('}');
                        string ss1 = text.Substring (index1 + 1, index2 - index1 - 1);
                        string [] stmts = ss1.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        PLKernel.EntryPoint ep = new PLKernel.EntryPoint ();
                        PLVariable results = new PLNull ();

                        foreach (string str in stmts)
                            ep.ProcessArithmeticExpression (ref results, str, workspace);

                        lineNumber = script.NextLineNumber (lineNumber);
                    }
                    break;

                    case "TEST":
                    {
                        PLVariable results = new PLNull ();
                        TestParsing tp = ParseTEST (words);

                        PLKernel.EntryPoint ep = new PLKernel.EntryPoint ();
                        ep.ProcessArithmeticExpression (ref results, tp.expression, workspace);

                        //PLBool res = results as PLBool;
                        PLBool res = new PLBool (results);


                        if (res == null)
                            throw new Exception ("Test " + tp.expression + " must resolve to a bool");

                        if (res.Data == true)
                        {
                            if (tp.trueLines.Count == 3)
                            {
                                RunScriptLines (script, tp.trueLines [0], tp.trueLines [1]);
                                lineNumber = tp.trueLines [2];
                            }

                            else if (tp.trueLines.Count == 1)
                            {
                                lineNumber = tp.trueLines [0];
                            }

                            else throw new Exception ("Error parsing " + words);
                        }

                        if (res.Data == false)
                        {
                            if (tp.falseLines.Count == 3)
                            {
                                RunScriptLines (script, tp.falseLines [0], tp.falseLines [1]);
                                lineNumber = tp.falseLines [2];
                            }

                            else if (tp.falseLines.Count == 1)
                            {
                                lineNumber = tp.falseLines [0];
                            }

                            else throw new Exception ("Error parsing " + words);
                        }

                    }
                    break;

                    case "CLEAR":
                    {
                        bool unused = false;
                        string expression = "clear ";
                        for (int i = 1; i<words.Length; i++)
                            expression += words [i] + " ";

                        InputLineProcessor ip = new InputLineProcessor (workspace);
                        PLVariable ans = new PLInteger (-1);
                        ip.ProcessOneStatement (ref ans, expression, ref unused);
                        lineNumber = script.NextLineNumber (lineNumber);
                    }
                    break;

                    case "NOP":
                        lineNumber = script.NextLineNumber (lineNumber);
                        break;

                    default:
                        RunOneScriptLine (text);
                        lineNumber = script.NextLineNumber (lineNumber);
                        break;
                }
            }

            return ScriptTerminationReason.Complete;
        }

        //***********************************************************************************





        List<Utils.InputLine> inputLines = new List<Utils.InputLine> (); 
        Utils.NestingLevel nestingLevel = new Utils.NestingLevel ();





        void RunOneScriptLine (string raw)
        {
            bool unused = false;
            InputLineProcessor ip = new InputLineProcessor (workspace, print, resume);
            Utils.CleanupRawInput (raw, inputLines, ref nestingLevel);

            int startIndex = 0;
            int endIndex = -1;

            for (int i=0; i<inputLines.Count; i++)
            {
                if (inputLines [i].complete)
                {
                    endIndex = i;
                    string expr = "";

                    for (int j=startIndex; j<=endIndex; j++)
                        expr += inputLines [j].text;

                    PLVariable ans = new PLNull ();
                    ip.ProcessOneStatement (ref ans, expr, ref unused);

                    if (ans != null && ans is PLNull == false && ans is PLCanvasObject == false)
                    {
                        ans.Name = "ans";
                        workspace.Add (ans);

                        // kludge to print "disp" results
                        bool forcePrint = false;

                        if (expr.Length > 4)
                            if (expr.Substring (0, 4) == "disp")
                                forcePrint = true;

                        if (inputLines [endIndex].printFlag || forcePrint)
                        {
                            print (ans.ToString ());
                            print ("\n");
                        }
                    }

                    startIndex = endIndex + 1;
                }
            }

            if (endIndex != -1)
            {
                inputLines.RemoveRange (0, endIndex + 1);
            }


        }

        //*******************************************************************************************************
        //
        // parse TEST command
        //

        class TestParsing
        {
            public string expression;
            public List<int> trueLines = new List<int> (3);
            public List<int> falseLines = new List<int> (3);
        }

        TestParsing ParseTEST (string[] inputWords)
        {
            TestParsing tp = new TestParsing ();

            tp.expression = inputWords [1];
            string t2 = inputWords [3];
            string t3 = inputWords [5];

            string [] t2Numbers = t2.Split (new char [] { ',' });
            string [] t3Numbers = t3.Split (new char [] { ',' });

            foreach (string str in t2Numbers)
                tp.trueLines.Add (Convert.ToInt32 (str));

            foreach (string str in t3Numbers)
                tp.falseLines.Add (Convert.ToInt32 (str));

            return tp;
        }
    }
}











