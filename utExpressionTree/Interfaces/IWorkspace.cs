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
        bool Contains  (string variableName);
        bool IsDefined (string variableName);
        PLVariable Get (string name);
        void Add (PLVariable var);    
        void OverwriteSubmatrix (string name,            // name of matrix already in workspace
                                 int tlcRow, int tlcCol, // 1-based
                                 PLVariable var);        // new data to overwrite some of old
    }
}
