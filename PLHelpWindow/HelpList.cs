using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

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

            catch (Exception ex)
            {
                Console.WriteLine ("Exception reading Help File " + filename + ": " + ex.Message);
            }
        }
    }
}
