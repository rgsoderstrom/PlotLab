using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

using CommonMath;
using PLCommon;

namespace FunctionLibrary
{
    public static partial class SignalProcessing
    {
        //
        // FFT - real or complex input, length must be power of 2
        //     - results = FFT (realVector);
        //     - results = FFT (realVector, sampleRate);
        //     - results = FFT (complexVector);
        //     - results = FFT (complexVector, sampleRate);
        //
        //     - retuned results is 3 rows of N columns where N is sample count
        //      - row 1 is frequency scale if sample rate is specified, else it is bin number
        //      - row 2 is magnitude squared spectrum
        //      - row 3 is phase angle of spectrum
        //

        static public PLVariable FFT (PLVariable arg)
        {
            //
            // unpack input
            //
            bool returnFreqScale = false;
            double sampleRate = 0; // in samples per second
            int length;

            PLRMatrix  realInput = null; 
            PLCMatrix complexInput = null;

            try
            {
                if (arg is PLList) // then sample rate is specified
                {
                    PLList lst = arg as PLList;
                    if (lst [0].IsVector == false) 
                        throw new Exception ("Input signal must be vector");

                    length = lst [0].Size;

                    sampleRate = (lst [1] as PLDouble).Data;
                    returnFreqScale = true;
                    realInput    = lst [0] as PLRMatrix;  // only one of these will be non-null
                    complexInput = lst [0] as PLCMatrix;

                    if (realInput == null && complexInput == null)
                        throw new Exception ("Unsupported in type " + lst [0].GetType ());
                }

                else if (arg is PLRMatrix) // real input
                {
                    realInput = arg as PLRMatrix;
                    if (realInput.IsVector == false) throw new Exception ("Input signal must be vector");
                    length = realInput.Size;
                }

                else if (arg is PLCMatrix) // complex input
                {
                    complexInput = arg as PLCMatrix;
                    if (complexInput.IsVector == false) throw new Exception ("Input signal must be vector");
                    length = complexInput.Size;
                }

                else
                    throw new Exception ("unreconized input: " + arg.GetType ());
            }

            catch (Exception ex)
            {
                throw new Exception ("FFT Input error: " + ex.Message);
            }

            //****************************************************************
            //
            // run appropriate transform
            //

            PLRMatrix results;

            // buffers for input to and output from Math.Net functions
            if (complexInput != null) // then is complex in
            {
                //ComplexFourierTransformation cft = new ComplexFourierTransformation ();
                System.Numerics.Complex [] workBuffer = new System.Numerics.Complex [length];

                for (int i = 0; i<length; i++)
                    workBuffer [i] = new System.Numerics.Complex (complexInput [i].Real, (float) complexInput [i].Imag);

                Fourier.Forward (workBuffer);
                results = FormatResults (workBuffer, returnFreqScale, sampleRate);
            }

            else // real input
            {
                int sampleCount = realInput.Cols;
                int pad = sampleCount.IsEven () ? 2 : 1;

                double [] fftReal    = new double [sampleCount];
                double [] fftImag    = new double [sampleCount];
                double [] workBuffer = new double [sampleCount + pad]; // before FFT: input signal
                                                                       // after FFT: half of complex spectrum

                for (int i=0; i<sampleCount; i++)
                    workBuffer [i] = realInput [i];

                Fourier.ForwardReal (workBuffer, sampleCount, FourierOptions.NoScaling);

                int put = 0;
                
                for (int k=0; k<workBuffer.Length; k+=2, put++)
                { 
                    fftReal [put] = workBuffer [k];
                    fftImag [put] = workBuffer [k+1];
                }

                put = fftReal.Length - 1;

                for (int k = 2; k<workBuffer.Length; k+=2, put--)
                {
                    fftReal [put] = workBuffer [k];
                    fftImag [put] = workBuffer [k+1] * -1;
                }

                results = FormatResults (fftReal, fftImag, returnFreqScale, sampleRate);
            }

            return results;
        }

        //********************************************************************************

        private static PLRMatrix FormatResults (System.Numerics.Complex [] workBuffer, bool doFreqScale, double sampleRate)
        {
            int length = workBuffer.Length;
            PLRMatrix results = new PLRMatrix (3, length);

            double [] frequencyScale = doFreqScale ? Fourier.FrequencyScale (length, sampleRate) : GenerateBinCount (length);

            // copy to output buffer, swapping halves
            int put = 0;
            int L2 = 1 + length / 2;

            for (int i = L2; i<length; i++, put++)
            {
                results [0, put] = frequencyScale [i];
                results [1, put] = PowerSpectrum (workBuffer [i].Real, workBuffer [i].Imaginary, length);
                results [2, put] = PhaseAngle    (workBuffer [i].Real, workBuffer [i].Imaginary);
            }

            for (int i = 0; i<L2; i++, put++)
            {
                results [0, put] = frequencyScale [i];
                results [1, put] = PowerSpectrum (workBuffer [i].Real, workBuffer [i].Imaginary, length);
                results [2, put] = PhaseAngle    (workBuffer [i].Real, workBuffer [i].Imaginary);
            }

            return results;
        }

        //********************************************************************************

        private static PLRMatrix FormatResults (double [] real, double [] imag, bool doFreqScale, double sampleRate)
        {
            int length = real.Length;
            PLRMatrix results = new PLRMatrix (3, length);

            double [] frequencyScale = doFreqScale ? Fourier.FrequencyScale (length, sampleRate) : GenerateBinCount (length);

            int L2 = 1 + length / 2;
            int put = 0;

            for (int i = L2; i<length; i++, put++)
            {
                results [0, put] = frequencyScale [i];
                results [1, put] = PowerSpectrum (real [i], imag [i], length);
                results [2, put] = PhaseAngle (real [i], imag [i]);
            }

            for (int i = 0; i<L2; i++, put++)
            {
                results [0, put] = frequencyScale [i];
                results [1, put] = PowerSpectrum (real [i], imag [i], length);
                results [2, put] = PhaseAngle (real [i], imag [i]);
            }

            return results;
        }

        //********************************************************************************

        private static double [] GenerateBinCount (int length)
        {
            double [] results = new double [length];

            for (int i = 0; i<length; i++)
                results [i] = i;

            return results;
        }

        private static double PowerSpectrum (double re, double im, double len)
        {
            return (re * re + im * im) / len;
        }

        private static double PhaseAngle (double re, double im)
        {
            return Math.Atan2 (im, re);
        }

        //********************************************************************************

    }
}
