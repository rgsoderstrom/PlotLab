
//
// See FreeMat/PV_Tests.m for FreeMat script to provide truth data
//

using System;
using System.Collections.Generic;

using PLCommon;

public delegate PLVariable PLFunction (PLVariable var);

namespace utVariables
{
    class Program
    {
        static void Main (string [] args)
        {
            try
            {
                // declare everything needed
                PLRMatrix rm1 = new PLRMatrix (3, 3);
                PLRMatrix rm2 = new PLRMatrix (3, 3);

                PLCMatrix cm1 = new PLCMatrix (3, 3);
                PLCMatrix cm2 = new PLCMatrix (3, 3);

                PLDouble pd1 = new PLDouble (7);
                PLDouble pd2 = new PLDouble (5);

                PLComplex pc1 = new PLComplex (1, 3);
                PLComplex pc2 = new PLComplex (5, 7);

                PLInteger pi1 = new PLInteger (3);
                PLInteger pi2 = new PLInteger (9);

                // initialize matrices
                for (int i = 0; i<rm1.Rows; i++)
                    for (int j = 0; j<rm1.Cols; j++)
                        rm1 [i, j] = (i + 1) * (j + 1);

                for (int i = 0; i<rm2.Rows; i++)
                    for (int j = 0; j<rm2.Cols; j++)
                        rm2 [i, j] = (i + 10) + (j + 10);

                for (int i = 0; i<cm1.Rows; i++)
                    for (int j = 0; j<cm1.Cols; j++)
                        cm1 [i, j] = new PLComplex (i + 1, j + 1);

                for (int i = 0; i<cm2.Rows; i++)
                    for (int j = 0; j<cm2.Cols; j++)
                        cm2 [i, j] = new PLComplex (2 * i + 7, 3 * j + 9);

                //***************************************************************

                //PLMatrix m = rm1.

                List<PLVariable> results = new List<PLVariable>
                {
                    rm1 - pd1,
                    pd2 - rm2,
                    cm1 - pd1,
                    pd2 - cm2,

                    //cm2 + pc1,
                    //pc1 + cm1,
                    //cm2 + pd1,
                    //pd2 + cm1,

                    //pd1 / pd2,
                    //pd1 / pc1,
                    //pc1 / pd1,
                    //pc1 / pc2,

                    //rm1 / pd1,
                    //pd1 / rm1,

                    //rm1,
                    //cm1,
                    //rm1.Get (1, 1),
                    //cm1.Get (1, 1),
                    //cm1.CollapseToColumn (),
                    //cm1.CollapseToColumn (),
                };

                foreach (PLVariable plv in results)
                {
                    Console.WriteLine (plv.GetType ());
                    Console.WriteLine (plv + "\n");
                }


            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }
    }
}
