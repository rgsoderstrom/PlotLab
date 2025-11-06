using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;

using PLCommon;

namespace FunctionLibrary
{
    static public partial class MathFunctions
    {
        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static public Dictionary<string, PLFunction> GetUserDefinedContents ()
        {
            return new Dictionary<string, PLFunction> 
            {
                {"ContourZ1",   ContourTestFunction},
                {"scalarField", ScalarField},
                {"Covector1",   Covector1},
                {"Covector2",   Covector2},
                {"TransducerSamples", TransducerSamples}
            };
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // User defined function

        // find largest jump in col 2

        static public PLVariable TransducerSamples (PLVariable args)
        {
            PLRMatrix input = args as PLRMatrix;

            if (input == null)
                throw new Exception ("TransducerSamples only accepts N-by-3 real matrix");

            // differences
            PLRMatrix diffs = new PLRMatrix (input.Rows-1, 1);

            for (int i=0; i<input.Rows-1; i++)
                diffs [i, 0] = input [i+1, 1] - input [i, 1];

            // largest absolute value difference and its index
            double ld = diffs [1, 0];
            double lda = 0;

            for (int i=1; i<diffs.Rows; i++)
            { 
                if (ld < Math.Abs (diffs [i, 0]))
                { 
                    ld = Math.Abs (diffs [i, 0]);
                    lda = input [i, 2];
                }
            }

            PLRMatrix results = new PLRMatrix (1, 2);
            results [0,0] = ld;
            results [0,1] = lda;
            return results;
        }

        //*********************************************************************************************

        static public PLVariable ContourTestFunction (PLVariable args)
        {
            PLList list = args as PLList;
            PLDouble xx = list [0] as PLDouble;
            PLDouble yy = list [1] as PLDouble;

            double x = xx.Data;
            double y = yy.Data;
            double z = Math.Sqrt (x * x + y * y + x * y); 
            return new PLDouble (z);
        }

        //*********************************************************************************************

        // eigenchris\TC7\scalarField.m

        static public PLVariable ScalarField (PLVariable args)
        {
            PLList list = args as PLList;
            PLDouble xx = list [0] as PLDouble;
            PLDouble yy = list [1] as PLDouble;

            double x = xx.Data;
            double y = yy.Data;
            double z = y * y + x - 1/2;
            return new PLDouble (z);
        }

        //*********************************************************************************************

        // eigenchris

        static public PLVariable Covector1 (PLVariable args)
        {
            double ep1 = 1.15789473684211;
            double ep2 = -0.42105263157895;

            PLList list = args as PLList;
            PLDouble xx = list [0] as PLDouble;
            PLDouble yy = list [1] as PLDouble;

            double x = xx.Data;
            double y = yy.Data;
            double z = ep1 * x + ep2 * y;
            return new PLDouble (z);
        }

        static public PLVariable Covector2 (PLVariable args)
        {
            double ep1 = -0.10526315789474;
            double ep2 = 0.94736842105263;

            PLList list = args as PLList;
            PLDouble xx = list [0] as PLDouble;
            PLDouble yy = list [1] as PLDouble;

            double x = xx.Data;
            double y = yy.Data;
            double z = ep1 * x + ep2 * y;
            return new PLDouble (z);
        }

    }
}






