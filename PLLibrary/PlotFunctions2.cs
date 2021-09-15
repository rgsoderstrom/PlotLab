using System;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plot2D_Embedded;
using PlottingLib;
using PLCommon;

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        //***********************************************************************************************************
        //
        // SetCanvasObject - change a CanvasObject property 
        //              

        static PLVariable SetCanvasObject (PLList args)
        {
            PLCanvasObject singleCanvasObject = args [0] as PLCanvasObject;
            LineView  lv = singleCanvasObject.Data as LineView;
            PointView pv = singleCanvasObject.Data as PointView;

            PLString       param = args [1] as PLString;
            if (param == null) throw new Exception ("Set function argument error");
           
            if (lv != null)
            {
                switch (param.Text)
                {
                    case "Thickness":
                    case "thickness": if (args [2] is PLDouble)  lv.Thickness = (args [2] as PLDouble).Data; break;

                    default:
                        throw new Exception ("Unrecognized set option - " + param.Data);
                }
            }

            else if (pv != null)
            {
                switch (param.Text)
                {
                    case "Thickness":
                    case "thickness": if (args [2] is PLDouble)  pv.BorderThickness = (args [2] as PLDouble).Data; break;

                    case "Size":
                    case "size": if (args [2] is PLDouble)  pv.Size = (args [2] as PLDouble).Data; break;

                    default:
                        throw new Exception ("Unrecognized set option - " + param.Data);
                }
            }

            return new PLNull ();            
        }

        static PLVariable SetFigure (PLList args)
        {
            PLInteger pli = args [0] as PLInteger;
            PLDouble pld = args [0] as PLDouble;
            if (pli == null && pld == null) throw new Exception ("SetFigure argument error");
            int requestedFigNumber = (int) (pli != null ? pli.Data : pld.Data);

            // look for that figure. 
            Window fig = null;

            foreach (Window w in Figures)
            {
                Plot2D p2 = w as Plot2D; if (p2 != null) {if (p2.ID == requestedFigNumber) {fig = w; break;}}
                Plot3D p3 = w as Plot3D; if (p3 != null) {if (p3.ID == requestedFigNumber) {fig = w; break;}}
                PlotFigure pf = w as PlotFigure; if (pf != null) {if (pf.ID == requestedFigNumber) {fig = w; break;}}
            }

            if (fig != null)
            {
                PLString pstr = args [1] as PLString;
                if (pstr == null) throw new Exception ("SetFigure error - 2nd arg must be a string");

                PLDouble pdbl = args [2] as PLDouble;
                PLMatrix pmat = args [2] as PLMatrix;

                switch (pstr.Text)
                {
                    case "Left":
                    case "left":
                        if (pdbl != null) fig.Left = pdbl.Data; else throw new Exception ("SetFigure error - 3rd arg error");
                        break;

                    case "Top":
                    case "top":
                        if (pdbl != null) fig.Top = pdbl.Data; else throw new Exception ("SetFigure error - 3rd arg error");
                        break;

                    case "Width":
                    case "width":
                        if (pdbl != null) fig.Width = pdbl.Data; else throw new Exception ("SetFigure error - 3rd arg error");
                        break;

                    case "Height":
                    case "height":
                        if (pdbl != null) fig.Height = pdbl.Data; else throw new Exception ("SetFigure error - 3rd arg error");
                        break;

                    case "Position":
                    case "position":
                        if (pmat != null)
                        {
                            if (pmat.Rows != 1 || pmat.Cols != 4) throw new Exception ("SetFigure error - 3rd arg must be 1x4 row vector");
                            fig.Left   = pmat.Data [0, 0];
                            fig.Top    = pmat.Data [0, 1];
                            fig.Width  = pmat.Data [0, 2];
                            fig.Height = pmat.Data [0, 3];
                        }
                        break;

                    default:
                        throw new Exception ("SetFigure error - unrecognized option: " + pstr.Data);
                }
            }

            return new PLNull ();            
        }
    }
}
