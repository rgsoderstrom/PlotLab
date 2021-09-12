using System;
using System.Collections.Generic;

using CommonMath;
using PLCommon;
using PLWorkspace;
using PLKernel;

namespace utKernel
{
    class KernelTest
    {
        readonly MainWindow window;

        static List<string> testCases = new List<string> ()
        {
            //"AA",
            //"AA (1)",
            //"length (AA)",
            "AA ('end')",
            //"AA (length (AA))",


            //"PlotVector (A', 'g')",
            //"text ([1 ; 2], 'aaa')",
            //"PlotVector ([1, 2]', 'g')",

            //"plot (1:3)",
            //"axis ([-1 5 -2 8])",
            //"a = axis",
            //"a",

            //"[  3/2 5/6 ; 4/3 3/4]",

            //"[  (3/2), (5/6) ; ( 4/3), (3/4 )  ]",

            //"[ [1 ; 2 ; 3], [4 ; 5 ; 6] ]",

            //"x = [1 : 6]",
            //"size (x, 1)",


            //"y = [11: 16]",
            //"[x' y']",
            //"[x ; y]",
            //"[x ; y]'",

            //"Origin = [0 ; 0]",
            //"A1 = [1 ; 0.2]",

            //"PlotVector (Origin, A1, [1 0 ; 0 1], 'r')",
            //"hold on",

            //"R = [0.4 ; 0.32]",
            //"PlotVector (Origin, R,  [1 0 ; 0 1], 'b-*')",

            //"disp ('asdfg')",
            //"disp (A)",
            //"disp ('A = %f', A)",
            //"disp (sprintf ('A = %f', A))",

            //"disp (B)",
            //"disp (C)",
            //"disp (D)",
            //"disp (E)",

            //"disp ('A = ')",
            //"disp (sprintf ('%f', A))",

            //"W = [1 ; 2 ; 3 ; 4 ; 5 ; 6]",
            //"A",

            //"h = sprintf ('zzz = %5.3f, w = %5.2f, c = %4.1f done'  , 1000 * 22/7, 123, 456)",
            //"h",
            //"h = sprintf ('zzz = %f, w = %d, c = %d done'  , 1000 * 22/7, 123, 456)",

            //"(A .* A)'",

            //"~B",

            //"A'''''",
            //"V = W'",
            //"AA",
            //"AA (3)",
            //"AA (2:4)",

            //"A = [(1, 2, 3, 4, 5, 6) ; (10, 20, 30, 40, 50, 60) ; (100, 200, 300, 400, 500, 600)]",
            //"A (2,3) = 999",

            //"A = [(1, 2, 3, 4, 5, 6) ; (10, 20, 30, 40, 50, 60) ; (100, 200, 300, 400, 500, 600)]",
            //"B = [(11, 22) ; (33, 44)]",
            //"A (1:2, 2:3) = B",

            //"A (2:3, 3:4)",
            //"A (:,3)",
            //"A (2:3, :)",
            //"A (6)",
            //"A (:)",
            //"A (2:3, 4)",
            //"A (:)",
            //"AA",
            //"AA (4:5)",
            //"AAA",
            //"AAA (2:4)",

            //"A (:, 3)",
            //"A (2, :)",

            //"0.5",
            //"[0.5]",
            //"[0.5, 0.3]",

           // "contour (ContourZ1, [0.1 : 0.4 : 5] + 2, -2.5, 2.5, -2, 2)",

            //"x = [-2.5 : 0.5 : 2.5]",
            //"y = [-2.5 : 0.5 : 2.5]",
            //"z = (x .* x)' * (y .* y)",

            //"contour (x, y, z, [0.1 0.5 0.9] + 0.1)",

            //"[0.1 : 0.4 : 5]",

            //" 6 * (1 ; 3 ; 5 ; 7 ; 9)",
            //"sqrt (7)",

            //"1 + 2 * (1 * 2 * 3) * 3 + 4 + 5",
            //"1 + C * (D * 2)",

            //"ZZ = 123",
            //"ZZ ^ 2",

            //"[3 : 4 : 19]",
            //"[3   4   19]",
            //"[3 ; 4 ; 19]",
            //"[3:4:19]",

            //"(1 : 4 : 20)",

            //"C",
            //"D",
            //"C == D",
            //"C ~= D",

            //"a = b = 567",
            //"a",
            //"b",

            //"p = 'aaaa'",

            //"A (3,4)",


            //"[1, 2, 3, 4, 5] * [10 ; 11 ; 12 ; 13 ; 14]",

            //"sqrt ('azxs')",
            //"sqrt ([1, 2, 3])",

            //"[1, 2, 3, 4, 5] * [1, 2, 3, 4, 5]'",
            //"[3:7:22] * E",

            //"0.123 * -.456",

            //"1 + 3 + 5 + 7 + 9",
            //"1 : 3 : 10",

            //"[(3;4;5) (6;7;8) (9;10;11)]",
            //"[[3;4;5] [6;7;8] [9;10;11]]",


            //"A",
            //"A (2, 3)",
            //"A (2:1:3, 3)",
            //"A (2:3, 3)",
            //"A (:)",
            //"A'",
            //"-5 * A",
            //"B",
            //"-sin (sqrt (sin (77)))",

            //"7 + sin (A) + 9",
            //"7 * A' + 9",
            //"66 * A'' + 88",




            //"-A * -7",
            //"-7 * H",
            //"-7",
            //"-A",
            //"--+8",
            //"---A",

            //"B",
            //"~B",
            //"~~B",

            //"-0.5",
            //"-.5",
            //"+sin (13)",
            //"-sin (12)",

            //"-sqrt (8)",
            //"8 -- 5",


            //"A (1:3, 5:6)",
            //"A (:)",
            //"sqrt (1, sqrt, 3 * 7)",
            //"A = 8",
            //"B = 8",


            //"[(1;3), (2;4), (3;5), (4;6), (5;7)]",
            //"[3:9*2]",
            //"[1, 2, 3, 4, 5]",
            //"[3:7:22]",
            //"[10 11 12 13]",
            //"[10 ; 11 ; 12 ; 13]",
            //"7",
            //"1 * 2 * 3 + 4 * 5",
            //"sqrt (45)",
        };

