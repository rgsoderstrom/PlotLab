﻿using System;
using System.Collections.Generic;
using System.Linq;

using PLCommon;
using CommonMath;
using Plot2D_Embedded;
using PlottingLib;

namespace FunctionLibrary
{
    static public partial class ContourPlotFunctions
    {
        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        //
        // Contour Plot
        //

        // two options:
        //
        // contour (zFunction, levels, minX, maxX, minY, maxY)                   - NUMBER OF POINTS IS HARDCODED
        //      - arg1 is a function that returns double for an (x, y) input     - MUST BE CODED IN C# (see MathFunctionsUserDefined.cs)
        //      - arg2 is list of the levels to plot
        //      - next 4 define the plot boundry
        //
        // contour (xValues, yValues, zValues, levels)
        //      - args 1 & 2 are lists
        //      - arg 3 is a matrix
        //      - arg 4 is a list

        // points to PlotLab function that calculates z (x, y)
        //static PLFunction contourZFunction = null;

        // this function invoked by Plot2D_embedded Contour Plot drawing code. Translates 
        // the input .Net variable to PlotLab variables and the results back to .Net
        //static double ZFromXYFunction (double x, double y)
        //{
        //    PLDouble xx = new PLDouble (x);
        //    PLDouble yy = new PLDouble (y);
        //    PLList pll = new PLList ();
        //    pll.Add (xx);
        //    pll.Add (yy);

        //    PLVariable ZZ = contourZFunction (pll);
        //    return (ZZ as PLDouble).Data;
        //}

        //******************************************************************************************

        /// <summary>
        /// Entry point for 2D Contour drawing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        public static PLVariable ContourPlot (PLVariable input)
        {
            PLList dataArgs = new PLList ();
            PLList displayArgs = new PLList ();

            SplitArguments (input, ref dataArgs, ref displayArgs);

            if (dataArgs.Count == 1)
                AddAxesToData (dataArgs);

            return ContourPlot (dataArgs, displayArgs);
        }

        //****************************************************************************************

        /// <summary>
        /// Synthesize X and Y axis data. X will be 1:NumbCols, Y will be 1:NumbRows
        /// </summary>
        /// <param name="dataArgs">PLList</param>

        private static void AddAxesToData (PLList dataArgs)
        {
            if (dataArgs [0] is PLRMatrix mat)
            {
                int nr = mat.Rows;
                int nc = mat.Cols;

                PLRMatrix X = new PLRMatrix (1, nc);
                PLRMatrix Y = new PLRMatrix (1, nr);

                for (int i=0; i<nc; i++)
                    X [0, i] = i + 1;

                for (int j=0; j<nr; j++)
                    Y [0, j] = j + 1;

                dataArgs.Insert (0, X);
                dataArgs.Insert (1, Y);
            }
        }

        //****************************************************************************************

        /// <summary>
        /// separate data arguments and display option arguments
        /// </summary>
        /// <param name="input">List containing all args passed to ContourPlot</param>
        /// <param name="dataArgs"></param>
        /// <param name="displayArgs"></param>
        
        private static void SplitArguments (PLVariable input, ref PLList dataArgs, ref PLList displayArgs)
        {
            // if input is just a matrix then there are no display args
            if (input is PLRMatrix)
            {
                dataArgs.Add (input as PLRMatrix);
                return; 
            }

            else if (input is PLList lst)
            {
                int nextArg; // index, our current position in input list

                //
                // get the data args into a separate list
                //
                for (nextArg=0; nextArg<lst.Count; nextArg++)
                {
                    if (lst [nextArg] is PLRMatrix mat)
                    {
                        if (mat.IsVector)
                            dataArgs.Add (mat);
                        else
                        {
                            dataArgs.Add (mat); // matrix must be the last data arg
                            break;
                        }
                    }

                    else
                        throw new Exception ("Contour plot argument error - must specify all data arguments before any display options");
                }


                //
                // look for optional vector or number specifying levels
                //
                
                if (++nextArg == lst.Count)
                    return;

                if (lst [nextArg] is PLDouble dbl)
                {
                    displayArgs.Add (new PLString ("Levels"));
                    displayArgs.Add (lst [nextArg] as PLDouble);
                    nextArg++;
                }

                else if (lst [nextArg] is PLRMatrix mat)
                {
                    displayArgs.Add (new PLString ("Levels"));
                    displayArgs.Add (lst [nextArg] as PLRMatrix);
                    nextArg++;
                }

                //
                // remainder are display option args
                //
                while (nextArg<lst.Count)
                {
                    displayArgs.Add (lst [nextArg++]);
                }
            }
        }

