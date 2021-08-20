using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonMath;
using PLCommon;

namespace PLWorkspace
{
    class Program
    {
        static void Main (string [] args)
        {
            try
            {
                Workspace workspace = new Workspace ();
                Workspace.Print = Console.Write;




                Matrix mat = new Matrix (4, 5);
                mat.FillByRow (new double [] { 11, 12, 13, 14, 15, 21, 22, 23, 24, 25, 31, 32, 33, 34, 35, 41, 42, 43, 44, 45 });
                PLMatrix Z = new PLMatrix ("Z", mat);
                workspace.Add (Z);

                workspace.PrintKeysAndSizes (Console.WriteLine);



                mat = new Matrix (2, 2);
                mat.FillByRow (new double [] { 88, 89, 98, 99 });
                PLMatrix mod = new PLMatrix ("mod", mat);

                workspace.OverwriteSubmatrix ("Z", 2, 2, mod);
                workspace.OverwriteSubmatrix ("Z", 1, 4, new PLDouble (123));
                workspace.OverwriteSubmatrix ("Z", 2, 4, new PLInteger (456));

                PLMatrix ZZ = workspace.Get ("Z") as PLMatrix;
                Console.WriteLine (ZZ.ToString ());

               // workspace.OverwriteSubmatrix ("Z", 4, 2, mod);  // throws exception

                Console.WriteLine ("--------------------------------------------------");

                double Q = 123; PLDouble PQ = new PLDouble (Q); PQ.Name = "Q"; workspace.Add (PQ);
                int W = 456; PLInteger PW = new PLInteger (W); PW.Name = "W"; workspace.Add (PW);

                workspace.Dump (new PLNull ());
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: {0}", ex.Message);
            }
        }
    }
}
