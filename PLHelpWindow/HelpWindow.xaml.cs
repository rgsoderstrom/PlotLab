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

namespace PLHelpWindow
{
    public partial class HelpWindow : Window
    {

        List<string> helpTextFiles = new List<string> ()
        {
            "GeneralPlotting.xml",
            "MathFunctions.xml",
        };

        public HelpWindow ()
        {
            InitializeComponent ();
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
                    foreach (HelpTreeViewItem htvi in hl)
                        htvi.LookUpSubtopics ();

                foreach (HelpList hl in lst)
                    TreeView.Items.Add (hl [0]);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception in WindowLoaded: " + ex.Message);
            }
        }

        private void HelpTextClearButton_Click (object sender, RoutedEventArgs e)
        {
            HelpTextBlock.Inlines.Clear ();
            HelpTextClearButton.IsEnabled = false;
            (TreeView.SelectedItem as TreeViewItem).IsSelected = false;
        }
    }
}
