
/*
    InputTypes.cs
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public partial class InputLineProcessor
    {
        //enum RawStringTypes
        //{ 
        //    Continued, // end in ...
        //    Compound,  // a=3; b=7; c=22;
        //}

        //enum AnnotatedStringTypes
        //{
        //    Partial, // made from a Continued

        //}

        //public enum BlockStartType
        //{
        //    BS_For, BS_While,
        //    BS_If, BS_Else, BS_Elseif,
        //}

        //public enum BlockEndType
        //{
        //    BE_End,
        //}

        public enum LineType
        {
            Unknown,
            ExpressionTree, 
            VariableName,
            ArrayInitStart,
            ArrayInitEnd,
            Command,
            Script,
            BlockStart,
            BlockEnd,
            Bang,
        }
    }
}
