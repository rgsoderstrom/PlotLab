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

        static internal void Draw2DObject (CanvasObject co)
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

        static internal void Draw2DObject (ContourPlotView co)
        {
            if (CurrentFigure == null)
                NewFigure2D ();

            if (CurrentFigure is PlotFigure)
                NewFigure2D (CurrentFigure as PlotFigure);

            if (CurrentFigure is Plot3D)
                NewFigure2D ();

            Plot2D fig = CurrentFigure as Plot2D;
            fig.Hold = true;
            fig.AxesEqual = true;
            fig.RectangularGridOn = true;
            fig.Plot (co);
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
                fig.AxesEqual  = false;
                fig.AxesFrozen = false;
                fig.AxesTight  = false;
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
        // Get - read a plot option
        //

        //
        // get (handle, 'property')
        //

        // Only supported propery is 'Position'

        static public PLVariable Get (PLVariable input)
        {
            // ensure input is a list
            PLList args = input as PLList;

            int figureID;
            string option = null;
            IPlotCommon figure = null;
            bool found = false;

            if (args [0] is PLInteger)
                figureID = (args [0] as PLInteger).Data;

            else if (args [0] is PLDouble)
                figureID = (int)(args [0] as PLDouble).Data;

            else
                throw new Exception ("Figure " + args [0].ToString () + " not found");

            // look for that id. if found make it the current figure
            foreach (Window w in Figures)
            {
                PlotFigure pf = w as PlotFigure; if (pf != null) { if (pf.ID == figureID) { found = true; figure = pf; break; } }
                Plot2D p2 = w as Plot2D; if (p2 != null) { if (p2.ID == figureID) { found = true; figure = p2; break; } }
                Plot3D p3 = w as Plot3D; if (p3 != null) { if (p3.ID == figureID) { found = true; CurrentFigure = p3; break; } }
            }

            if (found == false || figure == null)
                throw new Exception ("Figure " + args [0].ToString () + " not found");

            option = (args [1] as PLString).Data;

            switch (option)
            {
                case "'Position'":
                case "'position'":
                    CommonMath.Matrix size = new CommonMath.Matrix (1, 4);
                    size [0, 0] = figure.Left;
                    size [0, 1] = figure.Top;
                    size [0, 2] = figure.Width;
                    size [0, 3] = figure.Height;

                    return new PLMatrix (size);

                default:
                    throw new Exception ("Get option not supported");
            }
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        static public PLVariable Grid (PLVariable arg)
        {
            if (CurrentFigure != null && CurrentFigure is Plot2D)
            {
                if (arg is PLString)
                {
                    string str = (arg as PLString).Data;

                    if (str == "on")  (CurrentFigure as Plot2D).RectangularGridOn = true;
                    if (str == "off") (CurrentFigure as Plot2D).RectangularGridOn = false;
                }
            }

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
