using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    //****************************************************************************************
    //****************************************************************************************
    //****************************************************************************************
    //
    // Block - one of these descriptors created for each block
    //  - for -> end
    //  - while -> end
    //  - if -> end
    //

    public class Block
    {
        public enum Type {none = 0, IF, ELSE, ELSEIF, WHILE, FOR, END};

        public int  level; // nesting level
        public Type type;
        public List<Delimiter> delimiters = new List<Delimiter> (); // start & end for all "if", "for" and "while" blocks.
                                                                    // Also contain any "else" and "elseif" for "if" blocks

        //***********************************************************************************

        public class Delimiter
        {
            public int lineNumber;
            public Type type;

            public override string ToString ()
            {
                return string.Format ("({0}, {1})", lineNumber, type);
            }
        }

        //***********************************************************************************

        public Block ()
        {

        }

        public void AddDelimiter (int lineNumb, Type ty)
        {
            Delimiter del = new Delimiter () {lineNumber = lineNumb, type = ty};
            delimiters.Add (del);
        }

        public override string ToString ()
        {
            string str = "";
            str += type.ToString () + ", ";
            str += level + ", ";
            str += "[";
            foreach (Delimiter l in delimiters) str += l.ToString () + ", ";
            str += "]";
            
            return str;
        }
    }

    //***************************************************************************************
    //***************************************************************************************
    //***************************************************************************************

    public class NumberedLine 
    {
        public int Number;
        public string Text;

        public int Length {get {return ToString ().Length;}}

        public NumberedLine (int num, string txt)
        {
            Number = num;
            Text = txt;
        }

        public override string ToString ()
        {
            string str = "";
            str += Number.ToString ();
            str += ": ";
            str += Text;

            return str;
        }
    }

    //*************************************************************************************
    //*************************************************************************************

    public class NumberedScript : List<NumberedLine>
    {
        static public int firstLineNumber = 100;
        static public int lineNumberIncr = 10;

        Dictionary<int, int> lineNumberToIndex = new Dictionary<int, int> ();

        public NumberedScript (List<string> input) : base ()
        {
            int count = 0;

            foreach (string str in input)
                Add (new NumberedLine (firstLineNumber + count++ * lineNumberIncr, str));            
        }

        public NumberedScript (NumberedScript orig) : base ()
        {
            foreach (NumberedLine nl in orig)
                Add (new NumberedLine (nl.Number, nl.Text));
        }

        public NumberedScript (string fullFileName)  // DEBUG ONLY
        {
            StreamReader file = new StreamReader (fullFileName);
            string raw;

            while ((raw = file.ReadLine ()) != null)
            {
                int index = raw.IndexOf (':');
                int number = Convert.ToInt32 (raw.Substring (0, index));
                Add (new NumberedLine (number, raw.Substring (index + 2)));    
            }

            file.Close ();

            PopulateLineNumberDictionary ();
        }

        //*************************************************************************************

        public int FirstLineNumber
        {
            get
            {
                return this [0].Number;
            }
        }

        public int LastLineNumber
        {
            get
            {
                int index = Count - 1;
                return this [index].Number;
            }
        }

        //*************************************************************************************

        public void PopulateLineNumberDictionary ()
        {
            for (int index = 0; index<this.Count; index++)
            {
                int lineNumb = this [index].Number;
                lineNumberToIndex.Add (lineNumb, index);
            }
        }

        public int GetIndexFromLineNumber (int numb)
        {
            return lineNumberToIndex [numb];
            throw new Exception ("Line number not found");
        }

        public int NextLineNumber (int numb)
        {
            int index = lineNumberToIndex [numb] + 1;
            if (index < Count)
                return this [index].Number;

            return 9999;
        }

        //*************************************************************************************

        public string GetTextForIndex (int index)
        {
            return this [index].Text;
        }

        public string GetTextForLineNumber (int lineNumber)
        {
            if (lineNumberToIndex.ContainsKey (lineNumber))
            {
                int index = lineNumberToIndex [lineNumber];
                return this [index].Text;
            }

            for (int i=0; i<this.Count; i++)
            {
                if (this [i].Number == lineNumber)
                    return this [i].Text;
            }

            throw new Exception ("Line number not found");
        }

        public int GetLineNumber (int index)
        {
            return this [index].Number;
        }

        public NumberedLine GetLine (int lineNumber)
        {
            int index = lineNumberToIndex [lineNumber];
            return this [index];
        }

        //*************************************************************************************

        public int MaxLength
        {
            get
            {
                int max = 0;

                foreach (NumberedLine nl in this)
                    if (max < nl.Length)
                        max = nl.Length;

                return max;
            }
        }

        //*************************************************************************************

        public void OverwriteLine (int targetNumber, string txt)
        {
            int i;
            for (i = 0; i<Count; i++)
            {
                int thisLineNum = this [i].Number;

                if (thisLineNum == targetNumber)
                {
                    this [i].Text = txt;
                    break;
                }
            }

            if (i == Count)
                throw new Exception ("Overwrite failed");
        }

        //*************************************************************************************

        // InsertLine
        //   - Limitation: won't append a new last line

        public void InsertLine (int targetNumber, string txt)
        {
            for (int i=0; i<Count; i++)
            {
                int thisLineNum = this [i].Number;

                if (thisLineNum == targetNumber)
                {
                    Insert (i, new NumberedLine (targetNumber, txt));
                    break;
                }

                else if (i + 1 < Count)
                {
                    int nextLineNum = this [i + 1].Number;

                    if (thisLineNum < targetNumber && nextLineNum > targetNumber)
                    {
                        Insert (i + 1, new NumberedLine (targetNumber, txt));
                        break;
                    }
                }
            }
        }
    }

}