        //******************************************************************************

        private static PLVariable ContourPlot (PLList dataArgs, PLList displayArgs)
        {
            try
            {
                //
                // copy data into classes the plot library wants
                //
                PLRMatrix xx = dataArgs [0] as PLRMatrix;
                PLRMatrix yy = dataArgs [1] as PLRMatrix;
                PLRMatrix zz = dataArgs [2] as PLRMatrix;

                List<double> x = new List<double> (xx.Cols);
                for (int i=0; i<xx.Cols; i++)
                    x.Add (xx [0, i]);

                List<double> y = new List<double> (yy.Cols);
                for (int i=0; i<yy.Cols; i++)
                    y.Add (yy [0, i]);

                CommonMath.Matrix z = zz.Data;

                if (z.Rows != y.Count || z.Cols != x.Count)
                    throw new Exception ("Z Matrix size error");

                //
                // display options
                //  - must be pairs of string-value
                //  - 'Levels', [1 2 3],
                //

                // set all to default values
                List<double> levels = new List<double> ();
                ContourPlotView.ShowGradientArrows    = false; // these are all static and not set by the ContourPlotView ctor
                ContourPlotView.LabelLines            = false;
                ContourPlotView.ShowColoredBackground = false;
                ContourPlotView.DrawLines             = true;
                ContourPlotView.DrawLinesInColors     = false;

                //
                // defaults for case where no display arguments specified
                //
                if (displayArgs.Count == 0)
                {
                    displayArgs.Add (new PLString ("Levels"));
                    displayArgs.Add (new PLDouble (5)); // 5 equally spaced levels
                }



                for (int i=0; i<displayArgs.Count; i++)
                {
                    string arg = (displayArgs [i] as PLString).Data;
                    arg = arg.Trim (new char[] {'\''});

                    switch (arg)
                    {


                        case "Levels":
                        { 
                            if (displayArgs [++i] is PLRMatrix mat)
                            {
                                if (mat.IsRowVector == false)
                                    throw new Exception ("Levels must be specified by a number or a row vector");

                                for (int j=0; j<mat.Cols; j++)
                                    levels.Add (mat [0, j]);
                            }

                            else if (displayArgs [i] is PLDouble dbl)
                            {
                                int numberLevels = (int) dbl.Data;
                                double max = z.Max ();
                                double min = z.Min ();

                                if (numberLevels == 1)
                                {
                                    levels.Add ((max + min) / 2);
                                }

                                else if (numberLevels == 2)
                                {
                                    levels.Add (min + (max - min) * 0.33);
                                    levels.Add (min + (max - min) * 0.66);
                                }

                                else if (numberLevels > 2)
                                {
                                    double first = min + (max - min) * 0.1;
                                    double last  = min + (max - min) * 0.9;
                                    CommonMath.Matrix lvls = MathFunctions.Linspace (first, last, numberLevels);

                                    for (int j=0; j<lvls.Cols; j++)
                                        levels.Add (lvls [0, j]);
                                }

                                else
                                    throw new Exception ("Number of levels must be positive number");
                            }

                            else
                                throw new Exception ("Levels must be specified by a number or a row vector");
                        }
                        break;
        




				        case "ShowBackground":
                            ContourPlotView.ShowColoredBackground = ContourArgToBool (displayArgs [++i]);
                            break;

                        case "DrawContourLines":
                            ContourPlotView.DrawLines = ContourArgToBool (displayArgs [++i]);
                            break;

                        case "DrawLinesInColors":
                            ContourPlotView.DrawLinesInColors = ContourArgToBool (displayArgs [++i]);
                            break;

                        case "DrawArrows":
                            ContourPlotView.ShowGradientArrows = ContourArgToBool (displayArgs [++i]);
                            break;

                        case "ArrowSize":
                            ContourPlotView.GradientArrowSize = (displayArgs [++i] as PLDouble).Data;
                            break;
    
                        case "LabelLines":
                            ContourPlotView.LabelLines = ContourArgToBool (displayArgs [++i]);
                            break;

                        case "LabelFontSize":
                            ContourPlotView.LabelFontSize = (displayArgs [++i] as PLDouble).Data;
                            break;
    
                        default:
                            throw new Exception ("Unrecognized display option " + (displayArgs [i] as PLString).Data);
                    }
                }

                ContourPlotView cpv = new ContourPlotView (x, y, z, levels);
                PlotFunctions.Draw2DObject (cpv);
                (PlotFunctions.CurrentFigure as Plot2D).AxesEqual = false;
            }

            catch (Exception ex)
            {
                throw new Exception ("ContourPlot error: " + ex.Message);
            }

            return new PLNull ();
        }

