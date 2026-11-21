
//
// See FreeMat/PV_Tests.m for FreeMat script to provide truth data
//

using System;
using System.Collections.Generic;

using PLCommon;

//public delegate PLVariable PLFunction (PLVariable var);

using CommonMath;

namespace utVariables
{
    class Program
    {
        static void Main (string [] args)
        {
            // declare two of each:
            //    real matrix
            //    complex matrix
            //    real scalar
            //    complex scalar

            Matrix rm1 = new Matrix (3, 3); rm1.FillByRow (new double [] {3, 4,  5, 
                                                                          6, 7,  8, 
                                                                          9, 10, 11});
            PLRMatrix realMatrix1 = new PLRMatrix ("realMatrix1", rm1);
                
            Matrix rm2 = new Matrix (3, 3); rm2.FillByRow (new double [] {13, 14, 15, 
                                                                          16, 17, 18, 
                                                                          19, 20, 21});
            PLRMatrix realMatrix2 = new PLRMatrix ("realMatrix2", rm2);
                
            CMatrix cm1 = new CMatrix (3, 3); cm1.FillByRow (new Complex [] {new Complex (3, 1), new Complex (4, 1),  new Complex (5, 1),
                                                                             new Complex (6, 1), new Complex (7, 1),  new Complex (8, 1),
                                                                             new Complex (9, 1), new Complex (10, 1), new Complex (11, 1)});
            PLCMatrix complexMatrix1 = new PLCMatrix ("complexMatrix1", cm1);
                
            CMatrix cm2 = new CMatrix (3, 3); cm2.FillByRow (new Complex [] {new Complex (13, 1), new Complex (14, 1), new Complex (15, 1),
                                                                             new Complex (16, 1), new Complex (17, 1), new Complex (18, 1),
                                                                             new Complex (19, 1), new Complex (20, 1), new Complex (21, 1)});
            PLCMatrix complexMatrix2 = new PLCMatrix ("complexMatrix2", cm2);
                

            PLDouble double1 = new PLDouble (107);
            PLDouble double2 = new PLDouble (105);

            PLComplex complex1 = new PLComplex (100, 300);
            PLComplex complex2 = new PLComplex (500, 700);

            //***************************************************************

            List<PLVariable> allOp1 = new List<PLVariable> () {realMatrix1, complexMatrix1, double1, complex1};
            List<PLVariable> allOp2 = new List<PLVariable> () {realMatrix2, complexMatrix2, double2, complex2};

            //***************************************************************

            foreach (PLVariable op1 in allOp1)
            {
                foreach (PLVariable op2 in allOp2)
                {           
                    try
                    { 
                        Console.WriteLine (op1.GetType ().ToString () + " and " + op2.GetType ());
                        PLVariable results = op1 + op2;
                        Console.WriteLine ("result type: " + results.GetType ());
                        Console.WriteLine (results.ToString ());
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine ("Exception calculating: " + ex.Message);
                    }

                    Console.WriteLine ("\n---------------------------------------------\n");
                }
            }
        }
    }
}
