using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

//
// HelpList - a list of help tree topics
//      - ctor passed the full path of an xml file containing related help topics
//      - builds a list of HelpTreeViewItems
//

namespace utHelpWindow3
{
    internal class HelpList : List<HelpTreeViewItem>
    {
        public HelpList (string filename)
        {            
            //
            // load .xml help file and look for some errors
            //
            XmlDocument xd = new XmlDocument ();

            xd.Load (filename);

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
                        Add (ht);
                    }
                    break;

                    default:
                        break;
                }
            }
        }
    }
}
