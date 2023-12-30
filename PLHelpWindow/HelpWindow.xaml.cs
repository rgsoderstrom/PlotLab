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

using Common;

namespace PLHelpWindow
{
    partial class HelpWindow : Window
    {
        List<string> helpTextFiles = new List<string> ()
        {
            "General.xml",
            "GeneralPlotting.xml",
            "ContourPlots.xml",
            "VectorPlots2D.xml",
            "VectorPlots3D.xml",
            "PlotFigures.xml",
            "DisplayingText.xml",
            "MathFunctions.xml",
            "SignalProcessing.xml",
            "ComplexNumbers.xml",
            "M-FileFunctions.xml",
        };

        internal HelpWindow ()
        {
            InitializeComponent ();
        }

        internal bool SearchFor (string str)
        {
            List<HelpTreeViewItem> path = new List<HelpTreeViewItem> ();
            bool found = false;

            foreach (TreeViewItem tvi in TreeView.Items)
            {
                HelpTreeViewItem htvi = tvi as HelpTreeViewItem;

                if (htvi != null)
                {
                    found = htvi.Contains (str, ref path);
                    if (found == true)
                        break;
                }
            }

            if (found == true)
            {                
                foreach (HelpTreeViewItem htvi in path)
                    htvi.IsExpanded = true;

                path [path.Count - 1].IsSelected = true;
            }

            return found;
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            HelpTreeViewItem.win          = this;
            HelpTreeViewItem.textPane     = HelpTextBlock;
            HelpTreeViewItem.scrollViewer = HelpTextScroller;
           // HelpTreeViewItem.clearButton  = HelpTextClearButton;

            try
            {
                List<HelpList> lst = new List<HelpList> (); // one HelpList entry per file

                // Read all HelpTextFiles. For each make a list of its HelpTopic nodes
                foreach (string filename in helpTextFiles)
                    lst.Add (new HelpList (filename));




                foreach (HelpList hl in lst)
                    if (hl.lst.Count > 0)
                        TreeView.Items.Add (hl.lst [0]);




                TreeViewItem tvi = new TreeViewItem ();
                tvi.Header = "Contents";
                TreeView.Items.Add (tvi);

                foreach (HelpList hl in lst)
                    if (hl.lst.Count > 0)
                        tvi.Items.Add (hl.TableOfContents ());
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception in HelpWindow WindowLoaded: " + ex.Message);
            }
        }

        //private void HelpTextClearButton_Click (object sender, RoutedEventArgs e)
        //{
        //    HelpTextBlock.Inlines.Clear ();
        //    HelpTextClearButton.IsEnabled = false;
        //    (TreeView.SelectedItem as TreeViewItem).IsSelected = false;
        //}

        private void Window_Closed (object sender, EventArgs e)
        {
            HelpWindowManager.DeleteWindow (this);
        }
    }
}
