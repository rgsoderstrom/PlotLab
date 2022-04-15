using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;

using Common;

namespace utHelpWindow
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

        List<HelpTree> listOfHelpTrees = new List<HelpTree> ();

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            Top = 20;
            Left = 30;

            try
            {
                foreach (string fileName in listOfHelpTextFiles) 
                { 
                    listOfHelpTrees.Add (new HelpTree (fileName));
                }

                foreach (HelpTree ht in listOfHelpTrees)
                {
                    ht.FillInChildReferences ();
                }

                foreach (HelpTree ht in listOfHelpTrees)
                {
                    HelpTreeView.Items.Add (ht.BuildTreeView (HelpTextBox));
                }
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception loading Help Trees: " + ex.Message);
            }
        }
    }
}



