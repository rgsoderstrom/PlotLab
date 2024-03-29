﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Text;
using System.Xml;

using Common;

namespace utHelpWindow
{
    public class HelpTree
    {
        // find a HelpTreeNode from its SearchKey
        static public Dictionary<string, HelpTreeNode> HelpFromSearchKey = new Dictionary<string, HelpTreeNode> ();

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
                throw new Exception ("AAA Top level \"HelpTree\" node must have Subject and SearchKey attributes");

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

                    case "#comment":
                        break;

                    default:
                        throw new Exception ("Unrecognized top-level attribute: " + attr.Name);
                }
            }

        //    if (treeSearchKey == null || treeSubject == null)
         //       throw new Exception ("BBB Top level \"HelpTree\" node must have Subject and SearchKey attributes");

            ReadFileIntoNodeList (top);
            AddNodesToDictionarys ();
        }

        //********************************************************************************************

        TextBlock? HelpTextPane;

        public TreeViewItem BuildTreeView (TextBlock helpTextPane)
        {
            HelpTextPane = helpTextPane;

            TreeViewItem root = new TreeViewItem ();

            root.Header = treeSubject; // helpTreeNodeList [0].Subject;
            root.Tag = helpTreeNodeList [0];

            root.Selected += TreeViewItem_Selected;
            root.Expanded += TreeViewItem_Selected;

            //
            // helpTreeNodeList contains all the nodes in the file. 
            // the first one is the root, all others descend from it.
            //

            for (int i=0; i<helpTreeNodeList [0].childNodes.Count; i++)
            {
                HelpTreeNode node    = helpTreeNodeList [0].childNodes [i];
                string?      subject = helpTreeNodeList [0].childLinks [i].Subject;
                node.BuildTreeViewNode (root, subject, TreeViewItem_Selected);
            }

            return root;
        }

        private void TreeViewItem_Selected (object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            TreeViewItem? tvi = sender as TreeViewItem;

            if (tvi != null && HelpTextPane != null)
            {
                HelpTreeNode? htn = tvi.Tag as HelpTreeNode;
                
                HelpTextPane.Text = "";

                if (htn != null)
                {
                    if (htn.Subject != null)
                    {
                        Run run = new Run (htn.Subject + "\r\n");
                        run.FontWeight = FontWeights.SemiBold;
                        run.FontSize = 16;
                        run.TextDecorations.Add (TextDecorations.Underline);
                        HelpTextPane.Inlines.Add (run);
                    }

                    for (int i=0; i<htn.Content.Count; i++)
                    {
                        string str = htn.Content [i];
                        string str2 = str.Substring (1, str.Length - 2);
                        Run run = new Run (str2 + "\r\n");
                        run.FontSize = i == 0 ? 16 : 14;
                        HelpTextPane.Inlines.Add (run);
                    }
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
            }
        }

        public void FillInChildReferences ()
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

                        catch (KeyNotFoundException)
                        {
                            EventLog.WriteLine ("Subtopic key not found");
                            EventLog.WriteLine ("Key: " + cl.SearchKey);
                            EventLog.WriteLine ("Subject: " + cl.Subject);
                            EventLog.WriteLine ("Parent Subject: " + hn.Subject);
                            EventLog.WriteLine ("Tree Subject: " + treeSubject);

                            HelpTreeNode dummy = new HelpTreeNode ();
                            hn.childNodes.Add (dummy);
                        }

                        catch (Exception ex)
                        {
                            EventLog.WriteLine ("Exception filling in subtopic references: " + ex.Message);

                            HelpTreeNode dummy = new HelpTreeNode ();
                            hn.childNodes.Add (dummy);
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
