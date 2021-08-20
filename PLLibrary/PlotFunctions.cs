using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

using PLCommon;
using Plot2D_Embedded;
using Plot3D_Embedded;
using PlottingLib;

//***************************************************************************************************

namespace PLCommon
{
    public class PLCanvasObject : PLScalar
    {
        readonly public Plot2D_Embedded.CanvasObject Data;

        public PLCanvasObject (CanvasObject co)
        {
            Data = co;
        }

        public override string ToString (string fmt)
        {
            return "";
        }
    }

    public class PLViewportObject : PLScalar
    {
        readonly public Plot3D_Embedded.ViewportObject Data;

        public PLViewportObject (ViewportObject co)
        {
            Data = co;
        }

        public override string ToString (string fmt)
        {
            return "";
        }
    }
}

//***************************************************************************************************

//
// Public interface for plotting functions
//

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        //
        // holds 2D, 3D and PlotFigure (i.e. not yet assigned 2D or 3D) windows
        //
        static readonly List<IPlotCommon> Figures = new List<IPlotCommon> ();

        //
        // current active figure
        //
        //static Plot2D Fig2D = null;
        //static Plot3D Fig3D = null;

        static IPlotCommon CurrentFigure;

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
                {"PlotVector3D", PlotVector3D},
                {"quiver",       PlotVectorField},
                {"arrow",        PlotVector2D},
                {"contour",      ContourPlot},
                {"set",          Set},
                {"axis",         PlotLimits}, // a = axis; 
                {"figure",       Figure},
                {"clf",          ClearFigure},
                {"title",        Title},
            };
        }

        // functions that can be invoked with no arguments
        static public void GetZeroArgNames (List<string> names)
        {
            names.Add ("axis");
            names.Add ("figure");
            names.Add ("clf");
        }

        //******************************************************************************************

        static private void  NewFigureCommon (IPlotCommon fig)
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
            (CurrentFigure as Plot2D).Title += " - Plot2D";
        }

        static public void NewFigure2D (PlotFigure pf)
        {
            NewFigureCommon (new Plot2D (pf));
            pf.Close ();
            (CurrentFigure as Plot2D).Title += " - Plot2D";
        }






        static public void NewFigure3D ()
        {
            NewFigureCommon (new Plot3D ());
            //Plot3D Fig3D = new Plot3D ();
            (CurrentFigure as Plot3D).Title += " - Plot3D";
            //Fig3D.Closed    += Fig_Closed;
            //Fig3D.Activated += Fig_Activated; // when clicked to foreground
            //Figures.Add (Fig3D);
            //CurrentFigure = Fig3D;
        }

        static public void NewFigure3D (PlotFigure pf)
        {
            NewFigureCommon (new Plot3D (pf));
            pf.Close ();
            //Plot3D Fig3D = new Plot3D (pf);
            (CurrentFigure as Plot3D).Title += " - Plot3D";
            //Fig3D.Closed    += Fig_Closed;
            //Fig3D.Activated += Fig_Activated; // when clicked to foreground
            //Figures.Add (Fig3D);
            //CurrentFigure = Fig3D;
        }





        static void Fig_Activated (object sender, EventArgs e)
        {
            if (sender is Plot2D)      {CurrentFigure = sender as Plot2D;}
            else if (sender is Plot3D) {CurrentFigure = sender as Plot3D;}
            else CurrentFigure = sender as PlotFigure;
        }



        static bool IsPlot2D (IPlotCommon ipc) {return ipc is Plot2D;}
        static bool IsPlot3D (IPlotCommon ipc) {return ipc is Plot3D;}


        static void Fig_Closed (object sender, EventArgs e)
        {
            IPlotCommon closedFig = sender as IPlotCommon;
            if (CurrentFigure == closedFig) CurrentFigure = null;
            //if (Fig2D == closedFig) Fig2D = null;
            //if (Fig3D == closedFig) Fig3D = null;

            Figures.Remove (closedFig);

            //if (Fig2D == null) Fig2D = Figures.FindLast (IsPlot2D) as Plot2D;
            //if (Fig3D == null) Fig3D = Figures.FindLast (IsPlot3D) as Plot3D;
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
