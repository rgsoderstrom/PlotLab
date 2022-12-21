using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

//
// NOTE: Very little error checking here
//

namespace PLHelpWindow
{
        //  <HelpTopic SearchKey="this item's search key">
        //      <Header>
        //          Header, stored in TreeViewItem base class 
        //      </Header>
        //
        //      <Content>
        //          <Title>
        //             This item's title, displayed in bold underlined in TextPane
        //          </Title>
        //
        //          <Text>
        //              Displayed in normal font as a paragraph in TextPane
        //          </Text>
        //      </Content>
        //
        //      <SubTopics>
        //          <SubTopic SearchKey="key1"/>  <!-- HelpTreeViewItems that match these will -->
        //          <SubTopic SearchKey="key2"/>  <!-- be added to the "Items" list in TreeViewItem -->
        //          <SubTopic SearchKey="key3"/>
        //          <SubTopic SearchKey="key4"/>
        //      </SubTopics>
        // </HelpTopic>

    internal class HelpTreeViewItem : TreeViewItem
    {
        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************
        //
        // Static 
        //
        
        // WPF items in HelpWindow
        public static TextBlock    textPane;
        public static ScrollViewer scrollViewer;
        public static Button       clearButton;

        private static Dictionary<string, HelpTreeViewItem> dict = new Dictionary<string, HelpTreeViewItem> ();

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************
        //
        // Instance
        //

        string title = "??";

        public string Title 
        {
            get {return title; }            
            protected set {title = value;}            
        }

        string text = "??";

        public string Text 
        {
            get {return text; }            
            protected set {text = value;}            
        }

        private List<string> subtopicKeys = new List<string> ();

        private string SearchKey = "??";

        internal HelpTreeViewItem (XmlNode xml)
        {
            try
            { 
                if (xml.Attributes == null || xml.Attributes.Count == 0)
                    throw new Exception ("HelpTreeViewItem \"SearchKey\" required");

                if (xml.Attributes.Count > 1)
                    throw new Exception ("Only one attribute expected");

                SearchKey = xml.Attributes [0].Value;

                foreach (XmlNode childNode in xml.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "Header":
                            Header = childNode.InnerText;
                            break;

                        case "Content":
                            Title = childNode.FirstChild.InnerText;
                            Text  = RemoveLeadingSpaces (childNode.LastChild.InnerText);
                            break;

                        case "SubTopics":
                            XmlNodeList ch = childNode.ChildNodes;

                            foreach (XmlNode xn in ch)
                                subtopicKeys.Add (xn.Attributes [0].InnerText);

                            break;

                        case "#comment":
                            break;

                        default:
                            throw new Exception ("Unrecognized node: " + childNode.Name);
                    }
                }

                Selected += Tvi_Selected;

                //
                // add this HelpTreeViewItem to the dictionary so others can
                // link to it.
                //
                dict.Add (SearchKey, this);
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }
        }

        //********************************************************************************************

        private string RemoveLeadingSpaces (string str)
        {
            string formatted = "";
            string [] sep = new string [] { "\r\n" };

            string [] lines = str.Split (sep, StringSplitOptions.None);

            int padding = 0;

            for (int i = 0; i<lines [1].Length; i++)
            {
                if (char.IsWhiteSpace (lines [1][i]) == false)
                    break;

                padding++;
            }

         //   if (padding > 0) padding--;

            for (int i = 1; i<lines.Length; i++)
            {
                string str2 = lines [i].Length > padding ? lines [i].Substring (padding) : lines [i];
                formatted += str2 + "\n";
            }

            return formatted;
        }

        //********************************************************************************************

        public void LookUpSubtopics ()
        {
            foreach (string key in subtopicKeys)
            {
                try
                {
                    HelpTreeViewItem htvi = dict [key];
                    Items.Add (htvi);
                }

                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine ("Key not found: " + key);
                }

                catch (Exception ex)
                {
                    Console.WriteLine ("Exception: " + ex.Message);
                }
            }
        }

        //********************************************************************************************

        void Tvi_Selected (object obj, RoutedEventArgs args)
        {
            try
            {
                args.Handled = true;

                Run run = new Run (Title + "\r\n");
                run.FontWeight = FontWeights.SemiBold;
                run.FontSize = 16;
                run.TextDecorations.Add (TextDecorations.Underline);
                textPane.Inlines.Add (run);

                run = new Run (Text);
                textPane.Inlines.Add (run);

                scrollViewer.ScrollToBottom ();
                clearButton.IsEnabled = true;
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception in Tvi_Selected: " + ex.Message);
            }
        }
    }
}