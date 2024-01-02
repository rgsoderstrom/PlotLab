using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace utVariables
{
    class Program
    {
        static void Main (string [] args)
        {
            try
            {
                PLDouble a = new PLDouble (1.2);
                PLDouble b = new PLDouble (3.4);
                PLVariable c = a + b;

                PLDouble d = a.Add (b);

                Console.WriteLine ("c = " + (c as PLDouble).Data);
                Console.WriteLine ("d = " + d.Data);

                //PLMatrix M1 = new PLMatrix (3, 4);

                //for (int r = 0; r<M1.Rows; r++)
                //    for (int c = 0; c<M1.Cols; c++)
                //        M1 [r, c] = (r + 3) * (c + 1.2345);

                //PLMatrix M2 = new PLMatrix (4, 2);

                //for (int r = 0; r<M2.Rows; r++)
                //    for (int c = 0; c<M2.Cols; c++)
                //        M2 [r, c] = (r + 7) * (c + 7);


                //PLMatrix M3 = new PLMatrix (3, 4);

                //for (int r = 0; r<M3.Rows; r++)
                //    for (int c = 0; c<M3.Cols; c++)
                //        M3 [r, c] = (1 * r + 3) * (1 * c + 1);


                //Console.WriteLine (M1.ToString ()); Console.WriteLine ();
                ////     Console.WriteLine (M2.ToString ()); Console.WriteLine ();
                ////Console.WriteLine (M3.ToString ()); Console.WriteLine ();

                ////    PLMatrix M4 = M1 + M3;
                ////    Console.WriteLine (M4.ToString ()); Console.WriteLine ();

                //PLMatrix M5 = M1 ^ new PLDouble (2);
                //Console.WriteLine (M5.ToString ()); Console.WriteLine ();


                //Console.WriteLine ();

//                Console.WriteLine ("=======================================================");

                PLCMatrix N1 = new PLCMatrix (2, 3);
                N1 [0, 0] = new PLComplex (1, 2);
                N1 [0, 1] = new PLComplex (3, 4);
                N1 [0, 2] = new PLComplex (5, 6);
                N1 [1, 0] = new PLComplex (7, 6);
                N1 [1, 1] = new PLComplex (3, 1);
                N1 [1, 2] = new PLComplex (8, 9);


                PLCMatrix N2 = new PLCMatrix (2, 3);
                N2 [0, 0] = new PLComplex (3, 2);
                N2 [0, 1] = new PLComplex (2, 8);
                N2 [0, 2] = new PLComplex (7, 7);
                N2 [1, 0] = new PLComplex (2, 7);
                N2 [1, 1] = new PLComplex (1, 9);
                N2 [1, 2] = new PLComplex (8, 3);



                //for (int r = 0; r<N1.Rows; r++)
                //    for (int c = 0; c<N1.Cols; c++)
                //        N1 [r, c] = new PLComplex ((r + 3) * (c + 1), r * c + 1);

                //PLCMatrix N2 = new PLCMatrix (4, 2);

                //for (int r = 0; r<N2.Rows; r++)
                //    for (int c = 0; c<N2.Cols; c++)
                //        N2 [r, c] = new PLComplex ((r + 5) * (c + 2), 2 * r * c + 2);

                //Console.WriteLine (N1.ToString ()); Console.WriteLine ();
                //Console.WriteLine (N2.ToString ()); Console.WriteLine ();

                //PLCMatrix N3 = new PLComplex (2, -3) * N1;// + N2;
                //Console.WriteLine (N3.ToString ()); Console.WriteLine ();

                //Console.WriteLine (N [7].ToString ()); Console.WriteLine ();

                //PLMatrix lst2 = N1.CollapseToColumn ();

                //rows = lst2.Rows;

                //for (int i = 0; i<rows; i++)
                //    Console.WriteLine (lst2 [i].ToString ("%9.6f"));
                //Console.WriteLine ();

            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }
    }
}
