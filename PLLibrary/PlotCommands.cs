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

        static public Dictionary<string, PLFunction> GetPlotCommands ()
        {
            return new Dictionary<string, PLFunction> ()
            {
              //{"title",  Title},
              //{"figure", Figure},
              //{"clf",    ClearFigure},
                {"close",  CloseFigure},
                {"hold",   Hold},
            };
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // "axis" handler for some arguments

        // argument formats:
        //  axis equal       - adjust limits
        //  axis ([1 2 3 4]) - set limits
        //  axis             - no arg, return current limits

        static PLVariable AxisConstraints (PLVariable arg)
        {
            if (CurrentFigure == null)
                return new PLNull ();

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return new PLNull ();

            if (arg is PLString str)
            {
                switch (str.Data)
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
            }

            return new PLNull ();
        }

        //*********************************************************************************************

        static string ExtractOneString (PLVariable arg, string name = "")
        {
            PLString str = arg as PLString;
            PLList   lst = arg as PLList;
            PLString tstr;

            if      (str != null) tstr = str;
            else if (lst != null) tstr = lst [0] as PLString;
            else throw new Exception (name + " argument error");

            string txt = tstr.Data;
            if (txt [0] == '\'') txt = txt.Remove (0, 1);
            if (txt [txt.Length - 1] == '\'') txt = txt.Remove (txt.Length - 1);

            // look for embedded quote. remove the preceeding backslash
            int count = 0;
            int i1 = 0, i2 = 0;
            
            while (count++ < 100)
            {
                i1 = txt.IndexOf ('\\', i1);
                i2 = txt.IndexOf ('\'', i2);

                if (i1 >= 0 && i2 >= 0)
                    if (i2 == i1 + 1)
                        txt = txt.Remove (i1, 1);
                    else
                        break;
                else
                    break;
            }

            return txt;
        }

        //*********************************************************************************************

        static PLVariable Title (PLVariable arg)
        {
            if (CurrentFigure == null)
                return new PLNull ();

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return new PLNull ();

            string txt = ExtractOneString (arg, "title");

            if (fig is Plot2D)
                (fig as Plot2D).DataAreaTitle = txt;

            if (fig is Plot3D)
                (fig as Plot3D).DataAreaTitle = txt;

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable XLabel (PLVariable arg)
        {
            if (CurrentFigure == null)
                return new PLNull ();

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return new PLNull ();

            string txt = ExtractOneString (arg, "xlabel");

            if (fig is Plot2D)
                (fig as Plot2D).XAxisLabel = txt;

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable YLabel (PLVariable arg)
        {
            if (CurrentFigure == null)
                return new PLNull ();

            IPlotDrawable fig = CurrentFigure as IPlotDrawable;

            if (fig == null)
                return new PLNull ();

            string txt = ExtractOneString (arg, "ylabel");

            if (fig is Plot2D)
                (fig as Plot2D).YAxisLabel = txt;

            return new PLNull ();
        }

        //*********************************************************************************************

        // called in response to clf

        static PLVariable ClearFigure (PLVariable _)
        {
            if (CurrentFigure == null)
                NewFigure ();

            else if (CurrentFigure is IPlotDrawable)
                (CurrentFigure as IPlotDrawable).Clear ();

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable Figure (PLVariable arg)
        {
            int figNumber;

            if (arg is PLNull)
            {
                NewFigure ();
                figNumber = CurrentFigure.ID;
            }

            else
            {
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
            }

            return new PLInteger (figNumber);
        }

        //*********************************************************************************************

        static PLVariable Hold (PLVariable arg)
        {
            PLList args = arg as PLList;

            if (args == null)
                throw new Exception ("Hold command argument error");

            if (CurrentFigure == null)
                return new PLNull ();

            if (args.Count > 1)
                throw new Exception ("Too many args for Hold command");

            if (args.Count == 0)
                CurrentFigure.Hold = true;

            else
            {
                PLString str = args [0] as PLString;

                if (str == null)
                    throw new Exception ("Hold command argument error");

                else
                {
                    if (str.Data == "on")
                        CurrentFigure.Hold = true;

                    else if (str.Data == "off")
                        CurrentFigure.Hold = false;

                    else
                        throw new Exception ("Unrecognized argument for Hold command");
                }
            }

            return new PLNull ();
        }

        //*********************************************************************************************

        static PLVariable CloseFigure (PLVariable arg)
        {
            PLList pll = arg as PLList;

            if (pll != null)
            {
                if (pll.Count == 0)
                {
                    if (CurrentFigure != null)
                        (CurrentFigure as Window).Close ();
                }

                else if (pll.Count == 1) // 
                {
                    if (pll [0] is PLString)
                    {
                        if ((pll [0] as PLString).Text == "all")
                        {
                            CloseAll ();
                        }

                        else 
                        {
                            int fig = Convert.ToInt32 ((pll [0] as PLString).Text);
                            CloseOne (fig);
                        }
                    }
                }

                else throw new Exception ("Close - invalid args");
            }

            return new PLNull ();
        }
    }
}
