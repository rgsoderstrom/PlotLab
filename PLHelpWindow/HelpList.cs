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

//
// https://stackoverflow.com/questions/2820384/reading-embedded-xml-file-c-sharp
//

namespace PLHelpWindow
{
    internal class HelpList : List<HelpTreeViewItem>
    {
        //public string GetResourceTextFile (string filename)
        //{
        //    string result = string.Empty;

        //    //
        //    // Get a list of all resources
        //    //

        //    //string[] allResources = GetType ().Assembly.GetManifestResourceNames ();

        //    //foreach (string str in allResources)
        //    //    Console.WriteLine (str);
            
        //    try
        //    {
        //       using (Stream stream = this.GetType ().Assembly.GetManifestResourceStream ("utHelpWindow3.HelpTextFiles." + filename))
        //        {
        //            using (StreamReader sr = new StreamReader (stream))
        //            {
        //                result = sr.ReadToEnd ();
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        Console.WriteLine ("Exception reading embedded xml help file: " + ex.Message);
        //    }

        //    return result;
        //}


        public HelpList (string filename)
        {
            try
            {
                //
                // load .xml help file and look for some errors
                //
                XmlDocument xd = new XmlDocument ();

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
                            Add (ht);
                        }
                        break;

                        default:
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception reading Help File " + filename + ": " + ex.Message);
            }
        }
    }
}
