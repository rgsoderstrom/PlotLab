using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using System.Xml;

using Common;

//
// HelpTreeNode - contains all the information for one node
//                  - subject
//                  - search key
//                  - content
//                  - child links
//

namespace utHelpWindow2
{
    public class HelpTreeNode
    {
        //*******************************************************************************
        //
        // static variables and static ctor
        //

        static Dictionary<string, HelpTreeNode> dict = new Dictionary<string, HelpTreeNode> ();

        static internal HelpTreeNode? GetNodeFromSearchKey (string key)
        {
            try
            {
                return dict [key];
            }

            catch (Exception)
            {
                EventLog.WriteLine ("Exception: error finding node with key " + key);
                return null;
            }
        }

        //*******************************************************************************
        //
        // Instance variables
        //

        public string? Subject;   // this topic's subject
        public string? SearchKey; // key to find this topic

        public List<string> Content = new List<string> ();
        public List<SubtopicLink> childLinks = new List<SubtopicLink> ();

        //*************************************************************************************
        //
        // Information to describe and find subtopic HelpTreeNodes
        //

        public class SubtopicLink
        {
            public string? Subject = null;   // read from .xml file
            public string? SearchKey = null; // ditto

            public SubtopicLink (string sub, string key)
            {
                Subject = sub;
                SearchKey = key;
            }

            public SubtopicLink (string key)
            {
                SearchKey = key;
            }

            public override string ToString ()
            {
                string sub = (Subject == null) ? "???" : Subject;
                string key = (SearchKey == null) ? "???" : SearchKey;

                return sub + ", " + key;
            }
        }

        //************************************************************************
        //
        // Build TreeViewItem for this node
        //

        public TreeViewItem BuildTreeViewItem (RoutedEventHandler select)
        {
            TreeViewItem node = new TreeViewItem ();

            node.Header    = Subject;
            node.Tag       = this;
            node.Selected += select;
            //node.Expanded += select;

            for (int i = 0; i<childLinks.Count; i++)
            {
                if (childLinks [i] != null)
                {
                    string? key = childLinks [i].SearchKey;

                    if (key != null)
                    {
                        try
                        {
                            HelpTreeNode? childNode = GetNodeFromSearchKey (key);

                            if (childNode != null)
                                node.Items.Add (childNode.BuildTreeViewItem (select));
                        }

                        catch (Exception)
                        {
                            EventLog.WriteLine ("Exception: error finding node with key " + key);
                        }
                    }
                }
            }

            return node;
        }

        //*******************************************************************************
        //
        // Constructor - from xml file node
        //

        internal HelpTreeNode (XmlNode xml,
                               string fileName) // for error reporting only
        {
            try
            {
                if (xml.Attributes == null)
                    throw new Exception ("HelpTreeNode Attributes == null");

                if (xml.Attributes.Count == 0)
                    throw new Exception ("HelpTreeNode Attribute \"SearchKey\" required");

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
                            string str = (Subject != null) ? Subject + ", " + attr.Name + " = " + attr.Value 
                                                           : attr.Name + " = " + attr.Value;
                            throw new Exception ("Unrecognized in attribute in HelpTopic: " + str);
                    }
                }

                if (xml.ChildNodes.Count == 0)
                    throw new Exception ("Help Topic node " + "\"" + Subject + "\"" + " has no content");

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
                            string? formatted = FormatContent (textNode.InnerText);

                            if (formatted != null) 
                                Content.Add (formatted);
                        }

                        break;

                    case "SubTopics":
                        foreach (XmlNode subtopicNode in childNode.ChildNodes)
                        {
                            if (subtopicNode.Attributes != null)
                            {
                                string? subj = null, key = null;

                                foreach (XmlAttribute attr in subtopicNode.Attributes)
                                {
                                    switch (attr.Name)
                                    {
                                        case "Subject":
                                            subj = attr.Value;
                                            break;

                                        case "SearchKey":
                                            key = attr.Value;
                                            break;

                                        default:
                                            throw new Exception ("Unrecognized SubTopic attribute: " + attr.Name);
                                    }
                                }

                                if (key == null)
                                    throw new Exception ("Subtopic has no search key " + Subject);

                                if (subj == null)
                                    childLinks.Add (new SubtopicLink (key));
                                else
                                   childLinks.Add (new SubtopicLink (subj, key));
                            }
                        }
                        break;

                    case "#comment":
                        break;

                    default:
                        throw new Exception ("Unrecognized node: " + childNode.Name);
                }
            }

            if (SearchKey != null)
                dict.Add (SearchKey, this);
            }

            catch (Exception ex)
            {
                int index = fileName.LastIndexOf ('\\');
                string str = (index == -1) ? fileName : fileName.Remove (0, index + 1); // file name w/o path
                EventLog.WriteLine ("Error in file " + str + ": " + ex.Message);
            }
        }

        //*****************************************************************************
        //
        // For now, formatting just removes first line
        //
        private string? FormatContent (string str)
        {
            string? formatted = "";

            string [] lines = str.Split ("\r\n", StringSplitOptions.None);

            for (int i = 1; i<lines.Length; i++)
            {
                formatted += lines [i] + "\n";
            }

            return formatted;
        }

        //*****************************************************************************

        public override string ToString ()
        {
            string str = "HelpTreeNode" + "\n";

            str += "\tSubject: " + Subject + "\n";
            str += "\tSearchKey: " + SearchKey + "\n";
            str += "\tContent:\n";

            foreach (string s in Content)
                str += "\t\t" + s + "\n";

            str += "Children:\n";

            foreach (SubtopicLink cl in childLinks)
                str += "\t" + cl.ToString () + "\n";

            return str;
        }
    }
}
