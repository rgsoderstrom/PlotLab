using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PLCommon;

using Plot2D_Embedded;
using Plot3D_Embedded;
using PlottingLib;

//***************************************************************************************************

// "PL" wrappers for CanvasObject and ViewportObject so they can be passed to internal functions
// the same way matrices, vectors, etc. are

namespace PLCommon
{
    public abstract class PLDisplayObject : PLScalar
    {

    }

    public class PLCanvasObject : PLDisplayObject
    {
        readonly public Plot2D_Embedded.CanvasObject Data;

        public PLCanvasObject (CanvasObject co)
        {
            Data = co;
        }

        public override string ToString (string fmt)
        {
            return "PLCanvasObject";
        }
    }

    public class PLViewportObject : PLDisplayObject
    {
        readonly public Plot3D_Embedded.ViewportObject Data;

        public PLViewportObject (ViewportObject co)
        {
            Data = co;
        }

        public override string ToString (string fmt)
        {
            return "PLViewportObject";
        }
    }
}

