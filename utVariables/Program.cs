using System;
using System.Collections.Generic;
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
            Random rand = new Random ();

            List<PLDouble> A = new List<PLDouble> ()
            {
                new PLDouble (100 * 22.0 / 7),
                new PLDouble (-10 * 22.0 / 7),
                new PLDouble (-1 * 22.0 / 7),
                new PLDouble (0.1 * 22.0 / 7),
                new PLDouble (0.01 * 22.0 / 7),
            };

            foreach (PLDouble a in A)
                Console.WriteLine (a.ToString ("%8.4f"));
            Console.WriteLine ();
            Console.WriteLine ();
            Console.WriteLine ();

            //****************************************************

            PLMatrix M = new PLMatrix (3, 4);

            for (int r = 0; r<M.Rows; r++)
                for (int c = 0; c<M.Cols; c++)
                    M [r, c] = 100 * (rand.NextDouble () - 0.5);


            string str = M.ToString ("%9.3f");
            Console.WriteLine (str);
            Console.WriteLine ();

            //****************************************************

            List<PLInteger> G = new List<PLInteger> ();

            for (int i = 0; i<8; i++)
                G.Add (new PLInteger ((int) (100 * (rand.NextDouble () - 0.5))));

            for (int i = 0; i<G.Count; i++)
                Console.WriteLine (G [i].ToString ("%6d"));

            //****************************************************

            PLList H = new PLList ();
            for (int i = 0; i<8; i++)
                H.Add (new PLInteger ((int) (100 * (rand.NextDouble () - 0.5))));

            Console.WriteLine ();
            Console.WriteLine (H.ToString ("%8d"));
        }
    }
}
