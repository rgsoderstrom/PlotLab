
/*
    DefaultWorkspace
*/

using System;

namespace PLWorkspace
{
    internal class DefaultWorkspace : WorkspaceBaseClass
    {
        static int InstanceCounter = 0;

        internal DefaultWorkspace (string name) : base (name)
        {
            if (++InstanceCounter > 1)
                throw new Exception ("Only one DefaultWorkspace allowed");
        }
    }
}
