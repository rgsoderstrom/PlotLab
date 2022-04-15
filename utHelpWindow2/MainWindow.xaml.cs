using System;
using System.Collections.Generic;
using System.Windows;

using Common;

namespace utHelpWindow2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            InitializeComponent ();
        }

        List<string> listOfHelpTextFiles = new List<string> ()
        {
            @"C:\Users\rgsod\Documents\Visual Studio 2022\Projects\PlotLab\utHelpWindow\HelpTextFiles\HelpTreeText1.xml",
            @"C:\Users\rgsod\Documents\Visual Studio 2022\Projects\PlotLab\utHelpWindow\HelpTextFiles\HelpTreeText2.xml",
        };

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {
                EventLog.Open (@"..\..\..\log.txt", false);



                List<HelpTreeNodeList> lst = new List<HelpTreeNodeList> ();

                foreach (string filename in listOfHelpTextFiles)
                    lst.Add (new HelpTreeNodeList (filename));



                XamlHelpTreeView.Fill (lst, XamlHelpTextBox);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception: " + ex.Message);
            }
        }

        private void Window_Closed (object sender, EventArgs e)
        {
            EventLog.Close ();
        }
    }
}
