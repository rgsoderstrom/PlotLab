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

        string inputFile = @"C:\Users\rgsod\Documents\Visual Studio 2022\Projects\PlotLab\utHelpWindow\HelpTreeText.xml";

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            Left = 500;

            try
            {
                HelpTree ht = new HelpTree (inputFile);
                HelpTreeView.Items.Add (ht.BuildTreeView (HelpTextBox));

                
                
                //Console.WriteLine (ht.ToString ());
            }


            catch (Exception ex)
            {
                Console.Write ("Exception: " + ex.Message);
            }
        }
    }
}



