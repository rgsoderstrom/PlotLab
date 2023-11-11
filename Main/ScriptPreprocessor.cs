using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Main
{
    public class ScriptPreprocessor
    {
        List<string>   raw;          // as read from file, tabs expanded to spaces
        List<string>   streamlined;  // delete comments and blank lines

        NumberedScript numbered;     // line numbers added        
        NumberedScript expanded;     // 

        List<int>        nestingLevel; // one entry per numbered line
        List<Block.Type> ctrlType;     // ditto

        List<Block> blocks = new List<Block> ();

        public ScriptPreprocessor ()
        {

        }

        //*******************************************************************************
        //
        // Run - public entry point
        //
        public NumberedScript Run (string fullFilename, bool writeExpandedToFile)
        {           
            raw = ReadInputFile (fullFilename);  // read file lines

            RunFromRaw ();

            // for debug - save to text file
            if (writeExpandedToFile)
            {
                string outputFilename = fullFilename + "pp";
                StreamWriter file = new StreamWriter (outputFilename, false);

                foreach (NumberedLine nl in expanded)
                    file.WriteLine (nl);

                file.Close ();
            }

            return expanded;
        }




        public NumberedScript Run (List<string> lines)
        {
            raw = lines;
            return RunFromRaw ();
        }




        NumberedScript RunFromRaw ()
        { 
            // delete comments and blank lines
            streamlined = Cleanup (raw);

            // add line numbers
            numbered = new NumberedScript (streamlined);
            expanded = new NumberedScript (numbered); // deep copy

            // match-up if-else-end, for-end sets, etc
            IdentifyBlocks (numbered);

            int count = 0;

            while (DescribeBlocks (numbered) == true)
            {
                if (count++ > 1000) // 1000 is arbitrary
                    throw new Exception ("ScriptPreprocessor - stuck in DescribeBlocks loop");
            }

            // expand flow control statements
            TranslateBlocks (expanded);
            expanded.PopulateLineNumberDictionary ();

            return expanded;
        }

        //*******************************************************************************
        //
        // Passed full name of a script
        //    - including path and extension
        //
        List<string> ReadInputFile (string fullName)
        {
            try
            {
                List<string> scriptLines = new List<string> ();
                StreamReader file = new StreamReader (fullName);
                string raw;

                while ((raw = file.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        int index, count = 0;

                        while ((index = raw.IndexOf ('\t')) >= 0) // replace tabs with spaces
                        {
                            raw = raw.Remove (index, 1);
                            raw = raw.Insert (index, "    ");

                            if (count++ > 100) throw new Exception ("Error removing tabs");
                        }

                        scriptLines.Add (raw);
                    }
                }

                file.Close ();
                return scriptLines;
            }

            catch (Exception ex)
            {
                throw new Exception ("Error reading script file: " + ex.Message);
            }
        }

        //*******************************************************************************
        //
        // remove comments, blank line and leading/trailing blanks
        //
        //    LIMITATION: assumes at most one quoted string per line
        //
        List<string> Cleanup (List<string> input)
        {
            List<string> output = new List<string> ();

            foreach (string str in input)
            {
                string ostr = (string) str.Clone ();

                int index1 = ostr.IndexOf ('\'');
                int index2 = ostr.LastIndexOf ('\'');  
                int index3 = ostr.IndexOf ('%');     
                
                if (index3 >= 0)
                    if (index3 < index1 || index3 > index2) 
                        ostr = ostr.Remove (index3); // a % outside of single quotes is a comment

               // ostr = ostr.TrimEnd ();
                ostr = ostr.Trim ();

                if (ostr.Length > 0) output.Add (ostr);
            }

            return output;
        }

        //*******************************************************************************
        //
        // Identify block statements and their nesting level
        //  - if, while, etc.
        //
        bool IdentifyBlocks (NumberedScript input)
        {
            nestingLevel = new List<int> (input.Count);
            ctrlType = new List<Block.Type> (input.Count);

            int nesting = 0; // current nesting level

            foreach (NumberedLine str in input)
            {
                string [] words = str.Text.Split (new char [] {' ', '\t', '('}, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length < 1)
                    throw new Exception ("Error parsing script line: " + str);

                switch (words [0])
                {
                    case "if":
                        nestingLevel.Add (++nesting);
                        ctrlType.Add (Block.Type.IF);
                        break;

                    case "elseif":
                        nestingLevel.Add (nesting);
                        ctrlType.Add (Block.Type.ELSEIF);
                        break;

                    case "else":
                        nestingLevel.Add (nesting);
                        ctrlType.Add (Block.Type.ELSE);
                        break;

                    case "while":
                        nestingLevel.Add (++nesting);
                        ctrlType.Add (Block.Type.WHILE);
                        break;

                    case "for":
                        nestingLevel.Add (++nesting);
                        ctrlType.Add (Block.Type.FOR);
                        break;

                    case "end":
                        nestingLevel.Add (nesting--);
                        if (nesting < 0) throw new Exception ("Nesting error, unmatched \"end\"");
                        ctrlType.Add (Block.Type.END);
                        break;

                    default:
                        nestingLevel.Add (nesting);
                        ctrlType.Add (Block.Type.none);
                        break;
                }
            }

            return nestingLevel.Contains (1);
        }

        //********************************************************************************************************
        //
        // DescribeBlocks
        //    - may need to be called repeatedly, each invocation adds at most one block descriptor
        //    - returns true if should be called again
        //
        bool DescribeBlocks (NumberedScript input)
        {
            int startLine = -1;
            int nl = 0;

            //
            // look for an increase in nesting level
            //
            for (int i=0; i<input.Count; i++)
            { 
                if (nestingLevel [i] > 0)
                {
                    nl = nestingLevel [i];
                    startLine = i;
                    break;
                }
            }

            if (startLine == -1) // reached end of script w/o finding any
                return false;

            //
            // create a decriptor, then find the corresponding "end" statement
            //
            Block block = new Block {level = nl, type = ctrlType [startLine]};
            block.AddDelimiter (input [startLine].Number, block.type);

            for (int i=startLine + 1; i<input.Count; i++)
            {
                if (nestingLevel [i] == nl)
                {
                    if (ctrlType [startLine] == Block.Type.IF)
                    {
                        if (ctrlType [i] == Block.Type.ELSE || ctrlType [i] == Block.Type.ELSEIF)
                            block.AddDelimiter (input [i].Number, ctrlType [i]);
                    }

                    if (ctrlType [i] == Block.Type.END)
                    {
                        block.AddDelimiter (input [i].Number, ctrlType [i]);
                        blocks.Add (block);

                        for (int j = startLine; j<=i; j++)
                            if (nestingLevel [j] == nl)
                                nestingLevel [j] = 0;                    
                        break;
                    }
                }
            }

            return true;
        }

        //********************************************************************************************************

        void TranslateBlocks (NumberedScript input)
        {
            int incr = NumberedScript.lineNumberIncr; // a compact alias

            foreach (Block block in blocks)
            {
                for (int i=0; i<block.delimiters.Count; i++)
                {                    
                    string line = input.GetTextForLineNumber (block.delimiters [i].lineNumber);
                    int j = i + 1;
                    int last = block.delimiters.Count - 1;

                    switch (block.delimiters [i].type)
                    {
                        case Block.Type.IF:
                        case Block.Type.ELSEIF:
                        {
                            // test expression runs from right after "if" to final terminator
                            int index1 = i == 0 ? (line.IndexOf ("if") + 3) : (line.IndexOf ("elseif") + 7);
                            int index2 = line.Length - 1;                            
                            if (line [index2] == ',') index2--;

                            // the expression evaluated to decide the "if"
                            string testExpression = line.Substring (index1, index2 - index1 + 1);
                            string testExprTrue  = string.Format ("{{{0}, {1}, {2}}}", block.delimiters [i].lineNumber + incr, block.delimiters [j].lineNumber - incr, block.delimiters [last].lineNumber);
                            string testExprFalse = string.Format ("{{{0}}}", block.delimiters [j].lineNumber);

                            string str = string.Format ("TEST {{({0})}} ? {1} : {2}", testExpression, testExprTrue, testExprFalse);
                            input.OverwriteLine (block.delimiters [i].lineNumber, str);
                        }
                        break;


                        case Block.Type.WHILE:
                        {
                            // test expression runs from right after "while" to final terminator
                            int index1 = line.IndexOf ("while") + 6;
                            int index2 = line.Length - 1;

                            if (line [index2] == ',')
                                index2--;

                            string testExpression = line.Substring (index1, index2 - index1 + 1);
                            string testExprTrue  = string.Format ("{{{0}, {1}, {2}}}", block.delimiters [i].lineNumber + incr, block.delimiters [j].lineNumber - incr, block.delimiters [i].lineNumber);
                            string testExprFalse = string.Format ("{{{0}}}", block.delimiters [j].lineNumber);

                            string str = string.Format ("TEST {{{0}}} ? {1} : {2}", testExpression, testExprTrue, testExprFalse);
                            input.OverwriteLine (block.delimiters [i].lineNumber, str);
                        }
                        break;

                        case Block.Type.FOR:
                        {
                            int index1 = line.IndexOf ("for") + 4;
                            int index2 = line.IndexOf ("=");

                            string loopVariable = line.Substring (index1, index2 - index1).Trim ();

                            string values = line.Substring (index2 + 1);
                            if (values [values.Length - 1] == ',')
                                values = values.Remove (values.Length - 1);

                            string assnString1 = string.Format ("ASSN {{{0}vals = ({1}); {0}sel = 0;}}", loopVariable, values);

                            string assnString2 = string.Format ("ASSN {{{0}sel = {0}sel + 1;}}", loopVariable);

                            string testString = string.Format ("TEST {{{0}sel <= length({0}vals)}} ? {{{1}, {2}, {3}}} : {{{4}}}", loopVariable, 
                                                                                                         block.delimiters [0].lineNumber + 3, 
                                                                                                         block.delimiters [1].lineNumber - incr, 
                                                                                                         block.delimiters [0].lineNumber + 1,
                                                                                                         block.delimiters [1].lineNumber);

                            string assnString3 = string.Format ("ASSN  {{{0} = {0}vals ({0}sel)}}", loopVariable);

                            string clearString = string.Format ("CLEAR {{{0}vals {0}sel}}", loopVariable);

                            input.OverwriteLine (block.delimiters [0].lineNumber,     assnString1);
                            input.InsertLine    (block.delimiters [0].lineNumber + 1, assnString2);
                            input.InsertLine    (block.delimiters [0].lineNumber + 2, testString);
                            input.InsertLine    (block.delimiters [0].lineNumber + 3, assnString3);
                            input.OverwriteLine (block.delimiters [1].lineNumber,     clearString);
                            block.delimiters [1].type = Block.Type.none;

                        }
                        break;

                        case Block.Type.ELSE:
                        case Block.Type.END:
                            input.OverwriteLine (block.delimiters [i].lineNumber, "NOP");
                            break;

                    }
                }
            }
        }
    }
}







