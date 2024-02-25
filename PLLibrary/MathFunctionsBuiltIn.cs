using System;
using System.Collections.Generic;

using PLCommon;

//
// Built In Functions - functions called by user code
//      - trig functions
//      - log, exp
//      - roots
//

namespace FunctionLibrary
{
    static public partial class MathFunctions
    {
        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static public Dictionary<string, PLFunction> GetBuiltInContents ()
        {
            return new Dictionary<string, PLFunction>
            {
                {"linspace", Linspace},
                {"zeros", Zeros},
                {"ones", Ones},
                {"ceil", Ceil},
                {"square", Square},
                {"sin",  Sin},
                {"cos",  Cos},
                {"tan",  Tan},
                {"sinh", Sinh},
                {"cosh", Cosh},
                {"tanh", Tanh},
                {"abs",  Abs},
                {"sqrt", Sqrt},
                {"inv",  Inverse},
                {"atan2", Atan2},
                {"rand", Rand},
                {"Transpose", Transpose},
                {"log",   Log},
                {"log2",  Log2},
                {"log10", Log10},
                {"exp",   Exp},
                {"max",   Max},
                {"min",   Min},
                {"cross", CrossProduct},
                {"real",  Real},
                {"imag",  Imag},
                {"conj",  Conj},
                {"mag",   Mag},
                {"angle", Angle},
                {"round", Round},
            };
        }

