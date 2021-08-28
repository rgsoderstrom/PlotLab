﻿using System;
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
                {"sin", Sin},
                {"cos", Cos},
                {"sqrt", Sqrt},
                {"inv",  Inverse},
                {"atan2", Atan2},
                {"rand", Rand},
                {"Transpose", Transpose},
                {"log", Log},
                {"max", Max},
                {"min", Min},
                {"cross", CrossProduct},
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
            PLMatrix result = new PLMatrix (3, 1);

            try
            {
                PLList lst = var as PLList;

                if (lst == null)
                    throw new Exception ("input args must be two column vectors");

                PLMatrix v1 = lst [0] as PLMatrix;
                PLMatrix v2 = lst [1] as PLMatrix;

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

        // Transpose 
        //    - implements ' operator

        static public PLVariable Transpose (PLVariable var)
        {
            if (var is PLDouble)
                return var;

            PLMatrix arg = var as PLMatrix;

            if (arg != null)
            {
                PLMatrix result = new PLMatrix (arg.Cols, arg.Rows);

                for (int i = 0; i<arg.Rows; i++)
                    for (int j = 0; j<arg.Cols; j++)
                        result [j, i] = arg [i, j];

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
            PLMatrix mat = new PLMatrix (1, 1);
            PLNull   nul = var as PLNull;
            PLDouble dbl = var as PLDouble;
            PLList lst = var as PLList;

            if (nul != null) // true if rand invoked with no argument
            {
                // use default mat size of 1 x 1
            }

            else if (dbl != null)
            {
                int N = (int)(0.5 + dbl.Data);
                mat = new PLMatrix (N, N);
            }

            else if (lst != null)
            {
                if (lst.Count == 2)
                {
                    int M = (int)(0.5 + (lst.Data [0] as PLDouble).Data);
                    int N = (int)(0.5 + (lst.Data [1] as PLDouble).Data);
                    mat = new PLMatrix (M, N);
                }
            }

            else
                throw new Exception ("Unreconized arguments to function rand");

            for (int r = 0; r<mat.Rows; r++)
                for (int c = 0; c<mat.Cols; c++)
                    mat [r, c] = random.NextDouble ();

            return mat;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        static public PLVariable Inverse (PLVariable var)
        {
            CommonMath.Matrix mat = null;

            if (var is PLMatrix)
                mat = (var as PLMatrix).Data;

            else if (var is PLList)
            {
                if ((var as PLList) [0] is PLMatrix)
                    mat = ((var as PLList) [0] as PLMatrix).Data;
            }

            else
                throw new Exception ("Inverse arg error");

            CommonMath.Matrix mat1 = CommonMath.Matrix.Inverse (mat);

            return new PLMatrix ("inverse", mat1);
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        static public PLVariable Linspace (PLVariable arg)
        {
            PLList args = arg as PLList;
            if (args == null) throw new Exception ("linspace argument error");
            if (args.Count != 3) throw new Exception ("linspace argument error");

            double start = (args [0] as PLDouble).Data;
            double stop = (args [1] as PLDouble).Data;
            int count = (int)(args [2] as PLDouble).Data;
            if (count < 2) throw new Exception ("linspace arg 3 must be 2 or greater");

            PLMatrix results = new PLMatrix (1, count);

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
                    {
                        rows = (int)(list [0] as PLInteger).Data;
                        cols = (int)(list [1] as PLInteger).Data;
                    }

                    else if (list [0] is PLDouble)
                    {
                        rows = (int)(list [0] as PLDouble).Data;
                        cols = (int)(list [1] as PLDouble).Data;
                    }

                    else throw new Exception ("zeros argument error");
                }
            }

            if (rc != null) rows = cols = (int)rc.Data;

            PLMatrix results = new PLMatrix (rows, cols);
            return results;
        }

        //*********************************************************************************************

        delegate double ddFunction (double arg);

        static PLVariable MathFunction (ddFunction func, string name, PLVariable arg)
        {
            if (arg is PLDouble) return MathFunction (func, arg as PLDouble);
            if (arg is PLMatrix) return MathFunction (func, arg as PLMatrix);
            throw new Exception ("Argument error - " + name + " function");
        }

        static PLDouble MathFunction (ddFunction func, PLDouble arg)
        {
            return new PLDouble (func (arg.Data));
        }

        static PLMatrix MathFunction (ddFunction func, PLMatrix arg)
        {
            PLMatrix result = new PLMatrix (arg.Rows, arg.Cols);

            for (int i = 0; i<result.Rows; i++)
                for (int j = 0; j<result.Cols; j++)
                    result [i, j] = func (arg [i, j]);

            return result;
        }

        //*********************************************************************************************

        static public PLVariable Sin  (PLVariable arg) {return MathFunction (Math.Sin,  "sin", arg);}
        static public PLVariable Cos  (PLVariable arg) {return MathFunction (Math.Cos,  "cos", arg);}
        static public PLVariable Sqrt (PLVariable arg) {return MathFunction (Math.Sqrt, "sqrt", arg);}
        static public PLVariable Log  (PLVariable arg) {return MathFunction (Math.Log,  "log", arg);}

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

            PLMatrix mat = arg as PLMatrix;

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
                PLMatrix results = new PLMatrix (1, mat.Cols);

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
                PLMatrix a = list [0] as PLMatrix;
                PLMatrix b = list [1] as PLMatrix;
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

        static PLMatrix Atan2 (PLMatrix arg1, PLMatrix arg2)
        {
            if (arg1.Rows != arg2.Rows || arg1.Cols != arg2.Cols)
                throw new Exception ("Argument size error - atan2 function");

            PLMatrix result = new PLMatrix (arg1.Rows, arg1.Cols);

            for (int i = 0; i<result.Rows; i++)
                for (int j = 0; j<result.Cols; j++)
                    result [i, j] = Math.Atan2 (arg1 [i, j], arg2 [i, j]);

            return result;
        }
    }
}



