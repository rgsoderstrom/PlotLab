using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonMath;
using PLCommon;
using PLWorkspace;

namespace PLWorkspace
{
    class Program
    {
        static void Main (string [] args)
        {
            try
            {
                Workspace.Print = Console.Write;

                //ReadWriteTest ();
                OverwriteSubmatrix_Test2 ();
                //OverwriteSubmatrix_Test1 ();
                //Functions_Test1 ();
                //PushPopTest ();
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: {0}", ex.Message);
             //   Console.WriteLine (ex.StackTrace);
            }
        }

        //********************************************************************************

        static void Functions_Test1 ()
        {
            PLVariable RealScalar = new PLDouble ("RealScalar", 123);
            Workspace.Add (RealScalar);

            PLVariable ComplexScalar = new PLComplex ("ComplexScalar", 777, 888);
            Workspace.Add (ComplexScalar);

            CMatrix mat3 = new CMatrix (2, 3);
            mat3.FillByRow (new Complex [] {new Complex (12, 34),  new Complex (22, 22),  new Complex (33, 33),  
                                            new Complex (44, 44),  new Complex (55, 55),  new Complex (67, 89)});
            PLMatrix ComplexMat = new PLCMatrix ("ComplexMat", mat3);
            Workspace.Add (ComplexMat);


            PLVariable results;

            results = Workspace.EvaluateFunction ("exists", new PLString ("RealScalar"));
            Console.WriteLine ("RealScalar: " + results.ToString ());

            results = Workspace.EvaluateFunction ("size", Workspace.Get ("ComplexMat"));
            Console.WriteLine ("ComplexMat: " + results.ToString ());

            results = Workspace.EvaluateFunction ("cols", Workspace.Get ("ComplexMat"));
            Console.WriteLine ("ComplexMat: " + results.ToString ());

            results = Workspace.EvaluateFunction ("rows", ComplexMat);
            Console.WriteLine ("ComplexMat: " + results.ToString ());


        }

        //********************************************************************************

        static void OverwriteSubmatrix_Test1 ()
        {
            //Matrix mat2 = new Matrix (2, 2);
            //mat2.FillByRow (new double [] {101, 102, 
            //                               103, 104});
            //PLMatrix RealMat = new PLRMatrix ("RealMat", mat2);
            //Workspace.Add (RealMat);

            //PLVariable RealScalar = new PLDouble ("RealScalar", 123);
            //Workspace.Add (RealScalar);

            //CMatrix mat3 = new CMatrix (2, 3);
            //mat3.FillByRow (new Complex [] {new Complex (12, 34),  new Complex (22, 22),  new Complex (33, 33),  
            //                                new Complex (44, 44),  new Complex (55, 55),  new Complex (67, 89)});
            //PLMatrix ComplexMat = new PLCMatrix ("ComplexMat", mat3);
            //Workspace.Add (ComplexMat);

            //PLVariable ComplexScalar = new PLComplex ("ComplexScalar", 777, 888);
            //Workspace.Add (ComplexScalar);

            //Console.WriteLine ("before: " + RealMat.ToString ());
            //Console.WriteLine ();

            //Workspace.OverwriteSubmatrix ("RealMat",    // name of matrix already in workspace
            //                              1, 1,   // 1-based
            //                              RealScalar);    // new data to overwrite some of old

            //RealMat = Workspace.Get ("RealMat") as PLMatrix;
            //Console.WriteLine ("After: " + RealMat.ToString ());
        }

        //********************************************************************************

