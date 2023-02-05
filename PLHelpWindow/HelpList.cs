using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Controls;
using Common;

//
// HelpList - a list of help tree topics
//      - ctor passed the full path of an xml file containing related help topics
//      - builds a list of HelpTreeViewItems
//

namespace PLHelpWindow
{
    internal class HelpList
    {
        //
        // List of all HelpTreeViewItems read from one file. The first one will be displayed
        // as an item in the top-level Help tree. AFter that the order doesn't matter
        //
        public List<HelpTreeViewItem> lst = new List<HelpTreeViewItem> ();

        //
        // Dictionary to find any HelpTopic in this HelpList if it is a subtopic of another topic
        //
        private Dictionary<string, HelpTreeViewItem> dict = new Dictionary<string, HelpTreeViewItem> ();

        //*********************************************************************************************

        public HelpList (string filename)
        {
            try
            {
                //
                // load .xml help file and look for some errors
                //
                XmlDocument xd = new XmlDocument ();

                // from https://stackoverflow.com/questions/2820384/reading-embedded-xml-file-c-sharp
                Stream str = GetType ().Assembly.GetManifestResourceStream ("PLHelpWindow.HelpTextFiles." + filename);

                if (str == null)
                    throw new FileNotFoundException (filename);

                xd.Load (str);

                string topNodeName = "HelpTree";

                XmlNodeList xmlNodelist = xd.SelectNodes (topNodeName);

                if (xmlNodelist == null)
                    throw new Exception ("nodelist == null");

                XmlNode top = xmlNodelist [0];

                if (top == null)
                    throw new Exception ("top == null");

                //
                // Next-level nodes for remainder of file are either "HelpTopic" or comment
                //

                foreach (XmlNode node in top.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "#comment":
                            break;

                        case "HelpTopic":
                        {
                            HelpTreeViewItem ht = new HelpTreeViewItem (node);
                            lst.Add (ht);
                            dict.Add (ht.SearchKey, ht);
                        }
                        break;

                        default:
                            break;
                    }
                }

                //
                // Fill-in subtopic links for each topic
                //
                foreach (HelpTreeViewItem htvi in lst)
                    htvi.LookUpSubtopics (dict);
            }

            catch (FileNotFoundException)
            {
                EventLog.WriteLine ("Help File not found: " + filename);
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception reading Help File " + filename + ": " + ex.Message);
            }
        }

        //*********************************************************************************************

        // Return Table Of Contents for this file

        public TreeViewItem TableOfContents ()
        {
            TreeViewItem t1 = new TreeViewItem ();
            t1.Header = lst [0].Title;

            for (int i=1; i<lst.Count; i++)
            {
                TreeViewItem t2 = new TreeViewItem ();
                t2.Header = lst [i].Title;
                t2.Tag = lst [i].SearchKey;
                t2.Selected += T2_Selected;
                t1.Items.Add (t2);
            }

            return t1;
        }

        private void T2_Selected (object sender, System.Windows.RoutedEventArgs e)
        {
            //(TreeView.SelectedItem as TreeViewItem).IsSelected = false;
            //(HelpTreeViewItem.win.TreeView.SelectedItem as TreeViewItem).IsSelected = false;


            TreeViewItem tvi = sender as TreeViewItem;
            string       key = tvi.Tag as string;

            HelpTreeViewItem.win.SearchFor (key);
        }
    }
}





