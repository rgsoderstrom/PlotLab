
/*
    ILibrary.cs - 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public interface ILibrary
    {
        bool IsDefined (string funcName);
     // bool IsBuiltInFunction (string funcName);
    }
}
