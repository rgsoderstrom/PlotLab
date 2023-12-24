using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Filtering.FIR;
using MathNet.Numerics.Transformations;

namespace FunctionLibrary
{
    public static partial class SignalProcessing
    {
        // these are allocated one time when first needed
        static RealFourierTransformation realFft = null;
        static ComplexFourierTransformation complexFft = null;



    }
}