        static int Next = 0;

        Workspace workspace = new Workspace ();

        public KernelTest (MainWindow wind)
        {
            window = wind;
            Workspace.Print = Console.Write;

            //CommonMath.Matrix mat = new Matrix (3, 3);
            ////mat.FillByRow (new double [] { 1, 2, 3, 4, 11, 12, 13, 14, 21, 22, 23, 24 });
            //mat.FillByRow (new double [] { 0.3759,    0.3580,    0.0990,    0.0183,    0.7604,    0.4972,    0.9134,    0.8077,    0.3478 });
            //PLMatrix A = new PLMatrix ("A", mat);
            //workspace.Add (A);

            //CommonMath.Matrix mat = new Matrix (1, 2);
            //mat.FillByRow (new double [] { 1, 2});
            //PLMatrix A = new PLMatrix ("A", mat);
            //workspace.Add (A);

            CommonMath.Matrix mat2 = new Matrix (1, 6);
            mat2.FillByRow (new double [] { 21, 22, 23, 24, 25, 26 });
            PLMatrix AA = new PLMatrix ("AA", mat2);
            workspace.Add (AA);

            //CommonMath.Matrix mat3 = new Matrix (7, 1);
            //mat3.FillByRow (new double [] { 51, 52, 53, 54, 55, 56, 57 });
            //PLMatrix AAA = new PLMatrix ("AAA", mat3);
            //workspace.Add (AAA);



            //PLBool b = new PLBool (true);
            //b.Name = "B";
            //workspace.Add (b);

            //PLDouble C = new PLDouble (1.23);
            //C.Name = "C";
            //workspace.Add (C);

            //PLDouble D = new PLDouble (9.87);
            //D.Name = "D";
            //workspace.Add (D);

            //PLInteger E = new PLInteger (678);
            //E.Name = "E";
            //workspace.Add (E);

            //PLString S1 = new PLString ("ABCDEF");
            //S1.Name = "S1";
            //workspace.Add (S1);

            //PLString S2 = new PLString ("ghij");
            //S2.Name = "S2";
            //workspace.Add (S2);
        }

        public void ShowWorkspace ()
        {
            Console.WriteLine ("-----------------------------------------------\n");
            workspace.PrintKeysAndSizes (Console.Write);
        }

        public void DumpWorkspace ()
        {
            Console.WriteLine ("-----------------------------------------------\n");
            workspace.Dump (new PLNull ());
        }

        public void NextTest ()
        {
            try
            {
                window.TreeView1.Items.Clear ();

                Console.WriteLine ("-----------------------------------------------\n");

                Console.WriteLine ("Test Case:");
                Console.WriteLine (testCases [Next]);
                Console.WriteLine ();

                bool showTopLevelTokens = true;

                if (showTopLevelTokens == true)
                {
                    TokenParsing parsing = new TokenParsing ();
                    List<Token> passOneTokens = parsing.ParsingPassOne (testCases [Next]);
                    List<Token> passTwoTokens = parsing.ParsingPassTwo (passOneTokens, workspace);

                    Console.WriteLine ("Pass 1 tokens:");
                    foreach (Token tok in passOneTokens)
                        Console.WriteLine (tok);

                    Console.WriteLine ();

                    Console.WriteLine ("Pass 2 tokens:");
                    foreach (Token tok in passTwoTokens)
                        Console.WriteLine (tok);
                    Console.WriteLine ();
                }

                Console.WriteLine ("Build TreeView");

                ExpressionTree tree = new ExpressionTree (testCases [Next], workspace);
                window.TreeView1.Items.Clear ();
                window.TreeView1.Items.Add (tree.TreeView ());

                Console.WriteLine ("Evaluate");

                PLVariable results = tree.Evaluate (workspace);
                Console.WriteLine (results.ToString ());
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }

            if (++Next == testCases.Count)
            {
                Next = 0;
                Console.WriteLine ("End of list");
            }
        }
    }
}
