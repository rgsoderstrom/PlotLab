using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using CommonMath;

namespace PLCommon
{
    //
    // Abstract base class for real and complex matrices
    //
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

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            //throw new Exception ("PLMatrix.ToString (cfmt) not implemented");

            int a = 8, b = 5; // defaults
            string str = "";

            //PLVariable p1; 
            //PLComplex  p2;
            //PLDouble   p3;

            try
            {
                List<string> cfmtParts = base.SplitFormatString (cfmt);

                if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
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
                        //p1 = Get (r, c);
                        //p2 = p1 as PLComplex;
                        //p3 = p1 as PLDouble;

                        str += string.Format (fmt, Get (r, c)) + ", ";
                    }

                    if (r < Rows - 1)
                        str += "\n";
                }
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception: " + ex.Message);
                //EventLog.WriteLine (ex.StackTrace);
            }

            return str;
        }

        //**************************************************************************

        // 

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

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************
    //
    //  Real matrix
    //
    public class PLRMatrix : PLMatrix
    {
        public CommonMath.Matrix Data;

        public PLRMatrix (int r, int c)
        {
            Data = new Matrix (r, c);
        }

        public PLRMatrix (CommonMath.Matrix src)
        {
            Data = src;
        }

        public PLRMatrix (PLDouble src)
        {
            Data = new Matrix (1, 1);
            Data [0, 0] = src.Data;
        }

        public PLRMatrix (string name, CommonMath.Matrix data)
        {
            Name = name;
            Data = data;
        }

        public List<double> ReadByColumn ()
        {
            List<double> results = new List<double> (Rows * Cols);

            for (int j = 0; j<Cols; j++)
            {
                for (int i = 0; i<Rows; i++)
                    results.Add (Data [i, j]);
            }

            return results;
        }

        public override PLMatrix CollapseToColumn ()
        {
            PLRMatrix result = new PLRMatrix (Size, 1);

            List<double> data = ReadByColumn ();

            for (int i = 0; i<Size; i++)
            {
                result.Data [i, 0] = data [i];
            }

            return result;
        }

        public override int Rows {get {return Data.Rows;}}
        public override int Cols {get {return Data.Cols;}}
        public override int Size {get {return Rows * Cols;}}

        public double this [int row, int col]  // zero-based indices
        {
            get {return Data [row, col];}
            set {Data [row, col] = value;}
        }

        public double this [int sel]  // zero-based indices
        {
            get
            {
                int col = sel / Rows;
                int row = sel % Rows;
                return Data [row, col];
            }

            set
            {
                int col = sel / Rows;
                int row = sel % Rows;
                Data [row, col] = value;
            }
        }

        public override PLVariable Get (int row, int col)
        {
            return new PLDouble (Data [row, col]);
        }

        public override void Set (int row, int col, PLVariable dbl)
        {
            if (dbl is PLDouble)
                this [row, col] = (dbl as PLDouble).Data;

            else
                throw new Exception ("Real matrix can only store real numbers");
        }

        //*********************************************************************************
        //
        // Arithmetic on real matrices
        //

        public PLRMatrix Add (PLRMatrix op2) {return new PLRMatrix ("", Data + op2.Data);}
        public PLRMatrix Add (PLDouble op2)  {return new PLRMatrix ("", Data + op2.Data);}



     // public PLCMatrix Add (PLComplex op2) {return new PLCMatrix ("", Data + op2.Data);}
        public PLCMatrix Add (PLComplex op2) 
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j] + op2.Real, op2.Imag);

            return results;
        }



        public PLRMatrix Sub (PLRMatrix op2) {return new PLRMatrix ("", Data - op2.Data);}
        public PLRMatrix Sub (PLDouble op2)  {return new PLRMatrix ("", Data - op2.Data);}
        public PLRMatrix Sub (PLInteger op2) {return new PLRMatrix ("", Data - op2.Data);}

        public PLRMatrix Mul (PLDouble op2)  {return new PLRMatrix ("", Data * op2.Data);}
        public PLRMatrix Mul (PLRMatrix op2) {return new PLRMatrix ("", Data * op2.Data);}

        public PLRMatrix Div (PLDouble op2)  {return new PLRMatrix ("", Data / op2.Data);}

        public PLRMatrix Pwr (PLDouble op2)  {return new PLRMatrix ("", Data ^ op2.Data);}
        public PLRMatrix Pwr (PLRMatrix op2) {return new PLRMatrix ("", Data ^ op2.Data);}

        public PLCMatrix Add (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j] + op2 [i, j].Real, op2 [i, j].Imag);

            return results;
        }

        public PLCMatrix Sub (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j] - op2 [i, j].Real, -op2 [i, j].Imag);

            return results;
        }

        public PLCMatrix Mul (PLCMatrix op2)
        {
            if (Cols != op2.Rows)
                throw new Exception ("Real-Complex matrix multiply size error");

            int M = Rows;
            int N = Cols;
            int P = op2.Cols;

            PLCMatrix results = new PLCMatrix (Rows, op2.Cols);

            for (int i = 0; i<M; i++)
            {
                for (int j = 0; j<P; j++)
                {
                    PLComplex acc = new PLComplex (0, 0);

                    for (int k = 0; k<N; k++)
                    {
                        acc = (acc + new PLComplex (this [i, k] * op2 [k, j].Real, this [i, k] * op2 [k, j].Imag)) as PLComplex;
                    }

                    results [i, j] = acc;
                }
            }

            return results;
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************
    //
    // Complex matrix.
    //
    public class PLCMatrix : PLMatrix
    {
        public CommonMath.CMatrix Data;


        public PLCMatrix (PLRMatrix src)
        {
            Data = new CMatrix (src.Rows, src.Cols);

            for (int i=0; i<src.Rows; i++)
            { 
                for (int j=0; j<src.Cols; j++)
                { 
                    Data [i, j].Real = src [i, j];
                    Data [i, j].Imag = 0;
                }
            }
        }

        public PLCMatrix (string name, CommonMath.CMatrix data)
        {
            Name = name;
            Data = data;
        }




        public PLCMatrix (int r, int c)
        {
            Data = new CMatrix (r, c);
        }

        public PLCMatrix (CommonMath.CMatrix src)
        {
            Data = src;
        }

        public PLCMatrix (PLComplex src)
        {
            Data = new CMatrix (1, 1);
            Data [0, 0] = new CommonMath.Complex (src.Real, src.Imag);
        }

        public PLCMatrix (string name, PLCMatrix src)
        {
            Name = name;
            Data = src.Data;
        }

        public PLComplex this [int row, int col]  // zero-based indices
        {
            get {return new PLComplex (Data [row, col]);}
            set {Data [row, col] = new CommonMath.Complex (value.Real, value.Imag);}
        }

        public PLComplex this [int sel]  // zero-based indices
        {
            get
            {
                int col = sel / Rows;
                int row = sel % Rows;
                return this [row, col];
            }

            set
            {
                int col = sel / Rows;
                int row = sel % Rows;
                this [row, col] = value;
            }
        }

        public override PLVariable Get (int row, int col)
        {
            return new PLComplex (Data [row, col]);
        }

        public override void Set (int row, int col, PLVariable cmp)
        {
            if (cmp is PLComplex)
                this [row, col] = cmp as PLComplex;

            else if (cmp is PLDouble)
                this [row, col] = new PLComplex ((cmp as PLDouble).Data, 0);

            else
                throw new Exception ("Complex matrix can not store type " + cmp.GetType ());
        }

        public override int Rows {get {return Data.Rows;}}
        public override int Cols {get {return Data.Cols;}}
        public override int Size {get {return Rows * Cols;}}

        public override PLMatrix CollapseToColumn ()
        {
            PLCMatrix result = new PLCMatrix (Size, 1);
            List<PLComplex> data = ReadByColumn ();
            for (int i = 0; i<Size; i++)
            {
                result [i, 0] = data [i];
            }
            return result;
        }

        public List<PLComplex> ReadByColumn ()
        {
            List<PLComplex> results = new List<PLComplex> (Rows * Cols);
            for (int j = 0; j<Cols; j++)
            {
                for (int i = 0; i<Rows; i++)
                    results.Add (this [i, j]);
            }
            return results;
        }

        public PLCMatrix Add (PLCMatrix op2)
        {
            return new PLCMatrix (Data + op2.Data);
        }

        public PLCMatrix Add (PLComplex op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = this [i, j].Add (op2);

            return results;
        }

        public PLCMatrix Sub (PLRMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j].Real - op2 [i, j], this [i, j].Imag);

            return results;
        }

        public PLCMatrix Sub (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j].Real - op2 [i, j].Real, this [i, j].Imag - op2 [i, j].Imag);

            return results;
        }

        public PLCMatrix Sub (PLComplex op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j].Real - op2.Real, this [i, j].Imag - op2.Imag);

            return results;
        }

        public PLCMatrix Mul (PLComplex op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
            {
                for (int j = 0; j<Cols; j++)
                {
                    results [i, j] = this [i, j] * op2 as PLComplex;
                }
            }

            return results;
        }

        public PLCMatrix Mul (PLRMatrix op2)
        {
            if (Cols != op2.Rows)
                throw new Exception ("Complex-Real matrix multiply size error");

            PLCMatrix results = new PLCMatrix (Rows, op2.Cols);

            for (int i = 0; i<Rows; i++)
            {
                for (int j = 0; j<op2.Cols; j++)
                {
                    PLComplex acc = new PLComplex (0, 0);

                    for (int k = 0; k<Cols; k++)
                        acc = acc.Add (this [i, k].Mul (op2 [k, j]));

                    results [i, j] = acc;
                }
            }

            return results;
        }

        public PLCMatrix Mul (PLCMatrix op2)
        {
            return new PLCMatrix (Data * op2.Data);
        }

        public PLCMatrix Div (PLDouble op2)
        {
            return new PLCMatrix (Data / op2.Data);
        }

        public PLCMatrix Div (PLComplex op2)
        {
            return new PLCMatrix (Data / new CommonMath.Complex (op2.Data.Real, op2.Data.Imag));
        }
    }
}
