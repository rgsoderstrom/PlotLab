using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PlottingLib;
using System.Windows.Documents;

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        //*********************************************************************************************
        //
        // map command strings to executable functions
        //

        static public void AddPlotCommands (ref Dictionary<string, BSFunction> dst)
        {
            dst.Add ("close",  CloseFigure);
            dst.Add ("hold",   Hold);
            dst.Add ("axis",   AxisConstraints);
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // "axis" handler for some arguments

        // argument formats:
        //  axis equal       - adjust limits

        static bool AxisConstraints (string arg)
        {
            if (CurrentFigure == null)
                return false;

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return false;

            switch (arg)
            {
                case "tight":
                    fig.AxesTight = true;
                    break;

                case "equal":
                    fig.AxesEqual = true;
                    break;

                case "frozen":
                    fig.AxesFrozen = true;
                    break;

                case "auto":
                    (fig as IPlotCommon).Hold = true;
                    fig.AxesFrozen = false;
                    fig.AxesTight  = true;
                    fig.AxesEqual  = false;
                    break;

                default:
                    throw new Exception ("Axis command - unrecognized option");
            }

            return true;
        }

        //*********************************************************************************************

        static PLVariable Title (PLVariable ptxt)
        {
            if (ptxt is PLString)
            { 
                string txt = (ptxt as PLString).Text;

                if (CurrentFigure == null)
                    return new PLBool (false);

                if (CurrentFigure is PlotFigure)
                    (CurrentFigure as PlotFigure).DataAreaTitle = txt;

                IPlotDrawable fig = CurrentFigure as IPlotDrawable;

                if (fig == null)
                    return new PLBool (false);

                //if (fig is PlotFigure)
                //    (fig as PlotFigure).DataAreaTitle = txt;

                if (fig is Plot2D)
                    (fig as Plot2D).DataAreaTitle = txt;

                if (fig is Plot3D)
                    (fig as Plot3D).DataAreaTitle = txt;
            }

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable XLabel (PLVariable ptxt)
        {
            if (CurrentFigure == null)
                return new PLNull ();

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return new PLNull ();

            if (ptxt is PLString)
            { 
                string txt = (ptxt as PLString).Text;

                if (fig is Plot2D)
                    (fig as Plot2D).XAxisLabel = txt;
            }

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable YLabel (PLVariable ptxt)
        {
            if (CurrentFigure == null)
                return new PLNull ();

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return new PLNull ();

            if (ptxt is PLString)
            { 
                string txt = (ptxt as PLString).Text;

                if (fig is Plot2D)
                    (fig as Plot2D).YAxisLabel = txt;
            }

            return new PLNull ();
        }

        //*********************************************************************************************

        // called in response to clf

        static PLVariable ClearFigure ()
        {
            if (CurrentFigure == null)
                NewFigure ();

            else if (CurrentFigure is IPlotDrawable)
                (CurrentFigure as IPlotDrawable).Clear ();

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable Figure ()
        {
            NewFigure ();
            return new PLInteger (CurrentFigure.ID);
        }

        static PLVariable Figure (PLVariable arg)
        {
            int figNumber;

            PLDouble  dbl  = arg as PLDouble;
            PLInteger intr = arg as PLInteger;
            if (dbl == null && intr == null) throw new Exception ("Figure command argument error");

            int requestedFigNumber = intr == null ? (int) dbl.Data : intr.Data;
            bool found = false;

            // look for that id. if found make it the current figure
            foreach (Window w in Figures)
            {
                // see if "w" is a PlotFigure (i.e. not assigned to 2D or 3D yet)
                PlotFigure pf = w as PlotFigure; if (pf != null) {if (pf.ID == requestedFigNumber) {found = true; CurrentFigure = pf; break;}}

                // see if it's a Plot2D
                Plot2D p2 = w as Plot2D; if (p2 != null) {if (p2.ID == requestedFigNumber) {found = true; CurrentFigure = p2; break;}}

                // or a Plot3D
                Plot3D p3 = w as Plot3D; if (p3 != null) {if (p3.ID == requestedFigNumber) {found = true; CurrentFigure = p3; break;}}
            }

            if (found) // pull it to front
            {
                (CurrentFigure as Window).Topmost = true;
                (CurrentFigure as Window).Topmost = false;
            }

            else // not found, so make a new figure and assign that id                
            {
                NewFigure ();
                (CurrentFigure as Window).Title = "Figure " + requestedFigNumber.ToString ();
                CurrentFigure.ID = requestedFigNumber;
            }

            figNumber = requestedFigNumber;

            return new PLInteger (figNumber);
        }

        //*********************************************************************************************

        static bool Hold (string arg)
        {
            if (arg.Length == 0)
            { 
                if (CurrentFigure != null)
                    CurrentFigure.Hold = true;

                return true;
            }

            if (arg == "on")
                CurrentFigure.Hold = true;

            else if (arg == "off")
                CurrentFigure.Hold = false;

            else
                throw new Exception ("Unrecognized argument for Hold command");

            return true;
        }

        //*********************************************************************************************

        static bool CloseFigure (string arg)
        {
            if (arg.Length == 0)
            { 
                if (CurrentFigure != null)
                    (CurrentFigure as Window).Close ();

                return true;
            }

            string [] tokens = arg.Split (new char [] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
            {
                if (CurrentFigure != null)
                    (CurrentFigure as Window).Close ();
                
                return true;
            }

            if (tokens [0] == "all")
            { 
                CloseAll ();
                return true;
            }

            try 
            {
                int fig = Convert.ToInt32 (tokens [0]);
                CloseOne (fig);
            }

            catch (Exception)
            { 
                throw new Exception ("Close - invalid args");
            }

            return true;
        }
    }
}