        static void OverwriteSubmatrix_Test2 ()
        {
            //Matrix mat1 = new Matrix (4, 5);
            //mat1.FillByRow (new double [] {11, 12, 13, 14, 15,
            //                               21, 22, 23, 24, 25,
            //                               31, 32, 33, 34, 35,
            //                               41, 42, 43, 44, 45 });
            //PLMatrix Z1 = new PLRMatrix ("Z1", mat1);
            //Workspace.Add (Z1);

            //Matrix mat2 = new Matrix (2, 2);
            //mat2.FillByRow (new double [] { 101, 102, 103, 104 });
            //PLMatrix Z2 = new PLRMatrix ("Z2", mat2);
            //Workspace.Add (Z2);

            CMatrix mat3 = new CMatrix (2, 3);
            mat3.FillByRow (new Complex [] {new Complex (12, 34),  new Complex (22, 22),  new Complex (33, 33),
                                            new Complex (44, 44),  new Complex (55, 55),  new Complex (67, 89)});
            PLMatrix Z3 = new PLCMatrix ("Z3", mat3);
            Workspace.Add (Z3);

            CMatrix mat4 = new CMatrix (1, 2);
            mat4.FillByRow (new Complex [] { new Complex (-12, -34), new Complex (-22, -22) });
            PLMatrix Z4 = new PLCMatrix ("Z4", mat4);
            Workspace.Add (Z4);

            Workspace.RunCommand ("dump");
            //return;


            //PLMatrix ZReadback = Workspace.Get ("Z3") as PLMatrix;
            //Console.WriteLine (ZReadback.ToString ());
            //Console.WriteLine ();

            //Workspace.OverwriteSubmatrix ("Z1",    // name of matrix already in workspace
            //                              2,2,    // 1-based
            //                              Z3);    // new data to overwrite some of old

            //Workspace.OverwriteSubmatrix ("Z1",    // name of matrix already in workspace
            //                              2, 2,   // 1-based
            //                              Z2);    // new data to overwrite some of old

            //Workspace.OverwriteSubmatrix ("Z3",    // name of matrix already in workspace
            //                              1, 1,   // 1-based
            //                              Z2);    // new data to overwrite some of old

            Workspace.OverwriteSubmatrix ("Z3",    // name of matrix already in workspace
                                          1, 1,   // 1-based
                                          Z4);    // new data to overwrite some of old

            //ZReadback = Workspace.Get ("Z3") as PLMatrix;
            //Console.WriteLine (ZReadback.ToString ());

            Console.WriteLine ("");
            Workspace.RunCommand ("dump");
        }

        //********************************************************************************

        static void PushPopTest ()
        {
            PLDouble X1 = new PLDouble ("X1", 2.456);
            PLDouble X2 = new PLDouble ("X2", 3.33);

            Workspace.Add (X1);
            Workspace.Add (X2);

            Workspace.RunCommand ("dump");

            //************************************************

            // variable names in Current
            List<string> ActualInputParameters = new List<string> () {"X1"};

            // what their copies will be called in function
            List<string> FormalInputParameters = new List<string> () {"XX1"};

            Workspace.Push ("Func1", ActualInputParameters, FormalInputParameters);

            // Add a new variable to function's workspace
            PLDouble X3 = new PLDouble ("X3", 88);
            Workspace.Add (X3);

            Workspace.RunCommand ("dump");

            //************************************************

            List<string> ActualOutputParameters = new List<string> () {"results"};
            List<string> FormalOutputParameters = new List<string> () {"X3"};

            Workspace.Pop (ActualOutputParameters, FormalOutputParameters);
            Workspace.RunCommand ("dump");
        }

        //********************************************************************************

        static void ReadWriteTest ()
        {
            PLDouble X1 = new PLDouble ("X1", 2.456);
            Workspace.Add (X1);

            PLDouble X2 = new PLDouble ("X2", 7.654);
            Workspace.Add (X2);

            PLDouble GX3 = new PLDouble ("GX3", 88);
            Workspace.AddGlobal (GX3);

            Workspace.RunCommand ("dump");

            PLVariable v1 = Workspace.Get ("X1");
            PLVariable v2 = Workspace.Get ("X2");
            PLVariable v3 = Workspace.Get ("GX3");
            //PLVariable v4 = Workspace.Get ("XX3");  // not defined, will throw exception

            Console.WriteLine (v1.ToString ());
            Console.WriteLine (v2.ToString ());
            Console.WriteLine (v3.ToString ());

         // push new function workspace
            List<string> SourceVarNames = new List<string> () {"X1"};
            List<string> DstVarNames    = new List<string> () {"localX1"};
                
            Workspace.Push ("Func1", SourceVarNames, DstVarNames);

            Workspace.RunCommand ("dump");

         // verify can get global but not local from caller


         // local with same name as global hides the global



        }




    }
}
