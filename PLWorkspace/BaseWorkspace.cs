using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLWorkspace
{
    internal class BaseWorkspace : WorkspaceBase
    {
        static int InstanceCounter = 0;

        internal BaseWorkspace (string name) : base (name)
        {
            if (++InstanceCounter > 1)
                throw new Exception ("Only one BaseWorkspace allowed");

           

        }
    }
}
