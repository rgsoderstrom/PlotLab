using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLWorkspace
{
    public class BaseWorkspace : Workspace
    {
        static int InstanceCounter = 0;

        public BaseWorkspace ()
        {
            if (++InstanceCounter > 1)
            {
                throw new Exception ("Only one BaseWorkspace allowed");



            }
        }
    }
}
