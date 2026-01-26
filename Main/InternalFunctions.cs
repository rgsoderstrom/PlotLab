using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;

using PLCommon;

//
// Internal Functions - not directly invoked by user
//

namespace Main
{
    static public class InternalFunctions
    {
        //*********************************************************************************************

        // Transpose 
        //    - implements ' operator

        static internal PLRMatrix Transpose (PLRMatrix arg)
        {
            PLRMatrix result = new PLRMatrix (arg.Cols, arg.Rows);

            for (int i = 0; i<arg.Rows; i++)
                for (int j = 0; j<arg.Cols; j++)
                    result [j, i] = arg [i, j];

            return result;
        }

        //*********************************************************************************************

        // element-by-element multiply

        static internal PLVariable DotTimes (PLVariable arg1, PLVariable arg2)
        {
            if (arg1.IsMatrix == false || arg2.IsMatrix == false)
            {
                return arg1 * arg2;
            }

            else
            { 
                PLMatrix m1 = arg1 as PLMatrix;
                PLMatrix m2 = arg2 as PLMatrix;

                if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
                    throw new Exception ("Argument size mis-match in dot-times");

                if (arg1 is PLRMatrix && arg2 is PLRMatrix) return _DotTimes (arg1 as PLRMatrix, arg2 as PLRMatrix);
                if (arg1 is PLRMatrix && arg2 is PLCMatrix) return _DotTimes (arg1 as PLRMatrix, arg2 as PLCMatrix);
                if (arg1 is PLCMatrix && arg2 is PLRMatrix) return _DotTimes (arg2 as PLRMatrix, arg1 as PLCMatrix);
                if (arg1 is PLCMatrix && arg2 is PLCMatrix) return _DotTimes (arg1 as PLCMatrix, arg2 as PLCMatrix);

                throw new Exception ("Dot-times argument error");
            }
        }

        static PLRMatrix _DotTimes (PLRMatrix m1, PLRMatrix m2)
        {
            PLRMatrix result = new PLRMatrix (m1.Rows, m1.Cols);
            for (int i = 0; i<result.Rows; i++) for (int j = 0; j<result.Cols; j++) result [i, j] = m1 [i, j] * m2 [i, j];
            return result;
        }

        static PLCMatrix _DotTimes (PLRMatrix m1, PLCMatrix m2)
        {
            PLCMatrix result = new PLCMatrix (m1.Rows, m1.Cols);
            for (int i = 0; i<result.Rows; i++) for (int j = 0; j<result.Cols; j++) result [i, j] = m2 [i, j].Mul (m1 [i, j]);
            return result;
        }

        static PLCMatrix _DotTimes (PLCMatrix m1, PLCMatrix m2)
        {
            PLCMatrix result = new PLCMatrix (m1.Rows, m1.Cols);
            for (int i = 0; i<result.Rows; i++) for (int j = 0; j<result.Cols; j++) result [i, j] = m1 [i, j].Mul (m2 [i, j]);
            return result;
        }

        //*********************************************************************************************

        // element-by-element divide

        static internal PLRMatrix DotDivide (PLVariable arg1, PLVariable arg2)
        {
            PLRMatrix m1 = arg1 as PLRMatrix;
            PLRMatrix m2 = arg2 as PLRMatrix;

            if (m1 != null && m2 != null)
            {
                if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
                    throw new Exception ("Argument size mis-match");

                PLRMatrix result = new PLRMatrix (m1.Rows, m1.Cols);

                for (int i = 0; i<result.Rows; i++)
                    for (int j = 0; j<result.Cols; j++)
                        result [i, j] = m1 [i, j] / m2 [i, j];

                return result;
            }

            throw new Exception ("Dot-divide requres two matrices");
        }

        //*********************************************************************************************
        
        // See comments for RowVector (PLList elem)

