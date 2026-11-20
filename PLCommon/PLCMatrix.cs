using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonMath;

//
// Complex matrix.
//

namespace PLCommon
{
    public class PLCMatrix : PLMatrix
    {
        public CMatrix Data;  // from CommonMath


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
