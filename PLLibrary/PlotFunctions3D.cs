using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Petzold.Media3D;

using PLCommon;
using Plot2D_Embedded;
using Plot3D_Embedded;
using PlottingLib;

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        //******************************************************************************************

        static void Draw3DObject (ViewportObject vo)
        {
            if (CurrentFigure == null)
                NewFigure3D ();

            if (CurrentFigure is PlotFigure)
                NewFigure3D (CurrentFigure as PlotFigure);

            if (CurrentFigure is Plot2D)
                NewFigure3D ();

            Plot3D fig = CurrentFigure as Plot3D;

            //fig.RectangularGridOn = true;
            fig.Plot (vo);
        }

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

        static public PLVariable PlotVector3D (PLVariable input)
        {
            // defaults
            Point3D tail = new Point3D (0, 0, 0);
            Vector3D vect = new Vector3D (1, 0, 0);
            List<Vector3D> basis = new List<Vector3D> () {new Vector3D (1, 0, 0), new Vector3D (0, 1, 0), new Vector3D (0, 0, 1)};
            string style = "k";
            Line3DView.LineStyles LineStyle = Line3DView.LineStyles.Solid;
            Color LineColor = Colors.Black; // default color
            bool showComponents = false;

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
                    PLRMatrix mat = lst [0] as PLRMatrix;
                    if (mat == null) throw new Exception ("PlotVector arg1 must be col vector");
                    if (mat.Rows != 3 || mat.Cols != 1) throw new Exception ("PlotVector arg1 must be 3D col vector");
                    vect = new Vector3D (mat [0, 0], mat [1, 0], mat [2, 0]);
                }

                else if (dataArgCount == 2)
                {
                    PLRMatrix mat1 = lst [0] as PLRMatrix;
                    PLRMatrix mat2 = lst [1] as PLRMatrix;
                    if (mat1 == null || mat2 == null) throw new Exception ("PlotVector arg1 error");

                    if (mat2.Cols == 1 && mat2.Rows == 3) // 2nd arg is a vector
                    {
                        tail = new Point3D  (mat1 [0, 0], mat1 [1, 0], mat1 [2, 0]);
                        vect = new Vector3D (mat2 [0, 0], mat2 [1, 0], mat2 [2, 0]);
                    }
                    else if (mat2.Cols == 2 && mat2.Rows == 2) // 2nd arg is a matrix
                    {
                        vect = new Vector3D (mat1 [0, 0], mat1 [1, 0], mat1 [2, 0]);
                        basis.Clear ();
                        basis.Add (new Vector3D (mat2 [0, 0], mat2 [1, 0], mat2 [2, 0]));
                        basis.Add (new Vector3D (mat2 [0, 1], mat2 [1, 1], mat2 [2, 0]));
                        basis.Add (new Vector3D (mat2 [0, 2], mat2 [1, 2], mat2 [2, 2]));
                    }
                    else
                        throw new Exception ("PlotVector arg1 error");
                }

                else if (dataArgCount == 3)
                {
                    PLRMatrix mat1 = lst [0] as PLRMatrix;
                    PLRMatrix mat2 = lst [1] as PLRMatrix;
                    PLRMatrix mat3 = lst [2] as PLRMatrix;
                    if (mat1 == null || mat2 == null || mat3 == null) throw new Exception ("PlotVector arg1 error");

                    if (mat1.IsColVector == false) throw new Exception ("PlotVector 1st arg must be column vector");
                    if (mat2.IsColVector == false) throw new Exception ("PlotVector 2nd arg must be column vector");
                    if (mat3.IsMatrix    == false) throw new Exception ("PlotVector 3rd arg must be matrx");

                    if (mat1.Rows != 3) throw new Exception ("PlotVector 1st arg must be column vector, length 3");
                    if (mat2.Rows != 3) throw new Exception ("PlotVector 2nd arg must be column vector, length 3");
                    if (mat3.Rows != 3 && mat3.Cols != 3) throw new Exception ("PlotVector 3rd arg must be 3x3 matrix");

                    tail = new Point3D  (mat1 [0, 0], mat1 [1, 0], mat1 [2, 0]);
                    vect = new Vector3D (mat2 [0, 0], mat2 [1, 0], mat2 [2, 0]);

                    basis.Clear ();
                    basis.Add (new Vector3D (mat3 [0, 0], mat3 [1, 0], mat3 [2, 0]));
                    basis.Add (new Vector3D (mat3 [0, 1], mat3 [1, 1], mat3 [2, 1]));
                    basis.Add (new Vector3D (mat3 [0, 2], mat3 [1, 2], mat3 [2, 2]));
                }

                else
                    throw new Exception ("PlotVector - too many arguments");

                // style setting
                foreach (char c in style)
                {
                    switch (c)
                    {
                        case 'r': LineColor = Colors.Red; break;
                        case 'o': LineColor = Colors.Orange; break;
                        case 'y': LineColor = Colors.Yellow; break;
                        case 'g': LineColor = Colors.Green; break;
                        case 'b': LineColor = Colors.Blue; break;
                        case 'k': LineColor = Colors.Black; break;

                        // single '-' => short dash
                        // double "--" => long dash
                        case '-': if (LineStyle == Line3DView.LineStyles.Solid) LineStyle = Line3DView.LineStyles.ShortDash; 
                                  else                                          LineStyle = Line3DView.LineStyles.LongDash; break;

                        case '.': LineStyle = Line3DView.LineStyles.Dot; break;
                        case 'c': showComponents = true; break;
                    }
                }
            }

            else if (input is PLRMatrix mat) // column vector only
            {
                if (mat == null) throw new Exception ("PlotVector arg1 must be col vector");
                if (mat.Rows != 3 || mat.Cols != 1) throw new Exception ("PlotVector arg1 must be 3D col vector");
                vect = new Vector3D (mat [0, 0], mat [1, 0], mat [2, 0]);
            }

            else
                throw new Exception ("PlotVector argument error");




            // DOESN'T USE BASIS


           

            Line3D l3d = new Line3D (tail, vect);
            Draw3DObject (l3d);

            l3d.LineView.Color = LineColor;
            l3d.LineView.ArrowEnds = Petzold.Media2D.ArrowEnds.End;
            l3d.LineView.LineStyle = LineStyle;

            PLList ll = new PLList ();
            PLViewportObject vo = new PLViewportObject (l3d);
            ll.Add (vo);

            if (showComponents == true)
            {
                Line3D lx = new Line3D (new Point3D (0, 0, 0),           new Vector3D (vect.X, 0, 0));
                Line3D ly = new Line3D (new Point3D (vect.X, 0, 0),      new Vector3D (0, vect.Y, 0));
                Line3D lz = new Line3D (new Point3D (vect.X, vect.Y, 0), new Vector3D (0, 0, vect.Z));
                lx.LineView.Color = ly.LineView.Color = lz.LineView.Color = Colors.LightGray;
                Draw3DObject (lx);
                Draw3DObject (ly);
                Draw3DObject (lz);
            }

            return ll;
        }


        //*********************************************************************************************

        // Utility to convert function args to a single Point3D

        static private Point3D ExtractPoint3D (PLVariable arg)
        {
            double x = double.NaN, y = double.NaN, z = double.NaN;
            
            if (arg is PLList)
            {
                PLList lst = arg as PLList;

                if (lst.Count != 3)
                    throw new Exception ("Arg must be 3 coordinate numbers");

                if (lst [0] is PLRMatrix) {PLRMatrix mx = (PLRMatrix)lst [0]; x = mx [0, 0]; }
                if (lst [0] is PLDouble) {PLDouble mx = (PLDouble)lst [0]; x = mx.Data; }
                if (lst [1] is PLRMatrix) {PLRMatrix my = (PLRMatrix)lst [1]; y = my [0, 0]; }
                if (lst [1] is PLDouble) {PLDouble my = (PLDouble)lst [1]; y = my.Data; }
                if (lst [2] is PLRMatrix) {PLRMatrix mz = (PLRMatrix)lst [2]; z = mz [0, 0]; }
                if (lst [2] is PLDouble) {PLDouble mz = (PLDouble)lst [2]; z = mz.Data; }

                if (x == double.NaN || y == double.NaN || z == double.NaN)
                    throw new Exception ("Arg must be 3 coordinate numbers");
            }

            else if (arg is PLRMatrix)
            {
                PLRMatrix mat = arg as PLRMatrix;

                if (mat.Rows == 1 && mat.Cols == 3)      { x = mat [0, 0]; y = mat [0, 1]; z = mat [0, 2]; }
                else if (mat.Rows == 3 && mat.Cols == 1) { x = mat [0, 0]; y = mat [1, 0]; z = mat [2, 0]; }
                else throw new Exception ("Center - arg must be 3 coordinate numbers");
            }

            else
                throw new Exception ("CameraCenter - Unrecognized arg");

            return new Point3D (x, y, z);
        }

        //*********************************************************************************************

        static PLVariable CameraCenter (PLVariable arg)
        {
            if (CurrentFigure is Plot3D == false)
                throw new Exception ("Figure is not a Plot3D");

            (CurrentFigure as Plot3D).CenterOn (ExtractPoint3D (arg));
            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable CameraPosition (PLVariable arg)
        {
            if (CurrentFigure is Plot3D == false)
                throw new Exception ("Figure is not a Plot3D");

            (CurrentFigure as Plot3D).CameraPosition (ExtractPoint3D (arg));
            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable CameraRelPosition (PLVariable arg)
        {
            if (CurrentFigure is Plot3D == false)
                throw new Exception ("Figure is not a Plot3D");

            (CurrentFigure as Plot3D).CameraRelPosition (ExtractPoint3D (arg));
            return new PLNull ();
        }

    }
}