        //******************************************************************************

        /// <summary>
        /// Convert a PLDouble, PLString or PLBool to .Net bool 
        /// </summary>
        /// <param name="arg">PLDouble, PLString or PLBool</param>
        /// <returns>bool</returns>
        
        private static bool ContourArgToBool (PLVariable arg)
        {
            PLDouble  pd = arg as PLDouble;
            PLString  ps = arg as PLString;
            PLBool    pb = arg as PLBool;

            bool result = false;
            if (pd != null) result = pd.Data != 0;
            if (pb != null) result = pb.Data;

            if (ps != null)
            { 
                if (ps.Data == "'true'")
                    result = true;

                else if (ps.Data == "'false'")
                    result = false;

                else
                    throw new Exception ("String argument " + ps.Data + " must be \'true\' or \'false\'");
            }

            return result;
        }

        //******************************************************************************

        //static PLVariable ContourPlotFromFunction (PLFunctionWrapper func, PLVariable _levels, PLDouble a2, PLDouble a3, PLDouble a4, PLDouble a5)
        //{ 
        //    //
        //    // extract function pointer
        //    //
        //    contourZFunction = func.Data;

        //    //
        //    // list of levels
        //    //
        //    PLRMatrix mat = _levels as PLRMatrix;
        //    PLDouble dbl = _levels as PLDouble;  // if a single level

        //    List<double> levels = new List<double> ();

        //    if (mat != null)
        //    {
        //        if (mat.Data.Rows != 1) throw new Exception ("Contour levels must be a row vector");
        //        for (int i = 0; i<mat.Data.Cols; i++) levels.Add (mat.Data [0, i]);
        //    }
        //    else if (dbl != null)
        //    {
        //        levels.Add (dbl.Data);
        //    }
        //    else
        //        throw new Exception ("Contour plot levels error");

        //    //
        //    // plot x & y limits
        //    //
        //    if (a2 == null || a3 == null || a4 == null || a5 == null)
        //        throw new Exception ("Contour plot argument error");

        //    ContourPlotView.ShowGradientArrows = false;
        //    ContourPlotView.LabelLines = false;
        //    ContourPlotView cpv = new ContourPlotView (ZFromXYFunction, levels, a2.Data, a3.Data, a4.Data, a5.Data, 100, 100);

        //    PlotFunctions.Draw2DObject (cpv);
        //    //Plot2D fig = CurrentFigure as Plot2D;
        //    //fig.SetAxes (a2.Data, a3.Data, a4.Data, a5.Data);
        //    //fig.AxesEqual = true;

        //    return new PLNull (); 
        //}

    }
}
