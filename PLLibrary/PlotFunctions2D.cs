using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using PLCommon;
using Plot2D_Embedded;
using PlottingLib;

//
// Private methods and variables for plot functions
//

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        //******************************************************************************************

        static void Draw2DObject (CanvasObject co)
        {
            if (CurrentFigure == null)
                NewFigure2D ();

            if (CurrentFigure is PlotFigure)
                NewFigure2D (CurrentFigure as PlotFigure);

            if (CurrentFigure is Plot3D)
                NewFigure2D ();

            Plot2D fig = CurrentFigure as Plot2D;

            fig.RectangularGridOn = true;
            fig.Plot (co);
        }

        static void Draw2DObject (ContourPlotView co)
        {
            if (CurrentFigure == null)
                NewFigure2D ();

            if (CurrentFigure is PlotFigure)
                NewFigure2D (CurrentFigure as PlotFigure);

            if (CurrentFigure is Plot3D)
                NewFigure2D ();

            Plot2D fig = CurrentFigure as Plot2D;

            fig.RectangularGridOn = true;
            fig.Plot (co);
        }

        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************

        static bool CharToDrawingOption (char c,
                                         ref LineView.DrawingStyle lineStyle, 
                                         ref PointView.DrawingStyle pointStyle, 
                                         ref Brush color)
        {
            bool success = true;

            switch (c)
            {
                case 'r': color = Brushes.Red; break;
                case 'n': color = Brushes.Orange; break;
                case 'y': color = Brushes.Yellow; break;
                case 'g': color = Brushes.Green; break;
                case 'b': color = Brushes.Blue; break;
                case 'k': color = Brushes.Black; break;

                case '-':
                {
                    if (lineStyle == LineView.DrawingStyle.Dashes)
                        lineStyle = LineView.DrawingStyle.LongDashes;
                    else
                        lineStyle = LineView.DrawingStyle.Dashes;
                }
                break;

                case '.': lineStyle = LineView.DrawingStyle.Dots; break;

                case 'o': pointStyle = PointView.DrawingStyle.Circle; break;
                case 's': pointStyle = PointView.DrawingStyle.Square; break;
                case '*': pointStyle = PointView.DrawingStyle.Star; break; 
                case 't': pointStyle = PointView.DrawingStyle.Triangle; break; 
                case 'x': pointStyle = PointView.DrawingStyle.X; break; 
                case '+': pointStyle = PointView.DrawingStyle.Plus; break; 

                default: success = false; break;
            }

            return success;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************
        //
        // Plot function
        //

        static public PLCanvasObject Plot (PLVariable input)
        {
            //
            // convert input argument to a PLLIst
            //
            if (input is PLList)
                return Plot (input as PLList);

            else if (input is PLMatrix) // e.g. plot (y)
                return Plot (new PLList () {input as PLMatrix});

            throw new Exception ("Plot input type " + input.GetType () + " not supported");
        }

        //*******************************************************************************************
        //
        //    - start unpacking plot arguments
        // 

        static PLCanvasObject Plot (PLList input)
        {
            //
            // the first one, two or three args are the data to plot
            //
            int count = 0;

            if (input [0] is PLMatrix || input [0] is PLDouble) count++;
            
            if (input.Count > 1) if (input [1] is PLMatrix || input [1] is PLDouble) count++;

            // if only one data arg (the ordinate), we create the abscissa
            if (count == 1) 
                return Plot1 (input);
            
            if (count == 2) 
                return Plot2 (input);

            throw new Exception ("Plot arguments error");
        }





        //*************************************************************************************************
        //
        // Plot1 
        //  - one data collection, e.g. plot (y) where y is a PLMatrix
        //  - generate abscissa values here then plot as plot (x, y)
        //

        static PLCanvasObject Plot1 (PLList args)
        {
            int length = (args [0] as PLMatrix).Size;
            PLMatrix xNumbers = new PLMatrix (1, length);
            xNumbers.Name = "x";

            for (int i = 0; i<length; i++)
                xNumbers.Data [0, i] = i + 1;

            args.Data.Insert (0, xNumbers);

            return Plot2 (args); // continue as if input was plot (x, y, ...) format
        }

        //*************************************************************************************************
        //
        // Plot2 - two data collections
        //
        static PLCanvasObject Plot2 (PLList args)
        {
            //
            // convert the two data args into List<Point>
            //
            List<Point> points = new List<Point> ();

            PLMatrix mx = args [0] as PLMatrix;
            PLMatrix my = args [1] as PLMatrix;

            PLDouble px = args [0] as PLDouble;  // e.g. plot (3, 7, ...)
            PLDouble py = args [1] as PLDouble;

            bool singlePoint = (px != null && py != null);
            bool setOfPoints = (mx != null && my != null);

            if (singlePoint == false && setOfPoints == false)
                throw new Exception ("Plot argument error");

            //
            // if just one point, convert it to a list with just one entry
            //
            if (singlePoint)
            {
                points.Add (new Point (px.Data, py.Data));
            }

            //
            // read matrices by column
            //
            else
            {
                if (mx.Size == my.Size)
                {
                    List<double> x = mx.ReadByColumn ();
                    List<double> y = my.ReadByColumn ();

                    for (int i = 0; i<x.Count; i++)
                        points.Add (new Point (x [i], y [i]));
                }
                else
                    throw new Exception ("Plot data x, y sizes different");
            }

            //
            // look at remaining args. are we plotting discrete points or a connected line?
            //

            Brush dataColor = null;
            PointView.DrawingStyle pointStyle = PointView.DrawingStyle.None;
            LineView.DrawingStyle lineStyle   = LineView.DrawingStyle.None;

            //
            // look at args [2] to help determine actual color and style values
            // 

            if (args.Count > 2)
            {
                string str = (args [2] as PLString).Data;

                // remove leading and trailing single quote
                int i1 = str.IndexOf ('\'');
                int i2 = str.LastIndexOf ('\'');

                if (i1 == -1 || i2 == -1) throw new Exception ("Plot option syntax error: " + str);

                str = str.Remove (i2);
                str = str.Remove (i1, 1);

                foreach (char c in str)
                {
                    bool success = CharToDrawingOption (c, ref lineStyle, ref pointStyle, ref dataColor);

                    if (success == false)
                        throw new Exception ("Unsupported plot option: " + str);
                }
            }

            // fill-in any not explicitly set
            if (dataColor == null) dataColor = Brushes.Black;

            if (pointStyle == PointView.DrawingStyle.None && lineStyle == LineView.DrawingStyle.None) lineStyle = LineView.DrawingStyle.Solid;

            if (pointStyle != PointView.DrawingStyle.None)
                return PlotPoints (points, dataColor, pointStyle);

            else 
                return PlotLine (points, dataColor, lineStyle);

            throw new Exception ("Plot argument error");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // PlotLimits - "axis" handler for some options

        static public PLVariable PlotLimits (PLVariable arg)
        {
            if (CurrentFigure is Plot2D == false)
                return new PLNull ();

            PLMatrix mat = arg as PLMatrix; // axis ([1 3 5 7])
            PLString str = arg as PLString; // axis auto

            if (str != null)
                return AxisConstraints (str);

            Plot2D fig = CurrentFigure as Plot2D;

            if (mat != null)
            {
                fig.SetAxes (mat [0, 0], mat [0, 1], mat [0, 2], mat [0, 3]);
                return new PLNull ();
            }

            else // no arg, so return current limits
            { 
                PLMatrix lim = new PLMatrix (1, 4);

                double x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                fig.GetAxes (ref x1, ref x2, ref y1, ref y2);

                lim [0, 0] = x1;
                lim [0, 1] = x2;
                lim [0, 2] = y1;
                lim [0, 3] = y2;
                return lim;
            }

            throw new Exception ("Axis argument error");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        //
        // Set - change a plot option
        //

        //
        // set (handle, 'property', value, ...)
        //

        static public PLVariable Set (PLVariable input)
        {
            // ensure input is a list
            PLList args = input as PLList;

            if (args == null)
                throw new Exception ("Set function argument error");

            if (args [0] is PLCanvasObject)
                return SetCanvasObject (args);

            if (args [0] is PLInteger || args [0] is PLDouble)
                return SetFigure (args);

            throw new Exception ("Set function argument error");
        }

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
        static PLFunction contourZFunction = null;

        // this function invoked by Plot2D_embedded Contour Plot drawing code. Translates 
        // the input .Net variable to PlotLab variables and the results back to .Net
        static double ZFromXYFunction (double x, double y)
        {
            PLDouble xx = new PLDouble (x);
            PLDouble yy = new PLDouble (y);
            PLList pll = new PLList ();
            pll.Add (xx);
            pll.Add (yy);

            PLVariable ZZ = contourZFunction (pll);
            return (ZZ as PLDouble).Data;
        }

        //******************************************************************************************
        //
        // Common entry point
        //

        static public PLVariable ContourPlot (PLVariable input)
        {
            // ensure input is a list
            PLList args = input as PLList;

            if (args == null)
                throw new Exception ("Contour plot argument error");

            // see what first arg is
            PLFunctionWrapper fptr = args [0] as PLFunctionWrapper;
            PLMatrix          mat  = args [0] as PLMatrix;

            if (fptr != null)
            {
                if (args.Count != 6) throw new Exception ("ContourPlot argument error");

                PLVariable levels = args [1];  // double or an array
                PLDouble a = args [2] as PLDouble;
                PLDouble b = args [3] as PLDouble;
                PLDouble c = args [4] as PLDouble;
                PLDouble d = args [5] as PLDouble;

                ContourPlotFromFunction (fptr, levels, a, b, c, d);

            }

            else if (mat != null)
            {
                PLMatrix a = args [0] as PLMatrix;
                PLMatrix b = args [1] as PLMatrix;
                PLMatrix c = args [2] as PLMatrix;

                if (a == null || b == null || c == null)              throw new Exception ("Contour Plot arg error - first 3 args must be arrays");
                if (a.IsRowVector == false || b.IsRowVector == false) throw new Exception ("Contour Plot arg error - first 2 args must be row vectors");
                if (c.IsMatrix == false)                              throw new Exception ("Contour Plot arg error - third arg must be matrix");

                List<double> x = new List<double> ();
                List<double> y = new List<double> ();

                for (int i=0; i<a.Cols; i++) x.Add (a [0, i]);
                for (int i=0; i<b.Cols; i++) y.Add (b [0, i]);




                PLMatrix d1 = args [3] as PLMatrix;
                PLDouble d2 = args [3] as PLDouble;

                List<double> levels = new List<double> ();

                if      (d1 != null) for (int i=0; i<d1.Cols; i++) levels.Add (d1 [0, i]);                
                else if (d2 != null) levels.Add (d2.Data);
                else                 throw new Exception ("Contour Plot arg error - 4th arg");


                ContourPlotFromMatrix (x, y, c.Data, levels);
            }

            else
                throw new Exception ("Contour Plot syntax error");

            return new PLNull ();
        }

        //******************************************************************************
    
        static PLVariable ContourPlotFromMatrix (List<double> x, List<double> y, CommonMath.Matrix z, List<double> levs)
        {
            ContourPlotView cpv = new ContourPlotView (x, y, z, levs);

            cpv.ShowGradientArrows = false; // not available ??????????
            cpv.ShowText = false;

            Draw2DObject (cpv);
            //Fig2D.SetAxes (a2.Data, a3.Data, a4.Data, a5.Data);
            (CurrentFigure as Plot2D).AxesEqual = true;

            return new PLNull ();
        }

        //******************************************************************************
         
        static PLVariable ContourPlotFromFunction (PLFunctionWrapper func, PLVariable _levels, PLDouble a2, PLDouble a3, PLDouble a4, PLDouble a5)
        { 
            //
            // extract function pointer
            //
            contourZFunction = func.Data;

            //
            // list of levels
            //
            PLMatrix mat = _levels as PLMatrix;
            PLDouble dbl = _levels as PLDouble;  // if a single level

            List<double> levels = new List<double> ();

            if (mat != null)
            {
                if (mat.Data.Rows != 1) throw new Exception ("Contour levels must be a row vector");
                for (int i = 0; i<mat.Data.Cols; i++) levels.Add (mat.Data [0, i]);
            }
            else if (dbl != null)
            {
                levels.Add (dbl.Data);
            }
            else
                throw new Exception ("Contour plot levels error");

            //
            // plot x & y limits
            //
            if (a2 == null || a3 == null || a4 == null || a5 == null)
                throw new Exception ("Contour plot argument error");

            ContourPlotView cpv = new ContourPlotView (ZFromXYFunction, levels, a2.Data, a3.Data, a4.Data, a5.Data, 100, 100);
            cpv.ShowText = false;
            cpv.ShowGradientArrows = false;

            Draw2DObject (cpv);
            Plot2D fig = CurrentFigure as Plot2D;
            fig.SetAxes (a2.Data, a3.Data, a4.Data, a5.Data);
            fig.AxesEqual = true;

            return new PLNull (); 
        }


        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        //
        // Plot a single vector arrow
        //

        //"tail" and "Vect" are columns
        //"basis" is matrix
        //"style" always optional, must be last arg if present

        //PlotVector (Vect);                        - std basis, tail at origin, default color
        //PlotVector (Vect, 'style');               - std basis, tail at origin
        //PlotVector (tail, Vect, 'style');         - std basis
        //PlotVector (Vect, basis, 'style');        - tail at origin
        //PlotVector (tail, Vect, basis, 'style');

        static public PLVariable PlotVector2D (PLVariable input)
        {
            // defaults
            Point tail = new Point (0, 0);
            Vector vect = new Vector (1, 0);
            List<Vector> basis = new List<Vector> () {new Vector (1, 0), new Vector (0, 1)};
            string style = "k";
            Brush LineColor = Brushes.Black; // default color
            bool showComponents = false;
            LineView.DrawingStyle LineStyle = LineView.DrawingStyle.Solid;

            if (input is PLList lst)
            {
                int dataArgCount = lst.Count;

                // see if last arg is style string.
                if (lst [lst.Count - 1] is PLString)
                {
                    style = (lst [lst.Count - 1] as PLString).Text;
                    dataArgCount--; // ignore last arg
                }

                if (dataArgCount == 0)
                {
                    throw new Exception ("PlotVector - nothing to plot");
                }

                if (dataArgCount == 1)
                {
                    PLMatrix mat = lst [0] as PLMatrix;
                    if (mat == null) throw new Exception ("PlotVector arg1 must be 2D col vector");
                    if (mat.Rows != 2 || mat.Cols != 1) throw new Exception ("PlotVector arg1 must be 2D col vector");
                    vect = new Vector (mat [0, 0], mat [1, 0]);
                }

                else if (dataArgCount == 2)
                {
                    PLMatrix mat1 = lst [0] as PLMatrix;
                    PLMatrix mat2 = lst [1] as PLMatrix;
                    if (mat1 == null || mat2 == null) throw new Exception ("PlotVector arg1 error");

                    if (mat2.Cols == 1 && mat2.Rows == 2) // 2nd arg is a vector
                    {
                        tail = new Point (mat1 [0, 0], mat1 [1, 0]);
                        vect = new Vector (mat2 [0, 0], mat2 [1, 0]);
                    }
                    else if (mat2.Cols == 2 && mat2.Rows == 2) // 2nd arg is a matrix
                    {
                        vect = new Vector (mat1 [0, 0], mat1 [1, 0]);
                        basis.Clear ();
                        basis.Add (new Vector (mat2 [0, 0], mat2 [1, 0]));
                        basis.Add (new Vector (mat2 [0, 1], mat2 [1, 1]));
                    }
                    else
                        throw new Exception ("PlotVector arg1 error");
                }

                else if (dataArgCount == 3)
                {
                    PLMatrix mat1 = lst [0] as PLMatrix;
                    PLMatrix mat2 = lst [1] as PLMatrix;
                    PLMatrix mat3 = lst [2] as PLMatrix;
                    if (mat1 == null || mat2 == null || mat3 == null) throw new Exception ("PlotVector arg1 error");

                    if (mat1.IsColVector == false) throw new Exception ("PlotVector 1st arg must be column vector");
                    if (mat2.IsColVector == false) throw new Exception ("PlotVector 2nd arg must be column vector");
                    if (mat3.IsMatrix    == false) throw new Exception ("PlotVector 3rd arg must be matrx");

                    if (mat1.Rows != 2) throw new Exception ("PlotVector 1st arg must be column vector, length 2");
                    if (mat2.Rows != 2) throw new Exception ("PlotVector 2nd arg must be column vector, length 2");
                    if (mat3.Rows != 2 && mat3.Cols != 2) throw new Exception ("PlotVector 3rd arg must be 2x2 matrix");

                    tail = new Point (mat1 [0, 0], mat1 [1, 0]);
                    vect = new Vector (mat2 [0, 0], mat2 [1, 0]);

                    basis.Clear ();
                    basis.Add (new Vector (mat3 [0, 0], mat3 [1, 0]));
                    basis.Add (new Vector (mat3 [0, 1], mat3 [1, 1]));
                }

                else
                    throw new Exception ("PlotVector - too many arguments");

                // style setting
                foreach (char c in style)
                {
                    switch (c)
                    {
                        case 'r': LineColor = Brushes.Red; break;
                        case 'o': LineColor = Brushes.Orange; break;
                        case 'y': LineColor = Brushes.Yellow; break;
                        case 'g': LineColor = Brushes.Green; break;
                        case 'b': LineColor = Brushes.Blue; break;
                        case 'k': LineColor = Brushes.Black; break;
                        case '-': LineStyle = LineView.DrawingStyle.Dashes; break; // see CharToDrawingOption to expand this
                        case '.': LineStyle = LineView.DrawingStyle.Dots; break;
                        case 'c': showComponents = true; break;
                    }
                }
            }

            else if (input is PLMatrix mat) // column vector only
            {
                if (mat == null) throw new Exception ("PlotVector arg1 must be 2D col vector");
                if (mat.Rows != 2 || mat.Cols != 1) throw new Exception ("PlotVector arg1 must be 2D col vector");
                vect = new Vector (mat [0, 0], mat [1, 0]);
            }

            else
                throw new Exception ("PlotVector argument error");

            VectorView VV;

            if (basis.Count == 2) VV = new VectorView (tail, vect, basis);
            else VV = new VectorView (tail, vect);

            VV.Color = LineColor;
            VV.LineStyle = LineStyle;
            VV.ShowComponents = showComponents;

            Draw2DObject (VV [0]);
            Plot2D fig = CurrentFigure as Plot2D;
            fig.Hold = true;
            fig.AxesEqual = true;
            Draw2DObject (VV [1]);
            Draw2DObject (VV [2]);

            PLList list = new PLList ();

            foreach (CanvasObject co in VV)
                list.Add (new PLCanvasObject (co));

            return list;
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************
        
        //
        // Plot a vector field
        //

        //
        // vect (tails, vects, ...)
        //      - arg1 & arg2 are matrices, 2 rows by N columns
        //

        static public PLCanvasObject PlotVectorField (PLVariable input)
        {
            List<Point> tails = new List<Point> ();
            List<Vector> vects = new List<Vector> ();

            // ensure input is a list
            PLList args = input as PLList;

            if (args == null)
                throw new Exception ("Vector plot argument error");

            if (args.Count >= 2)
            {
                if (args [0] is PLMatrix mat)
                    if (mat != null)
                        for (int col = 0; col<mat.Cols; col++)
                            tails.Add (new Point (mat [0, col], mat [1, col]));

                if (args [1] is PLMatrix mat2)
                    if (mat2 != null)
                        for (int col = 0; col<mat2.Cols; col++)
                            vects.Add (new Vector (mat2 [0, col], mat2 [1, col]));
            }

            else
                throw new Exception ("Quiver plot argument error");


            VectorFieldView vfv = new VectorFieldView (tails, vects);
            Draw2DObject (vfv);
            return new PLCanvasObject (vfv);
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        //
        // Text function
        //

        static public PLCanvasObject Text (PLVariable input)
        {
            try
            {
                PLList lst = input as PLList;

                PLDouble xd  = lst [0] as PLDouble;
                PLDouble yd  = lst [1] as PLDouble;
                PLMatrix mat = lst [0] as PLMatrix;

                double x, y;
                string txt;

                if (xd != null && yd != null)
                {
                    x = xd.Data;
                    y = yd.Data;
                    txt = (lst [2] as PLString).Data;
                }

                else if (mat != null && mat.IsColVector == true)
                {
                    x = mat.Data [0, 0];
                    y = mat.Data [1, 0];
                    txt = (lst [1] as PLString).Data;
                }

                else
                    throw new Exception ("text function arg error");

                if (txt [0] == '\'') txt = txt.Remove (0, 1);
                if (txt [txt.Length - 1] == '\'') txt = txt.Remove (txt.Length - 1);

                TextView tv = new TextView (new Point (x, y), txt);
                tv.FontSizeAppInUnits = 0.2;
                tv.Color = Brushes.Blue;

                Draw2DObject (tv);
                return new PLCanvasObject (tv);
            }

            catch (Exception ex)
            {
                throw new Exception ("Error plotting text: " + ex.Message);
            }
        }

        //**************************************************************************************
        //
        // plot discrete, disconnected points
        //
        static PLCanvasObject PlotPoints (List<Point> points, Brush color, PointView.DrawingStyle style)
        {
            PointView pv = new PointView (points, style);            
            pv.Color = color;
            pv.Size = 0.025;

            Draw2DObject (pv);
            return new PLCanvasObject (pv);
        }

        //**************************************************************************************
        //
        // plot a line connecting points
        //
        static PLCanvasObject PlotLine (List<Point> points, Brush color, LineView.DrawingStyle style)
        {
            LineView lv = new LineView (points);
            lv.LineStyle = style;
            lv.Color = color;

            Draw2DObject (lv);
            return new PLCanvasObject (lv);
        }
    }
}