        static internal PLMatrix ColVector (PLList elements)
        {
            if (elements.Count == 0)
                throw new Exception ("ColVector - empty inputs list");

            // make sure all elements have the same number of cols
            int cols = elements [0].Cols;

            for (int i = 1; i<elements.Count; i++)
                if (elements [i].Cols != cols)
                    throw new Exception ("ColVector - all elements must have same number cols");

            // "results" row count = sum of number of all inputs rows
            int rows = 0;

            for (int i = 0; i<elements.Count; i++)
                rows += elements [i].Rows;

            PLMatrix results;

            if (elements [0] is PLComplex || elements [0] is PLCMatrix)
                results = new PLCMatrix (rows, cols);
            else
                results = new PLRMatrix (rows, cols);


            for (int col = 0; col<cols; col++)
            {
                int put = 0;

                foreach (PLVariable var in elements)
                {
                    PLMatrix mat;

                    if (var is PLDouble)
                        mat = new PLRMatrix (var as PLDouble);

                    else if (var is PLComplex)
                        mat = new PLCMatrix (var as PLComplex);

                    else if (var is PLMatrix)
                        mat = var as PLMatrix;

                    else
                        throw new Exception ("Invalid operand: " + var.GetType () + " not supported");

                    for (int j = 0; j<mat.Rows; j++)
                    {
                        PLVariable v = mat.Get (j, col);

                        if (results is PLRMatrix && v is PLComplex)
                            results = new PLCMatrix (results as PLRMatrix);

                        results.Set (put++, col, v);
                    }
                }
            }

            return results;
        }

        //*********************************************************************************************
        //
        // RowVector - make a row vector from a list of elements
        //           - an element can be
        //              - a scalar
        //              - a column vector
        //              - a row vector
        //              - a matrix
        //              with the limitation that all have the same number of rows. The elements
        //              are concatenated in consecutive columns.
        //
        //              Note that this means the "RowVector" returned may be a matrix.
        //

        static internal PLMatrix RowVector (PLList elements)
        {
            if (elements.Count == 0)
                throw new Exception ("RowVector - empty inputs list");

            // make sure all elements have the same number of rows
            int rows = elements [0].Rows;

            for (int i = 1; i<elements.Count; i++)
                if (elements [i].Rows != rows)
                    throw new Exception ("RowVector - all elements must have same number rows");

            // "results" column count = sum of number of all inputs columns
            int cols = 0;

            for (int i = 0; i<elements.Count; i++)
                cols += elements [i].Cols;

            PLMatrix results;

            if (elements [0] is PLComplex || elements [0] is PLCMatrix)
                results = new PLCMatrix (rows, cols);
            else
                results = new PLRMatrix (rows, cols);

            for (int row=0; row<rows; row++)
            {
                int put = 0;

                foreach (PLVariable var in elements)
                {
                   PLMatrix mat;

                    if (var is PLDouble)
                        mat = new PLRMatrix (var as PLDouble);

                    else if (var is PLComplex)
                        mat = new PLCMatrix (var as PLComplex);

                    else if (var is PLMatrix)
                        mat = var as PLMatrix;

                    else
                        throw new Exception ("Invalid operand: " + var.GetType () + " not supported");

                    for (int j = 0; j<mat.Cols; j++)
                    {
                        PLVariable v = mat.Get (row, j);

                        if (results is PLRMatrix && v is PLComplex)
                            results = new PLCMatrix (results as PLRMatrix);

                        results.Set (row, put++, v);
                    }
                }
            }

            return results;
        }

        //*********************************************************************************************

        static internal PLRMatrix RowVector (double start, double step, double stop)
        {
            if (step == 0)
                throw new Exception ("Vector Expand: Step size cannot be 0");

            // verify a positive number of iterations will get us from start to stop
            double count = (stop - start) / step;

            if (count < 0)
                throw new Exception (string.Format ("Vector Expand arg error: [{0} : {1} : {2}]", start, step, stop));

            if (count > 10000) // arbitrary upper bound on vector length
                throw new Exception ("Expanded vector too long");

            // reasonable if we get here
            List<double> numbs = new List<double> ();

            int i = 0;

            while (true)
            {
                double val = start + i++ * step;

                bool between = (Math.Sign (stop - val) == Math.Sign (stop - start));
                bool equal = (val == stop);

                if (between || equal)
                    numbs.Add (val);
                else
                    break;
            }

            PLRMatrix results = new PLRMatrix (1, numbs.Count);
            results.Data.FillByRow (numbs.ToArray ());
            return results;
        }
    }
}
