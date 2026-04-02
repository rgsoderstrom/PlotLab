

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

        private List<List<IToken>> TokensForFileLines = new List<List<IToken>> ();

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            { 
                WindowState = WindowState.Minimized;

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

                        Console.WriteLine (text);

                        Counter++;

                        text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
                        AnnotatedString annotated = new AnnotatedString (text);

                        Console.WriteLine (annotated.ToString ());

                        //**********************************************************************

                        if (false) // show token parsing
                        { 
                            // first pass
                            TokenParsing parsing = new TokenParsing ();
                            Window win = new Window ();
                            TextBox tb = new TextBox ();

                            // first pass
                            List<IToken> tokens = parsing.ParsingPassOne (annotated);
                            tb.Text += "First pass:\n";
                            foreach (IToken tok in tokens) tb.Text += tok.ToString () + "\n";

                            // second pass
                            tokens = parsing.ParsingPassTwo (tokens);
                            tb.Text += "\nSecond pass:\n";
                            foreach (IToken tok in tokens) tb.Text += tok.ToString () + "\n";

                            win.Content = tb;
                            win.SizeToContent = SizeToContent.Height;
                            win.Title = "Parsing " + Counter;
                            win.Width = 400;
                            win.Show ();
                        }

                        //**********************************************************************

                        ExpressionTreeNode.Print      = Print;

                        ExpressionTree tree = new ExpressionTree (annotated);

                        if (false) // show expression tree
                        { 
                            Window win2 = new Window ();
                            TreeView tv = new TreeView ();
                            tv.Items.Add (tree.TreeView ());
                            win2.Content = tv;
                            win2.Title = "Tree " + Counter;
                            win2.Width = 400;
                            win2.Height = 300;
                            win2.Show ();
                        }

                        //**********************************************************************

                        PLVariable answer = tree.Evaluate ();

                        //Console.WriteLine ("SupressPrinting = " + tree.SupressPrinting);
                        if (answer is PLNull == false)  Console.WriteLine ("answer: " + answer.ToString ());
                        else                            Console.WriteLine ("null answer");

                        Console.WriteLine ("========================================");
                    }
                }

                inputFile.Close ();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine ("Exception in Window_Loaded: " + ex.Message);
                EventLog.WriteLine (ex.StackTrace);
            }
        }
    }
}
