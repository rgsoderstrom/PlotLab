
/*
    Library.cs - model of the built-in function library for utInputLine
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class Library : ILibrary
    {
        List<string> builtInFunction = new List<string> () {"sin" , "log2", "sqrt", "sprintf"};

        public bool IsDefined (string funcName)
        {
            return builtInFunction.Contains (funcName);
        }

        //public bool IsBuiltInFunction (string funcName)
        //{
        //    return builtInFunction.Contains (funcName);
        //}
    }
}
