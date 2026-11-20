using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonMath;

//
//  Real matrix
//

namespace PLCommon
{
    public class PLRMatrix : PLMatrix
    {
        public Matrix Data; // from CommonMath

        public PLRMatrix (int r, int c)
        {
            Data = new Matrix (r, c);
        }

        public PLRMatrix (Matrix src)
        {
            Data = src;
        }

        public PLRMatrix (PLDouble src)
        {
            Data = new Matrix (1, 1);
            Data [0, 0] = src.Data;
        }

        public PLRMatrix (string name, Matrix data)
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



        //public PLCMatrix Add (PLRMatrix op2)
        //{
        //    PLCMatrix results = new PLCMatrix (Rows, Cols);

        //    for (int i = 0; i<Rows; i++)
        //        for (int j = 0; j<Cols; j++)
        //            results [i, j] = new PLComplex (this [i, j] + op2 [i, j].Real, op2 [i, j].Imag);

        //    return results;
        //}




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
}
