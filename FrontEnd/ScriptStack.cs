//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using PLCommon;
//using PLWorkspace;

//namespace FrontEnd
//{
//    static internal class ScriptStack
//    {
//        static private List<ScriptProcessor> RunningScripts = new List<ScriptProcessor> ();

//        static ScriptStack ()
//        {
//        }

//        public static void Add (string fileName, Workspace wksp, PrintFunction pr)
//        {
//            ScriptProcessor sp = new ScriptProcessor (wksp, pr);
//            ScriptProcessor.ScriptTerminationReason reason = sp.FindAndRunScript (fileName);

//        }


//    }
//}
