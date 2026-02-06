

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using Main;

namespace utExpressionTree
{
    public partial class MainWindow : Window
    {
        static readonly string InputMFileName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\InputLineTests.m";

        private static readonly Workspace  workspace  = new Workspace ();
        private static readonly Library    library    = new Library ();
        private static readonly FileSystem fileSystem = new FileSystem ();

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
                       // List<List<IToken>> tok = inputProcessor.ParseOneInputLine (raw);
                    
                       //foreach (List<IToken> lt in tok)
                        //    TokensForFileLines.Add (lt);

                        Console.WriteLine (raw);
                    }
                }

                inputFile.Close ();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine ("Window_Loaded: " + ex.Message);
            }
        }

        //********************************************************************************
        //
        // Copied from utInputLine
        //
        private void InputLinesToTokens ()
        { 
            InputLineProcessor inputProcessor = new InputLineProcessor (workspace, library, fileSystem, Print);


        }
    }
}
