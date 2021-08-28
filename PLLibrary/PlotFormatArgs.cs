﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

using PLCommon;
using Plot2D_Embedded;
using Plot3D_Embedded;
using PlottingLib;

namespace FunctionLibrary
{
    static public partial class PlotFunctions
    {
        //******************************************************************************************

        private class DrawingParameters
        {
            public LineView.DrawingStyle lineStyle;
            public PointView.DrawingStyle pointStyle;
            public Brush color;
            public Brush edgeColor;
            public Brush fillColor;

            public double lineWidth;
            public double radius;

            public DrawingParameters ()
            {
                lineStyle = LineView.DrawingStyle.None;
                pointStyle = PointView.DrawingStyle.None;
                color = edgeColor = fillColor = Brushes.Black;
                lineWidth = 0.25;
                radius = 0.1;
            }
        }

        //******************************************************************************************

        static private DrawingParameters ParseDrawingStyle (PLList formatArgs)
        {
            DrawingParameters drawingParams = new DrawingParameters ();

            try
            {
                int get = 0;
            
                while (get < formatArgs.Count)
                {
                    string arg = (formatArgs [get] as PLString).Data;
                    arg = arg.Replace ("'", ""); // remove all single quotes

                    switch (arg)
                    {
                        case "LineWidth":
                            drawingParams.lineWidth = (formatArgs [++get] as PLDouble).Data;
                            break;

                        case "Color":
                        {
                            string colorString  = (formatArgs [++get] as PLString).Data;
                            drawingParams.color = CharToColor (colorString [0]);
                        }
                        break;

                        case "MarkerFaceColor":
                        {
                            string colorString  = (formatArgs [++get] as PLString).Data;
                            drawingParams.fillColor = CharToColor (colorString [0]);
                        }
                        break;

                        case "MarkerEdgeColor":
                        {
                            string colorString  = (formatArgs [++get] as PLString).Data;
                            drawingParams.edgeColor = CharToColor (colorString [0]);
                        }
                        break;

                        case "MarkerSize":
                            drawingParams.radius = (formatArgs [++get] as PLDouble).Data / 2;
                            break;

                        default: 
                            ParsePackedArg (ref drawingParams, arg);
                            break;
                    }

                    ++get;
                }
            }

            catch (Exception ex)
            {
                throw new Exception ("Error parsing format args: " + ex.Message);
            }

            return drawingParams;
        }

        //******************************************************************************************

        static private void ParsePackedArg (ref DrawingParameters drawingParams, string formatArg)
        {
            foreach (char c in formatArg)
            {
                switch (c)
                {
                    case 'r':
                    case 'n':
                    case 'y':
                    case 'g':
                    case 'b':
                    case 'k': drawingParams.color = CharToColor (c); break;
                            
                    case '-':
                    {
                        if (drawingParams.lineStyle == LineView.DrawingStyle.Dashes) drawingParams.lineStyle = LineView.DrawingStyle.LongDashes;
                        else                                                         drawingParams.lineStyle = LineView.DrawingStyle.Dashes;
                    }
                    break;

                    case '.': drawingParams.lineStyle = LineView.DrawingStyle.Dots; break;  
                            
                    case 'o': drawingParams.pointStyle = PointView.DrawingStyle.Circle; break;
                    case 's': drawingParams.pointStyle = PointView.DrawingStyle.Square; break;
                    case '*': drawingParams.pointStyle = PointView.DrawingStyle.Star; break; 
                    case 't': drawingParams.pointStyle = PointView.DrawingStyle.Triangle; break; 
                    case 'x': drawingParams.pointStyle = PointView.DrawingStyle.X; break; 
                    case '+': drawingParams.pointStyle = PointView.DrawingStyle.Plus; break;

                    default: throw new Exception ("Unrecognized plot format arg: " + formatArg);
                }
            }
        }

        //******************************************************************************************

        static private Brush CharToColor (char c)
        {
            Brush b;

            switch (c)
            {
                case 'r': b = Brushes.Red; break;
                case 'n': b = Brushes.Orange; break;
                case 'y': b = Brushes.Yellow; break;
                case 'g': b = Brushes.Green; break;
                case 'b': b = Brushes.Blue; break;
                case 'k': b = Brushes.Black; break;
                default: b = Brushes.Black; break;
            }

            return b;
        }
    }
}
