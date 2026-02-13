using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace Main
{
    public interface IWorkspace
    {
        bool IsDefined (string variableName);
        PLVariable Get (string name);
    }
}
