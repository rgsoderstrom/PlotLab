

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using Main;
using PLWorkspace;
using Common;
using PLCommon;

namespace utExpressionTree
{
    public partial class MainWindow : Window
    {
    //  static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\ExpressionTreeTests.m";
        static readonly string InputMFileName = @"..\..\..\Examples\ExpressionTreeTests.m";

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
            EventLog.Open ("D:\\From_C_Visual Studio 2022\\Visual Studio 2022\\Projects\\PlotLab\\utExpressionTree\\log.txt");
        }

        private List<List<IToken>> TokensForFileLines = new List<List<IToken>> ();

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            { 
                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;
                int Counter = 0;

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

                        //**********************************************************************

                        Counter++;

                        // first pass
                        TokenParsing parsing = new TokenParsing ();
                        Window win = new Window ();
                        TextBox tb = new TextBox ();

                        // first pass
                        List<IToken> tokens = parsing.ParsingPassOne (annotated);
                        tb.Text += "First pass:\n";
                        foreach (IToken tok in tokens) tb.Text += tok.ToString () + "\n";

                        // second pass
                        tokens = parsing.ParsingPassTwo (tokens, workspace, library, fileSystem);
                        tb.Text += "\nSecond pass:\n";
                        foreach (IToken tok in tokens) tb.Text += tok.ToString () + "\n";

                        win.Content = tb;
                        win.SizeToContent = SizeToContent.Height;
                        win.Title = "Parsing " + Counter;
                        win.Width = 400;
                        win.Show ();

                        //**********************************************************************

                        ExpressionTreeNode.Workspace  = workspace;
                        ExpressionTreeNode.Library    = library;
                        ExpressionTreeNode.FileSystem = fileSystem;
                        ExpressionTreeNode.Print      = Print;

                        ExpressionTree tree = new ExpressionTree (annotated);

                        Window win2 = new Window ();
                        TreeView tv = new TreeView ();
                        tv.Items.Add (tree.TreeView ());
                        win2.Content = tv;
                        win2.Title = "Tree " + Counter;
                        win2.Width = 400;
                        win2.Height = 300;
                        win2.Show ();

                        //**********************************************************************

                        PLVariable answer = tree.Evaluate (workspace);
                        Console.WriteLine ("answer: " + answer.ToString ());
                    }
                }

                inputFile.Close ();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine ("Exception in Window_Loaded: " + ex.Message);
                EventLog.WriteLine ("Exception in Window_Loaded: " + ex.StackTrace);
            }
        }

    }
}
