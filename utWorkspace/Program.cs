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

                PushPopTest ();


                //BaseWorkspace baseWorkspace = new BaseWorkspace ("Base");

                //PLDouble X1 = new PLDouble ("X1", 2.456);
                //baseWorkspace.Add (X1);

                //PLDouble X2 = new PLDouble ("X2", 3.33);
                //baseWorkspace.Add (X2);

                //PLDouble X3 = new PLDouble ("X3", 88);
                //baseWorkspace.Add (X3);

                //baseWorkspace.Dump ();

                //List<string> baseNames = new List<string> () {"X2"};
                //List<string> funcNames = new List<string> () {"AA"};
                //FunctionWorkspace functionWorkspace = new FunctionWorkspace ("F1", baseWorkspace, baseNames, funcNames);
                //functionWorkspace.Dump ();

                //PLDouble AA = functionWorkspace.Get ("AA") as PLDouble;
                //AA = new PLDouble (AA.Name, 100);
                //functionWorkspace.Add (AA);
                //functionWorkspace.Dump ();

                //functionWorkspace.Get (baseWorkspace, baseNames, funcNames);
                //baseWorkspace.Dump ();

                ////       GlobalWorkspace globalWorkspace = new GlobalWorkspace ();

                ////       WorkspaceManager manager = new WorkspaceManager (baseWorkspace);


                ////       Matrix mat = new Matrix (4, 5);
                ////       mat.FillByRow (new double [] { 11, 12, 13, 14, 15, 21, 22, 23, 24, 25, 31, 32, 33, 34, 35, 41, 42, 43, 44, 45 });
                ////       PLMatrix Z = new PLRMatrix ("Z", mat);
                ////       baseWorkspace.Add (Z);

                ////       PLDouble X = new PLDouble (1.234);
                ////       X.Name = "X";
                ////       globalWorkspace.Add (X);


                ////       X = new PLDouble (2.456);
                ////       X.Name = "X";
                //////       baseWorkspace.Add (X);


                ////       Console.WriteLine ("Base workspace:");
                ////       baseWorkspace.PrintKeysAndSizes (Console.Write);

                ////       Console.WriteLine ("\nGlobal workspace:");
                ////       globalWorkspace.PrintKeysAndSizes (Console.Write);

                ////       //PLDouble Y =
                ////       if      (baseWorkspace.Exists ("PI"))   X = baseWorkspace.Get ("PI") as PLDouble;
                ////       else if (globalWorkspace.Exists ("PI")) X = globalWorkspace.Get ("PI") as PLDouble;
                ////       else throw new Exception ("not found");

                ////       Console.WriteLine (X.Name + " = " + X.ToString ());




                // mat = new Matrix (2, 2);
                // mat.FillByRow (new double [] { 88, 89, 98, 99 });
                // PLMatrix mod = new PLRMatrix ("mod", mat);

                // workspace.OverwriteSubmatrix ("Z", 2, 2, mod);
                // workspace.OverwriteSubmatrix ("Z", 1, 4, new PLDouble (123));
                // workspace.OverwriteSubmatrix ("Z", 2, 4, new PLInteger (456));

                // PLMatrix ZZ = workspace.Get ("Z") as PLMatrix;
                // Console.WriteLine (ZZ.ToString ());

                //// workspace.OverwriteSubmatrix ("Z", 4, 2, mod);  // throws exception

                // Console.WriteLine ("--------------------------------------------------");

                // double Q = 123; PLDouble PQ = new PLDouble (Q); PQ.Name = "Q"; workspace.Add (PQ);
                // int W = 456; PLInteger PW = new PLInteger (W); PW.Name = "W"; workspace.Add (PW);

                // workspace.Dump (new PLNull ());
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: {0}", ex.Message);
                Console.WriteLine (ex.StackTrace);
            }
        }

        //********************************************************************************

        static void PushPopTest ()
        {
            PLDouble X1 = new PLDouble ("X1", 2.456);
            PLDouble X2 = new PLDouble ("X2", 3.33);

            Workspace.Add (X1);
            Workspace.Add (X2);

            Workspace.Dump ();

            //************************************************

            List<string> ActualInputParameters = new List<string> () {"X1"};
            List<string> FormalInputParameters = new List<string> () {"XX1"};
                
            Workspace.PushNew ("Func1", ActualInputParameters, FormalInputParameters);

            PLDouble X3 = new PLDouble ("X3", 88);
            Workspace.Add (X3);

            Workspace.Dump ();

            //************************************************

            List<string> ActualOutputParameters = new List<string> () {"XXX1"};
            List<string> FormalOutputParameters = new List<string> () {"X3"};

            Workspace.PopFunction (ActualOutputParameters, FormalOutputParameters);
            Workspace.Dump ();
        }

    }
}
