using System;
using System.Collections.Generic;

using PLCommon;

//using PerformanceAnalysis.FFT;
//using PerformanceAnalysis.LinearAlgebra;
//using MathNet.Numerics.Transformations;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Filtering;
using System.IO;
using MathNet.Filtering.FIR;
//using static System.Net.WebRequestMethods;

namespace FunctionLibrary
{
    static public partial class SignalProcessing
    {
        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static public Dictionary<string, PLFunction> GetSignalProcessingContents ()
        {
            return new Dictionary<string, PLFunction> 
            {
                {"CreateLPF",    CreateLPF},
                {"CreateFIR",    CreateFIR},
                {"RunFilter",    RunFilter},
                {"ClearFilter",  ClearFilter},
                {"DeleteFilter", DeleteFilter},
                {"fft",          FFT }
            };
        }



    }


}
