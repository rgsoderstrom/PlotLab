using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal class FileSystem : IFileSystem
    {
        List<string> ScriptFiles   = new List<string> () {"Ex1" , "ShowSamples", "PlotTests"};
        List<string> FunctionFiles = new List<string> () {"F1Func" , "F2", "PlotVector"};
        
        public bool IsScriptFile   (string fileName) {return ScriptFiles.Contains (fileName);}
        public bool IsFunctionFile (string fileName) {return FunctionFiles.Contains (fileName);}

    }
}
