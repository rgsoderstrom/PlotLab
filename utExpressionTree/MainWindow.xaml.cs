

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using PLMain;
using PLWorkspace;
using Common;
using PLCommon;
using FunctionLibrary;

namespace utExpressionTree
{
    public partial class MainWindow : Window
    {
        static readonly string InputMFileName = @"..\..\..\Examples\ExpressionTreeTests.m";

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        public MainWindow ()
        {
            InitializeComponent ();
            EventLog.Open (@"..\..\log.txt");
            IOFunctions.Print = Print;
        }

        //**********************************************************************

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            { 
                WindowState = WindowState.Minimized;

                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;
                int Counter = 0;

                //****************************************************************
                //****************************************************************
                //****************************************************************

                // Write some test data directly into Workspace

                CommonMath.Matrix bm = new CommonMath.Matrix (1, 5);
                bm.FillByRow (new double [] {91, 92, 93, 94, 95});

                PLMatrix b = new PLRMatrix (bm) {Name = "b"};
                Workspace.Add (b);

                //********************

                CommonMath.Matrix cm = new CommonMath.Matrix (4, 5);
                cm.FillByRow (new double [] {11, 12, 13, 14, 15, 
                                             21, 22, 23, 24, 25, 
                                             31, 32, 33, 34, 35, 
                                             41, 42, 43, 44, 45});

                PLMatrix c = new PLRMatrix (cm) {Name = "c"};
                Workspace.Add (c);

                //****************************************************************
                //****************************************************************
                //****************************************************************

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        string trimmed = raw.Trim ();

                        if (trimmed.Length == 0)
                            continue;

                        if (trimmed [0] == '%')
                            continue;

                        Console.WriteLine (trimmed);
                        AnnotatedString astr = new AnnotatedString (trimmed);
                        AnnotatedStringSet annotatedSet = new AnnotatedStringSet (astr);

                        foreach (AnnotatedString annotated in annotatedSet)
                        {
                            Console.WriteLine (annotated.ToString ());
                            Counter++;

                            //**********************************************************************

                            ExpressionTree.ShowParsingTokens = true;
                            ExpressionTree.ShowExprTree = true;

                            ExpressionTree tree = new ExpressionTree (annotated);

                            PLVariable answer = tree.Evaluate ();

                            if (answer is PLNull == false)  Console.WriteLine ("answer: " + answer.ToString ());
                            else                            Console.WriteLine ("null answer");

                            Console.WriteLine ("========================================");
                        }
                    }
                }

                inputFile.Close ();
            }
            
            catch (NotImplementedException ex)
            {
                Console.WriteLine ("Not implemented: " + ex.Message);
                //Console.WriteLine ("Not implemented: " + ex.StackTrace);
                EventLog.WriteLine (ex.Message);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception" + ex.Message);
                EventLog.WriteLine (ex.StackTrace);
            }
        }
    }
}
