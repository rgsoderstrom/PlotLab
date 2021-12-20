using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Xml;

namespace utHelpWindow
{
    public class HelpTree
    {
        // find a HelpTreeNode from its SearchKey
        static public Dictionary<string, HelpTreeNode> HelpFromSearchKey = new Dictionary<string, HelpTreeNode> ();

        // find a SearchKey from a text string (typically typed in by user)
        static public Dictionary<string, string> SearchKeyFromText = new Dictionary<string,string> ();

        //************************************************************************************

        //
        // Instance variables
        //

        //
        // Subject and SearchKey for the entire tree
        //
        string? treeSubject = null;
        string? treeSearchKey = null;

        //
        // nodes initially read in to a list, then re-organized in to a tree
        //
        public List<HelpTreeNode> helpTreeNodeList = new List<HelpTreeNode> ();

        //************************************************************************************

        public HelpTree (string helpTreeFileName)
        {
            //
            // load .xml help file and look for some errors
            //
            XmlDocument xd = new XmlDocument ();

            xd.Load (helpTreeFileName);

            string topNodeName = "HelpTree";

            XmlNodeList? xmlNodelist = xd.SelectNodes (topNodeName);

            if (xmlNodelist == null)
                throw new Exception ("nodelist == null");

            XmlNode? top = xmlNodelist [0];

            if (top == null)
                throw new Exception ("top == null");

            //
            // read top-level attributes
            //
            if (top.Attributes == null)
                throw new Exception ("Top level \"HelpTree\" node must have Subject and SearchKey attributes");

            foreach (XmlAttribute attr in top.Attributes)
            {
                switch (attr.Name)
                {
                    case "Subject":
                        treeSubject = attr.Value;
                        break;

                    case "SearchKey":
                        treeSearchKey = attr.Value;
                        break;

                    default:
                        throw new Exception ("Unrecognized top-level attribute: " + attr.Name);
                }
            }

            if (treeSearchKey == null || treeSubject == null)
                throw new Exception ("Top level \"HelpTree\" node must have Subject and SearchKey attributes");

            ReadFileIntoNodeList (top);
            AddNodesToDictionarys ();
            FIllInChildReferences ();
            //BuildTreeView ();


        }

        //********************************************************************************************

        TextBox HelpTextPane;

        public TreeViewItem BuildTreeView (TextBox helpTextPane)
        {
            HelpTextPane = helpTextPane;

            TreeViewItem root = new TreeViewItem ();

            root.Header = helpTreeNodeList [0].Subject;
            root.Tag = helpTreeNodeList [0];

            root.Selected += TreeViewItem_Selected;



            foreach (HelpTreeNode node in helpTreeNodeList [0].childNodes)
            {
                node.BuildTreeView (root, TreeViewItem_Selected);    
            }

           

            return root;
        }

        private void TreeViewItem_Selected (object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TreeViewItem? tvi = sender as TreeViewItem;


            if (tvi != null)
            {
                HelpTreeNode? htn = tvi.Tag as HelpTreeNode;
                HelpTextPane.Text = "";

                if (htn != null)
                    foreach (string str in htn.Content)
                    {
                        string str2 = str.Substring (1, str.Length - 2);
                        HelpTextPane.Text += str2 + "\n";
                    }
            }
        }

        //********************************************************************************************

        private void AddNodesToDictionarys ()
        {
            foreach (HelpTreeNode hn in helpTreeNodeList)
            {
                string key = hn.SearchKey;
                string txt = hn.Subject;

                HelpFromSearchKey.Add (key, hn);
                SearchKeyFromText.Add (txt, key);
            }
        }

        private void FIllInChildReferences ()
        {
            foreach (HelpTreeNode hn in helpTreeNodeList)
            {
                foreach (HelpTreeNode.ChildLink cl in hn.childLinks)
                {
                    if (cl.SearchKey != null)
                    {
                        try
                        {
                            HelpTreeNode? child = HelpFromSearchKey [cl.SearchKey];
                            hn.childNodes.Add (child);
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine ("Exception: " + ex.Message);
                        }
                    }
                }
            }
        }

        //********************************************************************************************

        private void ReadFileIntoNodeList (XmlNode top)
        { 
            //
            // Top-level nodes for remainder of file are either "HelpTopic" or comment nodes
            //

            foreach (XmlNode node in top.ChildNodes)
            {
                switch (node.Name)
                {
                    case "#comment":
                        break;

                    case "HelpTopic":
                        HelpTreeNode htn = new HelpTreeNode (node);
                        helpTreeNodeList.Add (htn);
                        break;

                    default:
                        break;
                }
            }        
        }

        //*******************************************************************************************

        public override string ToString ()
        {
            string str = "";

            str += "Tree: " + treeSubject + ", SearchKey = " + treeSearchKey + '\n';

            foreach (HelpTreeNode htn in helpTreeNodeList)
                str += htn.ToString () + '\n';

            return str;
        }
    }
}
