using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLHelpWindow
{
    static public class HelpWindowManager
    {
        static List<HelpWindow> HelpWindows = new List<HelpWindow> ();

        static public void LaunchNewHelpWindow ()
        {
            HelpWindow win = new HelpWindow ();
            HelpWindows.Add (win);
            win.Show ();
        }

        static public bool DisplayHelpTopic (string topic)
        {
            if (HelpWindows.Count == 0)
                LaunchNewHelpWindow ();

            bool found = HelpWindows [HelpWindows.Count - 1].SearchFor (topic);
            return found;
        }

        static internal void DeleteWindow (HelpWindow win)
        {
            HelpWindows.Remove (win);
        }
    }
}
