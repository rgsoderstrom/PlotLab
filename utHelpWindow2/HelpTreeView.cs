using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;

namespace utHelpWindow2
{
    public class HelpTreeView : TreeView
    {
        public TextBox? HelpTextPane;

        public void Fill (List<HelpTreeNodeList> allHelpLists, TextBox helpContentTextBox)
        {
            HelpTextPane = helpContentTextBox;

            foreach (HelpTreeNodeList htl in allHelpLists)
            {
                TreeViewItem tvi = new TreeViewItem ();
                Items.Add (tvi);

                tvi.Header = htl.Subject;
                tvi.Tag = htl [0];
                tvi.Selected += Tvi_Selected;
                tvi.Expanded += Tvi_Selected;

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
                    HelpTreeNode? ht = tvi.Tag as HelpTreeNode;

                    if (ht != null && HelpTextPane != null)
                    {
                        HelpTextPane.Text = "";

                        if (ht.Subject != null)
                        {
                            HelpTextPane.AppendText (ht.Subject + '\n');
                            string underline = new string ('-', ht.Subject.Length);
                            HelpTextPane.AppendText (underline + '\n');
                        }

                        foreach (string str in ht.Content)
                        {
                            string str2 = str.Substring (1, str.Length - 2);
                            HelpTextPane.AppendText (str2 + '\n');
                        }
                    }
                }
            }
        }
    }
}
