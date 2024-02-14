using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Filtering.FIR;
using MathNet.Filtering;
using PLCommon;

//
// FIR Filters
//

namespace FunctionLibrary
{
    static public partial class SignalProcessing
    { 
        static Dictionary<int, OnlineFirFilter> firFilterCollection = new Dictionary<int, OnlineFirFilter> ();
        static int nextFirFlterHandle = 1;

        //**********************************************************************************************************
        //
        // [handle, coefficients] = CreateLPF (sampleRate, cutoffFreq)
        //
        static public PLVariable CreateLPF (PLVariable arg)
        {
            PLList args = arg as PLList;
            if (args == null) throw new Exception ("CreateLPF: FIR filter argument error");
            if (args.Count != 2) throw new Exception ("CreateLPF: FIR filter argument error");

            double [] filterCoefs;
            double sampleRate = (args [0] as PLDouble).Data;
            double cutoff = (args [1] as PLDouble).Data;

            // generate filter coefficients
            int thisFilterHandle = nextFirFlterHandle++;

            try
            {
                filterCoefs = MathNet.Filtering.FIR.FirCoefficients.LowPass (sampleRate, cutoff);
                OnlineFirFilter filter = new OnlineFirFilter (filterCoefs);

                firFilterCollection.Add (thisFilterHandle, filter);
            }

            catch (Exception ex)
            {
                throw new Exception ("Error creating FIR filter: " + ex.Message);
            }

            PLList lst = new PLList ();
            lst.Add (new PLInteger (thisFilterHandle));

            PLRMatrix coefs = new PLRMatrix (1, filterCoefs.Length);
            for (int i = 0; i<filterCoefs.Length; i++)
                coefs [0, i] = filterCoefs [i];
            lst.Add (coefs);

            return lst;
        }

        //**********************************************************************************************************
        //
        // results = RunFilter (handle, samples, decimation); % decimation optional, default = 1
        //
        static public PLVariable RunFilter (PLVariable arg)
        {
            // check number of arguments
            PLList args = arg as PLList;
            if (args == null)   throw new Exception ("RunFilter: FIR filter argument error");
            if (args.Count < 2) throw new Exception ("RunFilter: FIR filter argument error");
            if (args.Count > 3) throw new Exception ("RunFilter: FIR filter argument error");

            //
            // get the requested filter
            //
            int handle;
            
            if      (args [0] is PLDouble)  handle = (int) (args [0] as PLDouble).Data;
            else if (args [0] is PLInteger) handle = (int) (args [0] as PLInteger).Data;
            else if (args [0] is PLRMatrix)  handle = (int) (args [0] as PLRMatrix).Data [0, 0];
            else
                throw new Exception ("RunFilter: handle argument type not supported");

            if (firFilterCollection.ContainsKey (handle) == false)
                throw new Exception ("FIR Filter not found");
            
            OnlineFilter filter = firFilterCollection [handle];

            //
            // get input samples in the format OnlineFilter wants            
            //

            if (args [1] is PLCMatrix)
                throw new Exception ("RunFilter - complex input not supported");

            PLRMatrix samples = args [1] as PLRMatrix;

            if (samples.IsRowVector == false)
                throw new Exception ("FirFilter, \"samples\": Only row vectors supported");

            double [] samples2 = new double [samples.Cols];

            for (int i = 0; i<samples.Cols; i++)
                samples2 [i] = samples [0, i];

            //
            // get decimation, if specified
            //
            int decimation = 1;

            if (args.Count == 3)
                decimation = (int) (args [2] as PLDouble).Data;

            //
            // run filter
            //
            double [] filtered = filter.ProcessSamples (samples2);

            //
            // put output in PlotLab format
            //
            PLRMatrix results = new PLRMatrix (1, filtered.Length / decimation);

            for (int i = 0; i<results.Cols; i++)
                results.Data [0, i] = filtered [i * decimation];

            return results;
        }

        //**********************************************************************************************************
        //
        // ClearFilter (handle);
        //  - clears history, not coefficients
        //
        static public PLVariable ClearFilter (PLVariable arg)
        {
            // get the requested filter
            int handle;
            
            if      (arg is PLDouble)  handle = (int) (arg as PLDouble).Data;
            else if (arg is PLInteger) handle = (int) (arg as PLInteger).Data;
            else if (arg is PLRMatrix)  handle = (int) (arg as PLRMatrix).Data [0, 0];
            else
                throw new Exception ("ClearFilter: argument type not supported");
                
            if (firFilterCollection.ContainsKey (handle) == false)
                throw new Exception ("ClearFilter: filter not found");
            
            OnlineFilter filter = firFilterCollection [handle];
            filter.Reset ();

            PLDouble results = new PLDouble (1);
            return results;
        }

        //**********************************************************************************************************
        //
        // DeleteFilter (handle);
        //
        static public PLVariable DeleteFilter (PLVariable arg)
        {
            // get the requested filter
            int handle;
            
            if      (arg is PLDouble)  handle = (int) (arg as PLDouble).Data;
            else if (arg is PLInteger) handle = (int) (arg as PLInteger).Data;
            else if (arg is PLRMatrix)  handle = (int) (arg as PLRMatrix).Data [0, 0];
            else
                throw new Exception ("DeleteFilter: argument type not supported");
                
            if (firFilterCollection.ContainsKey (handle) == false)
                throw new Exception ("DeleteFilter: filter not found");

            firFilterCollection.Remove (handle);

            PLDouble results = new PLDouble (1);
            return results;
        }

    }
}
