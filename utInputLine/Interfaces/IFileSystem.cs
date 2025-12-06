using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public interface IFileSystem
    {
        bool IsScriptFile (string fileName);
        bool IsFunctionFile (string fileName);


    }
}
