using System;
using System.Collections.Generic;
using System.Windows;

using Common;

namespace utHelpWindow2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow ()
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

                HelpTreeView.Fill (lst, HelpTextBlock, HelpTextClearButton, HelpTextScroller);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception: " + ex.Message);
            }
        }

        //****************************************************************************************

        private void TextBlockClearButton_Click (object sender, RoutedEventArgs e)
        {
            HelpTreeView.ClearHelpText ();
        }

        //****************************************************************************************

        private void Window_Closed (object sender, EventArgs e)
        {
            EventLog.Close ();
        }
    }
}