        static public void GetZeroArgNames (List<string> funcs)
        {
            funcs.Add ("rand");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // CrossProduct  
        //    - two length-2 or two length-3 vectors

        static public PLVariable CrossProduct (PLVariable var)
        {
            PLRMatrix result = new PLRMatrix (3, 1);

            try
            {
                PLList lst = var as PLList;

                if (lst == null)
                    throw new Exception ("input args must be two column vectors");

                PLRMatrix v1 = lst [0] as PLRMatrix;
                PLRMatrix v2 = lst [1] as PLRMatrix;

                if (v1 == null || v2 == null)
                    throw new Exception ("input args");

                if (v1.IsColVector == false || v2.IsColVector == false)
                    throw new Exception ("input args must be two column vectors");
                
                if (v1.Rows != v2.Rows)
                    throw new Exception ("input vectors must be same length");
            
                if (v1.Rows == 2)
                {
                    result [2, 0] = v1 [0, 0] * v2 [1, 0] - v1 [1, 0] * v2 [0, 0]; 
                }

                else if (v1.Rows == 3)
                {
                    result [0, 0] =  v1 [1, 0] * v2 [2, 0] - v1 [2, 0] * v2 [1, 0]; 
                    result [1, 0] = (v1 [0, 0] * v2 [2, 0] - v1 [2, 0] * v2 [0, 0]) * -1; 
                    result [2, 0] =  v1 [0, 0] * v2 [1, 0] - v1 [1, 0] * v2 [0, 0]; 
                }

                else
                    throw new Exception ("input vectors must be length 2 or 3");
            }

            catch (Exception ex)
            {
                throw new Exception ("Error in cross product: " + ex.Message);
            }

            return result;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // extract part of a complex number

        static public PLVariable Real (PLVariable src)
        {
            if (src is PLComplex)
            {
                return new PLDouble ((src as PLComplex).Real);
            }

            else if (src is PLCMatrix)
            {
                PLCMatrix cmat = src as PLCMatrix;
                PLRMatrix results = new PLRMatrix (cmat.Rows, cmat.Cols);

                for (int i = 0; i<cmat.Rows; i++)
                    for (int j = 0; j<cmat.Cols; j++)
                        results [i, j] = cmat [i, j].Real;

                return results;
            }

            // throw new Exception ("Argument error, arg must be complex");
            return src;
        }

        static public PLVariable Imag (PLVariable src)
        {
            if (src is PLComplex)
            {
                return new PLDouble ((src as PLComplex).Imag);
            }

            else if (src is PLCMatrix)
            {
                PLCMatrix cmat = src as PLCMatrix;
                PLRMatrix results = new PLRMatrix (cmat.Rows, cmat.Cols);

                for (int i = 0; i<cmat.Rows; i++)
                    for (int j = 0; j<cmat.Cols; j++)
                        results [i, j] = cmat [i, j].Imag;

                return results;
            }

            //throw new Exception ("Argument error, arg must be complex");
            return new PLDouble (0);
        }

        static public PLVariable Conj (PLVariable src)
        {
            if (src is PLComplex)
            {
                return new PLComplex ((src as PLComplex).Real, -1 * (src as PLComplex).Imag);
            }

            else if (src is PLCMatrix)
            {
                PLCMatrix cmat = src as PLCMatrix;
                PLCMatrix results = new PLCMatrix (cmat.Rows, cmat.Cols);

                for (int i = 0; i<cmat.Rows; i++)
                    for (int j = 0; j<cmat.Cols; j++)
                        results [i, j] = new PLComplex (cmat [i, j].Real, -1 * cmat [i, j].Imag);

                return results;
            }

            //throw new Exception ("Argument error, arg must be complex");
            return src;
        }

        static public PLVariable Mag (PLVariable src)
        {
            if (src is PLDouble)
            {
                return new PLDouble (Math.Abs ((src as PLDouble).Data));
            }

            else if (src is PLComplex)
            {
                return new PLDouble ((src as PLComplex).Magnitude);
            }

            else if (src is PLCMatrix)
            {
                PLCMatrix cmat = src as PLCMatrix;
                PLRMatrix results = new PLRMatrix (cmat.Rows, cmat.Cols);

                for (int i = 0; i<cmat.Rows; i++)
                    for (int j = 0; j<cmat.Cols; j++)
                        results [i, j] = cmat [i, j].Magnitude;

                return results;
            }

            throw new Exception ("Argument error, arg must be complex");
        }

        static public PLVariable Angle (PLVariable src)
        {
            if (src is PLDouble)
            {
                bool pos = (src as PLDouble).Data >= 0;
                return new PLDouble (pos ? 0 : Math.PI);
            }

            else if (src is PLComplex)
            {
                return new PLDouble ((src as PLComplex).Angle);
            }

            else if (src is PLCMatrix)
            {
                PLCMatrix cmat = src as PLCMatrix;
                PLRMatrix results = new PLRMatrix (cmat.Rows, cmat.Cols);

                for (int i = 0; i<cmat.Rows; i++)
                    for (int j = 0; j<cmat.Cols; j++)
                        results [i, j] = cmat [i, j].Angle;

                return results;
            }

            throw new Exception ("Argument error, arg must be complex");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // Transpose 
        //    - implements ' operator

        static public PLVariable Transpose (PLVariable var)
        {
            if (var is PLDouble)
                return var;

            PLRMatrix arg = var as PLRMatrix;

            if (arg != null)
            {
                PLRMatrix result = new PLRMatrix (arg.Cols, arg.Rows);

                for (int i = 0; i<arg.Rows; i++)
                    for (int j = 0; j<arg.Cols; j++)
                        result [j, i] = arg [i, j];

                return result;
            }

            PLCMatrix arg2 = var as PLCMatrix;

            if (arg2 != null)
            {
                PLCMatrix result = new PLCMatrix (arg2.Cols, arg2.Rows);

                for (int i = 0; i<arg2.Rows; i++)
                    for (int j = 0; j<arg2.Cols; j++)
                        result [j, i] = new PLComplex (arg2 [i, j].Real, -1 * arg2 [i, j].Imag);

                return result;
            }

            throw new Exception ("Error: can't transpose type " + var.GetType ());
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        static Random random = new Random ();

        static public PLVariable Rand (PLVariable var)
        {
            PLRMatrix mat = var as PLRMatrix;
            PLNull   nul = var as PLNull;
            PLDouble dbl = var as PLDouble;
            PLList   lst = var as PLList;

            PLRMatrix rmat = new PLRMatrix (1, 1);

            if (nul != null) // true if rand invoked with no argument
            {
                // use default mat size of 1 x 1
            }

            else if (dbl != null)
            {
                int N = (int)(0.5 + dbl.Data);
                rmat = new PLRMatrix (N, N);
            }

            else if (lst != null)
            {
                if (lst.Count == 2)
                {
                    int M = (int)(0.5 + (lst.Data [0] as PLDouble).Data);
                    int N = (int)(0.5 + (lst.Data [1] as PLDouble).Data);
                    rmat = new PLRMatrix (M, N);
                }
            }

            else if (mat != null) // rand (size (z)); % where z is a matrix
            {
                rmat = new PLRMatrix ((int) mat [0, 0], (int) mat [0, 1]);
            }

            else
                throw new Exception ("Unreconized arguments to function rand");

            for (int r = 0; r<rmat.Rows; r++)
                for (int c = 0; c<rmat.Cols; c++)
                    rmat [r, c] = random.NextDouble ();

            return rmat;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        static public PLVariable Inverse (PLVariable var)
        {            
            if (var is PLRMatrix)
            { 
                CommonMath.Matrix inp = (var as PLRMatrix).Data;
                CommonMath.Matrix inv =  CommonMath.Matrix.Inverse (inp);
                return new PLRMatrix (inv);
            }

            else if (var is PLCMatrix)
            { 
                CommonMath.CMatrix inp = (var as PLCMatrix).Data;
                CommonMath.CMatrix inv =  CommonMath.CMatrix.Inverse (inp);
                return new PLCMatrix (inv);
            }

            else if (var is PLList)
            {
                CommonMath.Matrix inp = ((var as PLList) [0] as PLRMatrix).Data;
                CommonMath.Matrix inv =  CommonMath.Matrix.Inverse (inp);
                return new PLRMatrix (inv);
            }

            throw new Exception ("Inverse arg error: " + var.GetType () + " not supported");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Linspace - return a vector of linearly spaced spaced values
        /// </summary>
        /// <param name="arg">PLList, containing start, end and count</param>
        /// <returns></returns>

        static public PLVariable Linspace (PLVariable arg)
        {
            PLList args = arg as PLList;
            if (args == null) throw new Exception ("linspace argument error");
            if (args.Count != 3) throw new Exception ("linspace argument error");

            double start = (args [0] as PLDouble).Data;
            double stop = (args [1] as PLDouble).Data;
            int count = (int)(args [2] as PLDouble).Data;
            if (count < 2) throw new Exception ("linspace number of points (arg 3) must be 2 or greater");

            CommonMath.Matrix mat = Linspace (start, stop, count);
            PLRMatrix results = new PLRMatrix (mat);
            return results;
        }

        /// <summary>
        /// Linspace - return a vector of linearly spaced spaced values with standard .Net inputs
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        
        static public CommonMath.Matrix Linspace (double start, double stop, int count)
        {
            CommonMath.Matrix results = new CommonMath.Matrix (1, count);

            for (int i = 0; i<count; i++)
                results [0, i] = start + i * (stop - start) / (count - 1);

            return results;
        }

        //*********************************************************************************************

        static public PLVariable Zeros (PLVariable arg)
        {
            int rows = 0, cols = 0;
            PLList list = arg as PLList;
            PLDouble rc = arg as PLDouble; // rows and cols

            if (list != null)
            {
                if (list.Count > 2) throw new Exception ("zeros argument error");

                if (list.Count == 2)
                {
                    if (list [0] is PLInteger)
                        rows = (list [0] as PLInteger).Data;

                    else if (list [0] is PLDouble)
                        rows = (int) (list [0] as PLDouble).Data;

                    else 
                        throw new Exception ("ones argument error");

                    if (list [1] is PLInteger)
                        cols = (list [1] as PLInteger).Data;

                    else if (list [1] is PLDouble)
                        cols = (int) (list [1] as PLDouble).Data;

                    else 
                        throw new Exception ("ones argument error");
                }
            }

            if (rc != null) rows = cols = (int)rc.Data;

            PLRMatrix results = new PLRMatrix (rows, cols);
            return results;
        }

        //*********************************************************************************************

        static public PLVariable Ones (PLVariable arg)
        {
            int rows = 0, cols = 0;
            PLList list = arg as PLList;
            PLDouble rc = arg as PLDouble; // rows and cols

            if (list != null)
            {
                if (list.Count > 2) throw new Exception ("ones argument error");

                if (list.Count == 2)
                {
                    if (list [0] is PLInteger)
                        rows = (list [0] as PLInteger).Data;

                    else if (list [0] is PLDouble)
                        rows = (int) (list [0] as PLDouble).Data;

                    else 
                        throw new Exception ("ones argument error");

                    if (list [1] is PLInteger)
                        cols = (list [1] as PLInteger).Data;

                    else if (list [1] is PLDouble)
                        cols = (int) (list [1] as PLDouble).Data;

                    else 
                        throw new Exception ("ones argument error");
                }
            }

            if (rc != null) rows = cols = (int)rc.Data;

            PLRMatrix results = new PLRMatrix (rows, cols);

            for (int r=0; r<results.Rows; r++)
                for (int c=0; c<results.Cols; c++)
                    results [r, c] = 1;

            return results;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Delegate for static "Math" class functions that accept a single double precision float and return same
        /// </summary>
        ///
        delegate double ddFunction (double arg);

        /// <summary>
        /// MathFunction - accepts a function pointer, a PLDouble or PLRMatrix input and returns results in same format
        /// </summary>
        /// <param name="func - delegate referencing a function that accepts and returns a single double precision float"></param>
        /// <param name="name - only used for error reporting"></param>
        /// <param name="arg  - a PLDouble or a PLRMatrix. Other PL vars not supported"></param>
        /// <returns>A PLRMatrix or a PLDouble</returns>
        ///        
        static PLVariable MathFunction (ddFunction func, string name, PLVariable arg)
        {
            if (arg is PLDouble) return MathFunction (func, arg as PLDouble);
            if (arg is PLRMatrix) return MathFunction (func, arg as PLRMatrix);
            throw new Exception ("Argument error - " + name + " function");
        }

        /// <summary>
        /// MathFunction - accepts a single PLDouble, pass it to the function "func" and return a PLDouble
        /// </summary>
        /// <param name="func - delegate referencing the Math functio to invoke"></param>
        /// <param name="arg - a PLDouble"></param>
        /// <returns>A PLDouble</returns>
        /// 
        static PLDouble MathFunction (ddFunction func, PLDouble arg)
        {
            double funcArg = arg.Data; //
            double result = func (funcArg);
            return new PLDouble (result);
        }

        /// <summary>
        /// MathFunction - accepts a PLRMatrix and passes one element at a time to the function "func"
        /// </summary>
        /// <param name="func"></param>
        /// <param name="arg"></param>
        /// <returns>PLRMatrix same size as input arg</returns>
        /// 
        static PLRMatrix MathFunction (ddFunction func, PLRMatrix arg)
        {
            PLRMatrix result = new PLRMatrix (arg.Rows, arg.Cols);

            for (int i = 0; i<result.Rows; i++)
                for (int j = 0; j<result.Cols; j++)
                    result [i, j] = func (arg [i, j]);

            return result;
        }

        //*********************************************************************************************

        // the "name" string here is only for error reporting

        static public PLVariable Sin    (PLVariable arg) {return MathFunction (Math.Sin,  "sin",    arg);}
        static public PLVariable Cos    (PLVariable arg) {return MathFunction (Math.Cos,  "cos",    arg);}
        static public PLVariable Tan    (PLVariable arg) {return MathFunction (Math.Tan,  "tan",    arg);}
        static public PLVariable Abs    (PLVariable arg) {return MathFunction (Math.Abs,  "abs",    arg);}
        static public PLVariable Log    (PLVariable arg) {return MathFunction (Math.Log,  "log",    arg);}
        static public PLVariable Sinh   (PLVariable arg) {return MathFunction (Math.Sinh, "sinh",   arg);}
        static public PLVariable Cosh   (PLVariable arg) {return MathFunction (Math.Cosh, "cosh",   arg);}
        static public PLVariable Tanh   (PLVariable arg) {return MathFunction (Math.Tanh, "tanh",   arg);}
        static public PLVariable Square (PLVariable arg) {return MathFunction (_Square,   "square", arg);}
        static public PLVariable Log10  (PLVariable arg) {return MathFunction (Math.Log10, "log10", arg);}

        static public PLVariable Round  (PLVariable arg) {return MathFunction (Math.Round,  "round",    arg);}

        //*********************************************************************************

        static public PLVariable Exp (PLVariable arg) 
        {
            if (arg is PLComplex)
            {
                double re = (arg as PLComplex).Real;
                double im = (arg as PLComplex).Imag;

                double mag = Math.Exp (re);
                double re2 = Math.Cos (im);
                double im2 = Math.Sin (im);

                PLComplex results = new PLComplex (mag * re2, mag * im2);
                return results;
            }

            else if (arg is PLCMatrix)
            {
                PLCMatrix cmat = arg as PLCMatrix;
                int rows = cmat.Rows;
                int cols = cmat.Cols;
                PLCMatrix results = new PLCMatrix (rows, cols);

                for (int r = 0; r<rows; r++)
                {
                    for (int c = 0; c<cols; c++)
                    {
                        double re = cmat [r, c].Real;
                        double im = cmat [r, c].Imag;
                        double m = Math.Exp (re);
                        results [r, c] = new PLComplex (m * Math.Cos (im), m * Math.Sin (im));
                    }
                }

                return results;
            }

            return MathFunction (Math.Exp, "exp", arg);
        }

        //**********************************************************************

        static public PLVariable Sqrt (PLVariable arg) 
        {
            if (arg is PLDouble)
            {
                double d = (arg as PLDouble).Data;

                if (d >= 0)
                    return new PLDouble (Math.Sqrt (d));

                return new PLComplex (0, Math.Sqrt (-d));
            }

            return MathFunction (Math.Sqrt, "sqrt",   arg);
        }

        static public PLVariable Log2 (PLVariable arg) 
        {
            double d = (arg as PLDouble).Data;
            PLDouble dd = new PLDouble (Math.Log (d, 2));
            return dd;
        }

        static public PLVariable Ceil (PLVariable arg) 
        {
            double d = (arg as PLDouble).Data;
            PLDouble dd = new PLDouble (Math.Ceiling (d));
            return dd;
        }

        // for generating square waves
        static private double _Square (double arg)
        {
            return Math.Sin (arg) >= 0 ? 1.0 : -1.0;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        delegate double MaxOrMin (double a, double b);

        static PLVariable MaxMinCommon (MaxOrMin func, PLVariable arg)
        {
            PLList lst = arg as PLList;

            if (lst != null)
            {
                if (lst.Count != 2)
                    throw new Exception ("MaxMin arg error");

                PLDouble arg1 = lst [0] as PLDouble;
                PLDouble arg2 = lst [1] as PLDouble;
                return new PLDouble (func (arg1.Data, arg2.Data));
            }

            PLRMatrix mat = arg as PLRMatrix;

            if (mat != null)
            {
                //
                // one dimensional
                //
                if (mat.Rows == 1 || mat.Cols == 1)
                {
                    int count = mat.Size; 
                    double val = mat.Data [0, 0];

                    for (int i = 1; i<count; i++)
                        val = func (val, mat [i]);

                    return new PLDouble (val);
                }

                //
                // two dimensional
                //
                PLRMatrix results = new PLRMatrix (1, mat.Cols);

                for (int col=0; col<mat.Cols; col++)
                {
                    double val = mat [0, col];

                    for (int row = 1; row<mat.Rows; row++)
                        val = func (val, mat [row, col]);

                    results [0, col] = val;
                }

                return results;
            }

            throw new Exception ("MaxMin arg error");
        }

        static public PLVariable Max (PLVariable arg) {return MaxMinCommon (Math.Max, arg);}
        static public PLVariable Min (PLVariable arg) {return MaxMinCommon (Math.Min, arg);}

        //*********************************************************************************************

        static public PLVariable Atan2 (PLVariable arg)
        {
            if (arg is PLList)
            {
                PLList list = arg as PLList;
                PLRMatrix a = list [0] as PLRMatrix;
                PLRMatrix b = list [1] as PLRMatrix;
                PLDouble c = list [0] as PLDouble;
                PLDouble d = list [1] as PLDouble;

                if (a != null && b != null)
                    return Atan2 (a, b);

                else if (c != null && d != null)
                    return Atan2 (c, d);
            }

            throw new Exception ("Argument error - atan2 function");
        }

        static PLDouble Atan2 (PLDouble arg1, PLDouble arg2)
        {
            return new PLDouble (Math.Atan2 (arg1.Data, arg2.Data));
        }

        static PLRMatrix Atan2 (PLRMatrix arg1, PLRMatrix arg2)
        {
            if (arg1.Rows != arg2.Rows || arg1.Cols != arg2.Cols)
                throw new Exception ("Argument size error - atan2 function");

            PLRMatrix result = new PLRMatrix (arg1.Rows, arg1.Cols);

            for (int i = 0; i<result.Rows; i++)
                for (int j = 0; j<result.Cols; j++)
                    result [i, j] = Math.Atan2 (arg1 [i, j], arg2 [i, j]);

            return result;
        }
    }
}



