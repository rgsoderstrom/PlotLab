using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;

namespace utHelpWindow2
{
    public class HelpTreeView : TreeView
    {
        public TextBlock?     HelpTextPane;
        private Button?       ClearButton;
        private ScrollViewer? HelpTextScroller;

        public void Fill (List<HelpTreeNodeList> allHelpLists, TextBlock helpContentTextBlock, Button clearButton, ScrollViewer scroll)
        {
            HelpTextPane     = helpContentTextBlock;
            ClearButton      = clearButton;
            HelpTextScroller = scroll;

            foreach (HelpTreeNodeList htl in allHelpLists)
            {
                TreeViewItem tvi = new TreeViewItem ();
                Items.Add (tvi);

                tvi.Header = htl.Subject;
                tvi.Tag = htl [0];
                tvi.Selected += Tvi_Selected;
                //tvi.Expanded += Tvi_Selected;

                //
                // helpTreeNodeList contains all the nodes in the file. 
                // the first one is the root, all others descend from it.
                //

                for (int i=0; i<htl [0].childLinks.Count; i++)
                {
                    string? key = htl [0].childLinks [i].SearchKey;

                    if (key != null)
                    {
                        HelpTreeNode? node = HelpTreeNode.GetNodeFromSearchKey (key);

                        if (node != null)
                            tvi.Items.Add (node.BuildTreeViewItem (Tvi_Selected));
                    }
                }
            }
        }

        //*************************************************************************************

        private void Tvi_Selected (object sender, System.Windows.RoutedEventArgs e)
        {
            e.Handled = true;

            TreeViewItem? tvi = sender as TreeViewItem;

            if (tvi != null)
            {
                if (tvi.Tag != null)
                {
                    if (ClearButton != null)
                        ClearButton.IsEnabled = true;

                    HelpTreeNode? htn = tvi.Tag as HelpTreeNode;

                    if (htn != null && HelpTextPane != null)
                    {
                        if (htn.Subject != null)
                        {
                            Run run = new Run (htn.Subject + "\r\n");
                            run.FontWeight = FontWeights.SemiBold;
                            run.FontSize = 16;
                            run.TextDecorations.Add (TextDecorations.Underline);
                            HelpTextPane.Inlines.Add (run);
                        }

                        for (int i = 0; i<htn.Content.Count; i++)
                        {
                            string str = htn.Content [i];
                            string str2 = str.Substring (1, str.Length - 2);
                            Run run = new Run (str2 + "\r\n");
                            run.FontSize = i == 0 ? 16 : 14;
                            HelpTextPane.Inlines.Add (run);
                        }

                        if (HelpTextScroller != null)
                            HelpTextScroller.ScrollToBottom ();
                    }
                }
            }
        }

        //*************************************************************************************

        public void ClearHelpText ()
        {
            if (HelpTextPane != null) HelpTextPane.Inlines.Clear ();
            if (ClearButton != null)  ClearButton.IsEnabled = false;
        }
    }
}
