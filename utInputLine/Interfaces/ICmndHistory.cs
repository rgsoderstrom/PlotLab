using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal interface ICmndHistory
    {
        string RecallBang (string   bangCmnd,     // !43 or !12:p
                           ref bool printOnly, 
                           ref bool error);

        string RecallText (string first); // user enters first few characters followed by up-arrow
        string RecallPrevious ();         // up arrow
        string RecallNext ();             // down arror
    }
}
