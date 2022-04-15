using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.Xml;

namespace utHelpWindow
{
//************************************************************************

    public class HelpTreeNode
    {
        public string Subject = "";
        public string SearchKey = "";

        public List<string> Content = new List<string>();

        public class ChildLink
        {
            public string? Subject = null;
            public string? SearchKey = null;

            public override string ToString ()
            {
                string sub = (Subject == null)   ? "???" : Subject;
                string key = (SearchKey == null) ? "???" : SearchKey;

                return sub + ", " + key; 
            }
        }

        public List<ChildLink>    childLinks = new List<ChildLink>    (); // text read from xml file
        public List<HelpTreeNode> childNodes = new List<HelpTreeNode> (); // filled in when tree built

        //************************************************************************

        public void BuildTreeViewNode (TreeViewItem tree, string? subject, RoutedEventHandler select)
        {
            TreeViewItem node = new TreeViewItem ();
            tree.Items.Add (node);

            node.Header    = subject;
            node.Tag       = this;
            node.Selected += select;
            node.Expanded += select;

            for (int i = 0; i<childLinks.Count; i++)
            {
                childNodes [i].BuildTreeViewNode (node, childLinks [i].Subject, select);
            }
        }

        //************************************************************************

        public HelpTreeNode ()
        {
            Subject = "Not initialized";
        }


        public HelpTreeNode (XmlNode xml)
        {
            if (xml.Attributes == null)
                throw new Exception ("Attributes == null");

            if (xml.Attributes.Count == 0)
                throw new Exception ("Attribute \"SearchKey\" required");

            //
            // read topic attributes
            //

            foreach (XmlAttribute attr in xml.Attributes)
            {
                switch (attr.Name)
                {
                    case "Subject":
                        Subject = attr.Value;
                        break;

                    case "SearchKey":
                        SearchKey = attr.Value;
                        break;

                    default:
                        throw new Exception ("Unrecognized in attribute in HelpTopic: " + attr.Name);
                }
            }

            if (xml.ChildNodes.Count == 0)
                throw new Exception ("Help node " + "\"" + Subject + "\"" + " has no content");

            //
            // examine child nodes
            //

            foreach (XmlNode childNode in xml.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Content":
                        foreach (XmlNode textNode in childNode.ChildNodes)
                        {
                            Content.Add (textNode.InnerText);
                        }

                        break;

                    case "SubTopics":
                        foreach (XmlNode subtopicNode in childNode.ChildNodes)
                        {
                            if (subtopicNode.Attributes != null)
                            {
                                ChildLink link = new ChildLink ();

                                foreach (XmlAttribute attr in subtopicNode.Attributes)
                                {
                                    switch (attr.Name)
                                    {
                                        case "Subject":
                                            link.Subject = attr.Value;
                                            break;

                                        case "SearchKey":
                                            link.SearchKey = attr.Value;
                                            break;

                                        default:
                                            throw new Exception ("Unrecognized SubTopic attribute: " + attr.Name);
                                    }
                                }

                                if (link.Subject == null || link.SearchKey == null)
                                    throw new Exception ("Incomplete subtopic link in " + Subject);

                                childLinks.Add (link);
                            }
                        }
                        break;

                    case "#comment":
                        break;

                    default:
                        throw new Exception ("Unrecognized node: " + childNode.Name);
                }
            }
        }

        //***********************************************************************

        public override string ToString ()
        {
            string str = "HelpTreeNode" + "\n";

            str += "Subject: " + Subject + "\n";
            str += "SearchKey: " + SearchKey + "\n";
            str += "Content:\n";

            foreach (string s in Content)
                str += s + "\n";

            str += "Children:\n";

            foreach (ChildLink cl in childLinks)
                str += cl.ToString () + "\n";

            return str;
        }
    }
}


