

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Main;

namespace utExpressionTree
{
    public partial class MainWindow : Window
    {
        static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\ExpressionTreeTests.m";

        public static readonly Workspace  workspace  = new Workspace ();
        public static readonly Library    library    = new Library ();
        public static readonly FileSystem fileSystem = new FileSystem ();

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        public MainWindow ()
        {
            InitializeComponent ();
        }

        private List<List<IToken>> TokensForFileLines = new List<List<IToken>> ();

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            { 
                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        string text = InputLineProcessor.RemovePromptAndComments (raw);

                        if (text.Length == 0)
                            continue;

                        text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
                        AnnotatedString annotated = new AnnotatedString (text);

                     // Print (annotated.ToString ());

                        ExpressionTreeNode.Workspace  = workspace;
                        ExpressionTreeNode.Library    = library;
                        ExpressionTreeNode.FileSystem = fileSystem;
                        ExpressionTreeNode.Print      = Print;

                        ExpressionTree tree = new ExpressionTree (annotated);//, workspace, library, fileSystem, Print);

                        //TreeView tv = new TreeView ();
                        //tv.Items.Add (tree.TreeView ());
                        //win.Content = tv;
                        //win.Title = "Tree " + Counter;
                        //win.Width = 400;
                        //win.Height = 300;
                        //win.Show ();



                    }
                }

                inputFile.Close ();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine ("Window_Loaded: " + ex.Message);
            }
        }

    }
}
