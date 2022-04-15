using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Common;

namespace utHelpWindow2
{
    public class HelpTreeNodeList : List<HelpTreeNode>
    {
        //
        // Subject and SearchKey for this entire list
        //
        public string Subject;
        public string SearchKey;

        //*******************************************************************************************

        public HelpTreeNodeList (string helpTreeFileName)
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
                        Subject = attr.Value;
                        break;

                    case "SearchKey":
                        SearchKey = attr.Value;
                        break;

                    case "#comment":
                        break;

                    default:
                        throw new Exception ("Unrecognized top-level attribute: " + attr.Name);
                }
            }

            if (SearchKey == null || Subject == null)
                throw new Exception ("Top level \"HelpTree\" node must have Subject and SearchKey attributes");

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
                    {
                        HelpTreeNode ht = new HelpTreeNode (node, helpTreeFileName);
                        Add (ht);
                    }
                    break;

                    default:
                        break;
                }
            }
        }

        //**********************************************************************************

        public override string ToString ()
        {
            string str = "Tree Subject: " + Subject;
            str += ", TreeSearchKey: " + SearchKey;

            string divider = "\n" + new string ('=', str.Length) + "\n";
            str += divider;

            str += "\t" + Count.ToString () + " topics\n";

            foreach (HelpTreeNode htn in this)
            {
                str += htn.ToString ();
                str += "--------------------------------------\n";
            }

            return str;
        }
    }
}
