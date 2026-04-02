using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLFileSystem
{
    static internal class FileSystem
    {
        static List<string> ScriptFiles   = new List<string> () {"Ex1" , "ShowSamples", "PlotTests"};
        static List<string> FunctionFiles = new List<string> () {"F1Func" , "F2", "PlotVector"};
        
        static public bool IsScriptFile   (string fileName) {return ScriptFiles.Contains (fileName);}
        static public bool IsFunctionFile (string fileName) {return FunctionFiles.Contains (fileName);}

    }
}
