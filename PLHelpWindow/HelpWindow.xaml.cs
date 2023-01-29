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
            "GeneralPlotting.xml",
            "PlotFigures.xml",
            "MathFunctions.xml",
        };

        internal HelpWindow ()
        {
            InitializeComponent ();
        }

        internal bool SearchFor (string str)
        {
            List<HelpTreeViewItem> path = new List<HelpTreeViewItem> ();
            bool found = false;

            foreach (HelpTreeViewItem htvi in TreeView.Items)
            {
                found = htvi.Contains (str, ref path);
                if (found == true)
                    break;
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
            HelpTreeViewItem.textPane     = HelpTextBlock;
            HelpTreeViewItem.scrollViewer = HelpTextScroller;
            HelpTreeViewItem.clearButton  = HelpTextClearButton;

            try
            {
                List<HelpList> lst = new List<HelpList> ();

                foreach (string filename in helpTextFiles)
                    lst.Add (new HelpList (filename));

                foreach (HelpList hl in lst)
                    TreeView.Items.Add (hl.lst [0]);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception in HelpWindow WindowLoaded: " + ex.Message);
            }
        }

        private void HelpTextClearButton_Click (object sender, RoutedEventArgs e)
        {
            HelpTextBlock.Inlines.Clear ();
            HelpTextClearButton.IsEnabled = false;
            (TreeView.SelectedItem as TreeViewItem).IsSelected = false;
        }

        private void Window_Closed (object sender, EventArgs e)
        {
            HelpWindowManager.DeleteWindow (this);
        }
    }
}
