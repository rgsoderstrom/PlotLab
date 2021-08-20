using System;
using System.Collections.Generic;

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
            };
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        // User defined function

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






