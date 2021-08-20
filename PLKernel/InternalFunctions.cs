using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

//
// Internal Functions - not directly invoked by user
//

namespace PLKernel
{
    static public class InternalFunctions
    {
        //*********************************************************************************************

        // Transpose 
        //    - implements ' operator

        static internal PLMatrix Transpose (PLMatrix arg)
        {
            PLMatrix result = new PLMatrix (arg.Cols, arg.Rows);

            for (int i = 0; i<arg.Rows; i++)
                for (int j = 0; j<arg.Cols; j++)
                    result [j, i] = arg [i, j];

            return result;
        }

        //*********************************************************************************************

        // element-by-element multiply

        static internal PLMatrix DotTimes (PLVariable arg1, PLVariable arg2)
        {
            PLMatrix m1 = arg1 as PLMatrix;
            PLMatrix m2 = arg2 as PLMatrix;

            if (m1 != null && m2 != null)
            {
                if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
                    throw new Exception ("Argument size mis-match");

                PLMatrix result = new PLMatrix (m1.Rows, m1.Cols);

                for (int i = 0; i<result.Rows; i++)
                    for (int j = 0; j<result.Cols; j++)
                        result [i, j] = m1 [i, j] * m2 [i, j];

                return result;
            }

            throw new Exception ("Dot-times requres two matrices");
        }

        //*********************************************************************************************

        // element-by-element divide

        static internal PLMatrix DotDivide (PLVariable arg1, PLVariable arg2)
        {
            PLMatrix m1 = arg1 as PLMatrix;
            PLMatrix m2 = arg2 as PLMatrix;

            if (m1 != null && m2 != null)
            {
                if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
                    throw new Exception ("Argument size mis-match");

                PLMatrix result = new PLMatrix (m1.Rows, m1.Cols);

                for (int i = 0; i<result.Rows; i++)
                    for (int j = 0; j<result.Cols; j++)
                        result [i, j] = m1 [i, j] / m2 [i, j];

                return result;
            }

            throw new Exception ("Dot-divide requres two matrices");
        }

        //*********************************************************************************************

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

            PLMatrix results = new PLMatrix (rows, cols);

            for (int col = 0; col<cols; col++)
            {
                int put = 0;

                foreach (PLVariable var in elements)
                {
                    PLMatrix mat;

                    if (var is PLDouble)
                        mat = new PLMatrix (var as PLDouble);

                    else if (var is PLMatrix)
                        mat = var as PLMatrix;

                    else
                        throw new Exception ("Invalid operand");

                    for (int j = 0; j<mat.Rows; j++)
                    {
                        results [put++, col] = mat [j, col];
                    }
                }
            }

            return results;
        }

        //*********************************************************************************************

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

            PLMatrix results = new PLMatrix (rows, cols);

            for (int row=0; row<rows; row++)
            {
                int put = 0;

                foreach (PLVariable var in elements)
                {
                    PLMatrix mat;

                    if (var is PLDouble)
                        mat = new PLMatrix (var as PLDouble);

                    else if (var is PLMatrix)
                        mat = var as PLMatrix;

                    else
                        throw new Exception ("Invalid operand");

                    for (int j=0; j<mat.Cols; j++)
                    {
                        results [row, put++] = mat [row, j];
                    }
                }
            }

            return results;
        }

        //*********************************************************************************************

        static internal PLMatrix RowVector (double start, double step, double stop)
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

            PLMatrix results = new PLMatrix (1, numbs.Count);
            results.Data.FillByRow (numbs.ToArray ());
            return results;
        }
    }
}
