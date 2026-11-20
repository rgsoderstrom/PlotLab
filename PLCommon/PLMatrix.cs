using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using CommonMath;

//
// Abstract base class for real and complex matrices
//

namespace PLCommon
{
    public abstract class PLMatrix : PLVariable
    {
        public override bool IsRowVector {get {return Rows == 1 && Cols > 1;}}
        public override bool IsColVector {get {return Cols == 1 && Rows > 1;}}
        public override bool IsVector    {get {return IsRowVector || IsColVector;}}
        public override bool IsMatrix    {get {return true;}}

        // C++ style setter & getter simplify code that handles both real and complex matrices
        public abstract PLVariable Get (int r, int c);
        public abstract void       Set (int r, int c, PLVariable plv);
        public abstract PLMatrix   CollapseToColumn ();

        //*************************************************************************************


        //


        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string formatString)
        {
            int a = 8, b = 5; // defaults
            string str = "";

            try
            {
                List<string> cfmtParts = base.SplitFormatString (formatString);

                if (cfmtParts.Count == 3)
                {
                    a = int.Parse (cfmtParts [0]);
                    b = int.Parse (cfmtParts [1]);
                }

                string fmt = "{0," + a + ":0.";
                for (int i = 0; i<b; i++) fmt += '#';
                fmt += "}";

                for (int r = 0; r<Rows; r++)
                {
                    for (int c = 0; c<Cols; c++)
                    {
                        str += string.Format (fmt, Get (r, c)) + ", ";
                    }

                    if (r < Rows - 1)
                        str += "\n";
                }
            }

            catch (Exception ex)
            {
                if (Name.Length > 0) EventLog.WriteLine ("Error in matrix " + Name + " ToString (): " + ex.Message);
                else                 EventLog.WriteLine ("Error in matrix ToString (): " + ex.Message);
            }

            return str;
        }

        //**************************************************************************

        public override string ToString ()
        {
            string str = Rows > 1 ? "\n" : " ";

            try
            { 
                // make a parallel matrix of strings, one for each entry of "this" matrix
                string [,] textForNumbers = new string [Rows, Cols];

                if (this is PLCMatrix)
                { 
                    for (int i=0; i<Rows; i++)
                    {
                        for (int j=0; j<Cols; j++)
                        {
                            PLComplex cplex = (Get (i, j) as PLComplex);
                            textForNumbers [i, j] = cplex.ToString ();
                        }
                    }
                }

                else if (this is PLRMatrix)
                { 
                    for (int i=0; i<Rows; i++)
                    {
                        for (int j=0; j<Cols; j++)
                        {
                            double dbl = (Get (i, j) as PLDouble).Data;
                            textForNumbers [i, j] = DoubleToString (dbl);
                        }
                    }
                }

                else
                    throw new Exception ("Unsupported matrix type");

                // find the widest in each column
                List<int> columnWidth = new List<int> (Cols);

                for (int j=0; j<Cols; j++)
                    columnWidth.Add (textForNumbers [0, j].Length); 
            
                for (int i=1; i<Rows; i++)
                {
                    for (int j=0; j<Cols; j++)
                        if (columnWidth [j] < textForNumbers [i, j].Length) 
                            columnWidth [j] = textForNumbers [i, j].Length;
                }

                // pad all textForNumbers strings in a column to same width
                for (int i=0; i<Rows; i++)
                {
                    for (int j=0; j<Cols; j++)
                    {
                        if (columnWidth [j] > textForNumbers [i, j].Length)
                        {
                            int padLength = columnWidth [j] - textForNumbers [i, j].Length;
                            string padString = new string (' ', padLength);  
                            textForNumbers [i, j] = padString + textForNumbers [i, j];
                        }
                    }
                }

                //*****************************************************

                // add array delimiters

                // add left bracket before text (0,0) 
                textForNumbers [0, 0] = "[" + textForNumbers [0, 0];

                // pad all column 0 entries below it with one more space
                for (int i = 1; i<Rows; i++)
                    textForNumbers [i, 0] = " " + textForNumbers [i, 0];

                // add right bracket after last entry in last row
                textForNumbers [Rows-1, Cols-1] = textForNumbers [Rows-1, Cols-1] + "]";

                //*****************************************************

                // copy all that to results string
                for (int i=0; i<Rows; i++)
                {
                    for (int j=0; j<Cols; j++)
                    {
                        str += textForNumbers [i, j];
                        str += "  ";
                    }

                    if (i != Rows-1)
                        str += "\n";
                }
            }

            catch (Exception ex)
            {
                throw new Exception ("Exception in matrix ToString (): " + ex.Message);
            }

            return str;        
        }
    }
}
