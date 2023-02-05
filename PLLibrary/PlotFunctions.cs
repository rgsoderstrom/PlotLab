using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

using PLCommon;
using Plot2D_Embedded;
using Plot3D_Embedded;
using PlottingLib;

//***************************************************************************************************

//
// Public interface for plotting functions
//

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        // holds 2D, 3D and PlotFigure (i.e. not yet assigned 2D or 3D) windows
        static readonly List<IPlotCommon> Figures = new List<IPlotCommon> ();

        static internal IPlotCommon CurrentFigure;

        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //
        static public Dictionary<string, PLFunction> GetFunctionContents ()
        {
            return new Dictionary<string, PLFunction>
            {
                {"plot",         Plot},
                {"text",         Text},
                {"PlotVector",   PlotVector2D},
                {"PlotVector2D", PlotVector2D},
                {"PlotVector3D", PlotVector3D},
                {"quiver",       PlotVectorField},
                {"arrow",        PlotVector2D},
                {"contour",      ContourPlotFunctions.ContourPlot},
                {"set",          Set},
                {"axis",         PlotLimits}, // a = axis; 
                {"figure",       Figure},
                {"clf",          ClearFigure},
                {"title",        Title},
                {"PlotCenter",   CameraCenter},
                {"CameraCenter", CameraCenter},
                {"CameraAbsPos", CameraPosition},
                {"CameraRelPos", CameraRelPosition},
            };
        }

        // functions that can be invoked with no arguments
        static public void GetZeroArgNames (List<string> names)
        {
            names.Add ("axis");
            names.Add ("figure");
            names.Add ("clf");
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        /// <summary>
        /// Plot function - entry point for most 2D and 3D plotting functions
        ///      - usage:
        ///          h = plot (dataArg1, dataArg2, ..., formatArg1, formatArg2, ...);
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        
        //
        //
        //      - format args
        //          - optional, strings or numbers
        //          - all dataArgs must preceed first formatArg
        //          - first formatArg must be a string
        //

        static public PLDisplayObject Plot (PLVariable input)
        {
            //
            // separate data arguments and formatting arguments
            //
            PLList dataArgs = new PLList ();
            PLList formatArgs = new PLList ();

            if (input is PLMatrix)
            {
                dataArgs.Add (input);
            }
    
            else if (input is PLList)
            {
                PLList lst = input as PLList;
                int i;

                for (i=0; i<lst.Count; i++)
                {
                    if (lst [i] is PLString) // the first string indicates the start of formatting args
                        break;

                    dataArgs.Add (lst [i]);
                }

                for ( ; i<lst.Count; i++) // add all remaining to format list
                    formatArgs.Add (lst [i]);
            }

            else
                throw new Exception ("Plot function argument error");

            //EventLog.WriteLine ("----------------------------------------------");
            //EventLog.WriteLine (string.Format ("{0} data args, {1} fmt args", dataArgs.Count, formatArgs.Count));
           
            return Plot (dataArgs, formatArgs);
        }

        //******************************************************************************************

        // Parse format arguments and call further processng based on the number of data arguments

        static private PLDisplayObject Plot (PLList dataArgs, PLList formatArgs)
        {
            DrawingParameters dp = ParseDrawingStyle (formatArgs);

            if (dataArgs.Count == 1)
                return Plot1 (dataArgs, dp);

            else if (dataArgs.Count == 2)
                return Plot2 (dataArgs, dp);

            else if (dataArgs.Count == 3)
                return Plot3 (dataArgs, dp);

            else
                throw new Exception ("Plot function data argument count error");
        }

        //******************************************************************************************

        /// <summary>
        /// Plot1 - called with a single list containing the items to be drawn
        /// </summary>
        /// <param name="dataArgs"></param>
        /// <param name="dp"></param>
        /// <returns>PLDisplayObject</returns>
        /// 
        static private PLDisplayObject Plot1 (PLList dataArgs, DrawingParameters dp)
        {
            if (dataArgs [0] is PLMatrix)
            {
                PLMatrix mat = dataArgs [0] as PLMatrix;

                List<Point>   points   = new List<Point> ();
                List<Point3D> points3D = new List<Point3D> ();

                if (mat.IsColVector || mat.IsRowVector) // plot mat content as "y" data
                {
                    if (mat.IsRowVector) // plot passed-in data as "y", generate "x" here
                    {
                        for (int i=0; i<mat.Cols; i++)
                            points.Add (new Point (i + 1, mat [0, i]));
                    }
                    else // is a column vector
                    {
                        if (mat.Rows == 2) // a single 2D point
                        {
                            points.Add (new Point (mat [0, 0], mat [1, 0]));
                        }

                        else if (mat.Rows == 3) // a single 3D point
                        {
                            points3D.Add (new Point3D (mat [0, 0], mat [1, 0], mat [2, 0]));
                        }

                        else // plot passed-in data as "y", generate "x" here
                        {
                            for (int i=0; i<mat.Rows; i++)
                                points.Add (new Point (i + 1, mat [i, 0]));
                        }
                    }
                }

                else if (mat.Rows == 2) // plot each column as a 2D point
                {
                    for (int i=0; i<mat.Cols; i++)
                        points.Add (new Point (mat [0, i], mat [1, i]));
                }

                else if (mat.Rows == 3) // plot each column as a 3D point
                {
                    for (int i=0; i<mat.Cols; i++)
                        points3D.Add (new Point3D (mat [0, i], mat [1, i], mat [2, i]));
                }

                else
                    throw new Exception ("Plot1 arg error");

                //
                // continue with either 2D or 3D points
                //
                if (points.Count > 0) return Plot1 (points, dp);
                else                  return Plot1 (points3D, dp);
            }

            throw new NotImplementedException ("Plot1 argument type error");
        }

        //******************************************************************************************

        // Plot a list of 2D points, either as a connected line or separate points

        /// <summary>
        /// Plot1 - called with a list of 2D points to be drawn
        /// </summary>
        /// <param name="pts">a list of 2D points</param>
        /// <param name="dp - drawing parameters"></param>
        /// <returns>PLCanvasObject cast to the abstract base class PLDisplayObject</returns>
        /// 

        static private PLDisplayObject Plot1 (List<Point> pts, DrawingParameters dp)
        {
            if (CurrentFigure == null)
                NewFigure2D ();       
            
            else if (CurrentFigure is Plot3D)
                NewFigure2D ();

            else if (CurrentFigure is PlotFigure)
                NewFigure2D (CurrentFigure as PlotFigure);

            CanvasObject drawObj;

            // if no style specified plot as a line unless there is only one point
            if (dp.lineStyle == LineView.DrawingStyle.None && dp.pointStyle == PointView.DrawingStyle.None)
            {
                if (pts.Count > 1)
                {
                    drawObj = new LineView (pts);
                    (drawObj as LineView).LineStyle = LineView.DrawingStyle.Solid;
                    (drawObj as LineView).Color = dp.color;
                    (drawObj as LineView).Thickness = dp.lineWidth;
                }

                else
                {
                    drawObj = new PointView (pts, PointView.DrawingStyle.Star);
                    (drawObj as PointView).Color = dp.color;
                    (drawObj as PointView).Size = dp.radius * 2;
                    (drawObj as PointView).Thickness = dp.lineWidth;
                }
            }

            else if (dp.lineStyle != LineView.DrawingStyle.None)
            {
                drawObj = new LineView (pts);
                (drawObj as LineView).LineStyle = dp.lineStyle;
                (drawObj as LineView).Color = dp.color;
                (drawObj as LineView).Thickness = dp.lineWidth;
            }

            else
            {
                drawObj = new PointView (pts, dp.pointStyle);
                (drawObj as PointView).Color = dp.color;
                (drawObj as PointView).Size = dp.radius * 2;
                (drawObj as PointView).Thickness = dp.lineWidth;
            }

            Draw2DObject (drawObj);
            return new PLCanvasObject (drawObj);
        }

        //******************************************************************************************

        // Plot a list of 3D points. more than one will be connected line, one will be a single point ----- FIX

        /// <summary>
        /// Plot1 - called with a list of 3D points to be drawn
        /// </summary>
        /// <param name="dataArgs"></param>
        /// <param name="dp"></param>
        /// <returns>PLDisplayObject</returns>
        /// 

        static private PLDisplayObject Plot1 (List<Point3D> pts, DrawingParameters dp)
        {
            if (CurrentFigure == null)
                NewFigure3D ();
            
            else if (CurrentFigure is Plot2D)
                NewFigure3D ();

            else if (CurrentFigure is PlotFigure)
                NewFigure3D (CurrentFigure as PlotFigure);

            ViewportObject vo;

            if (pts.Count > 1)
            {
                if (dp.pointStyle == PointView.DrawingStyle.None)
                {
                    Polyline3D pl3;

                    switch (dp.lineStyle)
                    {
                        case LineView.DrawingStyle.None:
                        case LineView.DrawingStyle.Solid:
                        {
                            pl3 = new Polyline3D (pts);
                        }
                        break;

                        case LineView.DrawingStyle.Dots:
                        {
                            pl3 = new Polyline3D (CommonMath.Interpolation.Linear (pts, 32));
                            pl3.PolylineView.Decimation = 8; 
                        }
                        break;

                        case LineView.DrawingStyle.Dashes:
                        {
                            pl3 = new Polyline3D (CommonMath.Interpolation.Linear (pts, 32));
                            pl3.PolylineView.Decimation = 4; 
                        }
                        break;

                        case LineView.DrawingStyle.LongDashes:
                        {
                            pl3 = new Polyline3D (CommonMath.Interpolation.Linear (pts, 8));
                            pl3.PolylineView.Decimation = 2; 
                        }
                        break;

                        default:
                            pl3 = new Polyline3D (pts);
                            break;
                    }

                    pl3.PolylineView.Thickness = dp.lineWidth;
                    pl3.PolylineView.Color = dp.color.Color;
                    vo = pl3;
                }

                else // draw as points
                {
                    PointCloud3D pc = new PointCloud3D (pts);
                    vo = pc;
                    pc.PointView.Color = (dp.color as SolidColorBrush).Color;
                    pc.PointView.Diameter = 2 * dp.radius;
                }
                
                //pv.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
                //pv.ArrowLength = 5;
                //pv.Decimation = 1;
            }

            else // single point
            { 
                PointMarker pp = new Sphere (pts [0]);
                vo = pp;
               
                pp.Color = (dp.color as SolidColorBrush).Color;
                pp.Radius = dp.radius;
            }

            (CurrentFigure as Plot3D).Plot (vo);

            return new PLViewportObject (vo);
        }

        //******************************************************************************************

        // Plot2 - two data arguments

        static private PLDisplayObject Plot2 (PLList dataArgs, DrawingParameters dp)
        {
            List<PLVariable> args = dataArgs.Data;
            List<Point> pts = new List<Point> ();

            if (args [0] is PLMatrix && args [1] is PLMatrix)
            {
                PLMatrix m1 = args [0] as PLMatrix;
                PLMatrix m2 = args [1] as PLMatrix;

                if (m1.Rows == m2.Rows && m1.Cols == m2.Cols)
                {
                    if (m1.IsRowVector)
                    {
                        for (int i=0; i<m1.Cols; i++)
                            pts.Add (new Point (m1 [0, i], m2 [0, i]));
                    }

                    else
                    {
                        for (int i=0; i<m1.Rows; i++)
                            pts.Add (new Point (m1 [i, 0], m2 [i, 0]));
                    }
                }

                else
                    throw new Exception ("Plot error: x & y arg sizes must match");
            }

            else if (args [0] is PLDouble && args [1] is PLDouble)
            {
                double x = (args [0] as PLDouble).Data;
                double y = (args [1] as PLDouble).Data;
                pts.Add (new Point (x, y));
            }

            else
                throw new NotImplementedException ("Plot2 argument type error");

            return Plot1 (pts, dp);
        }

        //******************************************************************************************

        // Plot3 - three data args

        static private PLDisplayObject Plot3 (PLList dataArgs, DrawingParameters dp)
        {
            List<PLVariable> args = dataArgs.Data;
            List<Point3D> pts = new List<Point3D> ();

            if (args [0] is PLMatrix && args [1] is PLMatrix && args [2] is PLMatrix)
            {
                PLMatrix m1 = args [0] as PLMatrix;
                PLMatrix m2 = args [1] as PLMatrix;
                PLMatrix m3 = args [2] as PLMatrix;

                if (m1.IsVector == false || m2.IsVector == false)
                    throw new Exception ("Plot data args - first two must both be row or column vectors");

                if (m3.IsVector) // then plot a line
                {
                    int count = m1.Size;

                    if (m2.Size != count || m3.Size != count)
                        throw new Exception ("Plot data args - vectors must be same size");

                    if (m1.IsRowVector && m2.IsRowVector && m3.IsRowVector)
                    {
                        for (int i=0; i<count; i++)
                            pts.Add (new Point3D (m1 [0, i], m2 [0, i], m3 [0, i]));
                    }

                    else if (m1.IsColVector && m2.IsColVector && m3.IsColVector)
                    {
                        for (int i=0; i<count; i++)
                            pts.Add (new Point3D (m1 [i, 0], m2 [i, 0], m3 [i, 0]));
                    }

                    else
                        throw new Exception ("Plot data args - vectors must be same orientation, i.e. all row or all column");
                }

                else // draw a surface
                {
                    List<double> xCoords = m1.ReadByColumn ();
                    List<double> yCoords = m2.ReadByColumn ();
                    double [,] zValues   = m3.Data.Data;

                    ZFunctionOfXY ZSurface = new ZFunctionOfXY (xCoords, yCoords, zValues);
                    
                    if (CurrentFigure is Plot3D == false)
                        NewFigure3D ();

                    Surface3DView sv = (CurrentFigure as Plot3D).Plot (ZSurface) as Surface3DView;
                    //sv.BackColor = Colors.PeachPuff;
                    //sv.BackOpacity = 0.5;
                    ZSurface.ShowTraceLines = true;

                    PLViewportObject pdo = new PLViewportObject (ZSurface as ViewportObject);
                    return pdo;
                }
            }

            else if (args [0] is PLDouble && args [1] is PLDouble && args [2] is PLDouble)
            {
                double x = (args [0] as PLDouble).Data;
                double y = (args [1] as PLDouble).Data;
                double z = (args [2] as PLDouble).Data;
                pts.Add (new Point3D (x, y, z));
            }
             
            else
                throw new NotImplementedException ("Plot3 argument error");
    
            return Plot1 (pts, dp);
        }

        //******************************************************************************************

        /// <summary>
        /// NewFigureCommon - creates a new figure and sets it as "current". Adds Closed and Activated handlers
        /// </summary>
        /// <param name="fig">Plot2D or Plot3D</param>
        /// 
        static private void NewFigureCommon (IPlotCommon fig)
        {
            (fig as Window).Closed += Fig_Closed;
            (fig as Window).Activated += Fig_Activated; // when clicked to foreground
            Figures.Add (fig);
            CurrentFigure = fig;
        }

        static public void NewFigure ()
        {
            NewFigureCommon (new PlotFigure ());
        }

        static public void NewFigure2D ()
        {
            NewFigureCommon (new Plot2D ());
            //(CurrentFigure as Plot2D).Title += " - Plot2D";
        }

        static public void NewFigure2D (PlotFigure pf)
        {
            NewFigureCommon (new Plot2D (pf));
            pf.Close ();
            //(CurrentFigure as Plot2D).Title += " - Plot2D";
        }

        static public void NewFigure3D ()
        {
            NewFigureCommon (new Plot3D ());
            //(CurrentFigure as Plot3D).Title += " - Plot3D";
        }

        static public void NewFigure3D (PlotFigure pf)
        {
            NewFigureCommon (new Plot3D (pf));
            pf.Close ();
            //(CurrentFigure as Plot3D).Title += " - Plot3D";
        }

        static void Fig_Activated (object sender, EventArgs e)
        {
            if (sender is Plot2D)      {CurrentFigure = sender as Plot2D;}
            else if (sender is Plot3D) {CurrentFigure = sender as Plot3D;}
            else CurrentFigure = sender as PlotFigure;
        }

        static void Fig_Closed (object sender, EventArgs e)
        {
            IPlotCommon closedFig = sender as IPlotCommon;
            if (CurrentFigure == closedFig) CurrentFigure = null;
            Figures.Remove (closedFig);
        }

        //****************************************************************************************

        static void CloseAll ()
        {
            for (int i=Figures.Count - 1; i>=0; i--)
                (Figures [i] as Window).Close ();

            CurrentFigure = null;
        }

        static void CloseOne (int fig)
        {
            foreach (IPlotCommon ipc in Figures)
            {
                if (ipc.ID == fig)
                {
                    (ipc as Window).Close ();
                    break;
                }
            }
        }
    }
}
