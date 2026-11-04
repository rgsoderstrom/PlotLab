using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCommon
{
    public class PLComplex : PLScalar
    {
     // public System.Numerics.Complex Data = new System.Numerics.Complex (0, 0);
        public CommonMath.Complex Data = new CommonMath.Complex (0, 0);

        //public double Real {get {return Data.Real;}      set {Data = new System.Numerics.Complex (value, Data.Imaginary);}}
        //public double Imag {get {return Data.Imaginary;} set {Data = new System.Numerics.Complex (Data.Real, value);}}
        public double Real {get {return Data.Real;}      set {Data = new CommonMath.Complex (value, Data.Imag);}}
        public double Imag {get {return Data.Imag;}      set {Data = new CommonMath.Complex (Data.Real, value);}}

        public PLComplex (double re, double im)
        {
            Real = re;
            Imag = im;
        }

        public PLComplex (string name, double re, double im)
        {
            Name = name;
            Real = re;
            Imag = im;
        }

        public PLComplex (double re)
        {
            Real = re;
            Imag = 0;
        }

        public override int    DataAsInteger {get {throw new Exception ("DataAsInteger not implemented for complex");}}
        public override double DataAsDouble  {get {throw new Exception ("DataAsDouble not implemented for complex");}}

        public PLComplex (CommonMath.Complex src)
        {
          //Data = new System.Numerics.Complex (src.Real, src.Imaginary);
            Data = new CommonMath.Complex (src.Real, src.Imag);
        }

        //public PLComplex (CommonMath.Complex src)
        //{
        //    Data = new CommonMath.Complex (src.Real, src.Imag);
        //  //Data = new System.Numerics.Complex (src.Real, src.Imag);
        //}

        public PLComplex (PLVariable src)
        {
            if (src is PLDouble)
            {
                Real = (src as PLDouble).Data;
                Imag = 0;
            }

            if (src is PLComplex)
            {
                Real = (src as PLComplex).Real;
                Imag = (src as PLComplex).Imag;
            }

            else
                throw new Exception ("Unsupported PLComplex ctor: " + src.GetType ().ToString ());
        }

        public double Magnitude
        {
            get {return Data.Magnitude;}
        }

        public double Angle // in radians
        {
            get {return Data.Angle;}
        }

        public PLCMatrix Mul (PLRMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);

            for (int i = 0; i<op2.Rows; i++)
                for (int j = 0; j<op2.Cols; j++)
                    results [i, j] = new PLComplex (Real * op2 [i, j], Imag * op2 [i, j]);

            return results;
        }

        public PLCMatrix Mul (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);

            for (int i = 0; i<op2.Rows; i++)
            {
                for (int j = 0; j<op2.Cols; j++)
                {
                    results [i, j] = new PLComplex (Real, Imag) * op2 [i, j] as PLComplex;
                }
            }

            return results;
        }


        public static PLComplex operator/ (double op1, PLComplex op2)
        {  
            double mag = op1 / op2.Magnitude;
            double ang = -1 * op2.Angle;
            PLComplex r = new PLComplex (mag * Math.Cos (ang), mag * Math.Sin (ang));
            return r;
        }

        public static PLComplex operator/ (PLComplex op1, double op2)
        {  
            double mag = op1.Magnitude / op2;
            double ang = op1.Angle;
            PLComplex r = new PLComplex (mag * Math.Cos (ang), mag * Math.Sin (ang));
            return r;
        }

        public static PLComplex operator/ (PLComplex op1, PLComplex op2)
        {  
            double mag = op1.Magnitude / op2.Magnitude;
            double ang = op1.Angle - op2.Angle;
            PLComplex r = new PLComplex (mag * Math.Cos (ang), mag * Math.Sin (ang));
            return r;
        }




        public PLComplex Add (PLComplex op2) {return new PLComplex (Data + op2.Data);}
        public PLComplex Add (PLDouble  op2) {return new PLComplex (Data + op2.Data);}
        public PLComplex Sub (PLComplex op2) {return new PLComplex (Data - op2.Data);}
        public PLComplex Sub (PLDouble  op2) {return new PLComplex (Real - op2.Data, Imag);}
        public PLComplex Mul (PLComplex op2) {return new PLComplex (Data * op2.Data);}
        public PLComplex Mul (PLDouble op2)  {return new PLComplex (Data * op2.Data);}
        public PLComplex Mul (double op2)    {return new PLComplex (Real * op2, Imag * op2);}
        public PLComplex Div (PLDouble  op2) {return new PLComplex (Real / op2.Data, Imag / op2.Data);}
        public PLComplex Div (PLComplex op2) {return new PLComplex (Data / op2.Data);}

        public PLComplex Pwr (PLDouble op2) 
        {
            double mag = Math.Pow (Magnitude, op2.Data);
            double ang = Angle * op2.Data;
            return new PLComplex (mag * Math.Cos (ang), mag * Math.Sin (ang));
        }

        //public PLCMatrix Div (PLCMatrix op2)
        //{
        //    return new PLCMatrix (Data / op2.Data);
        //}

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            return "PLComplex.ToString (cfmt) not implmented";
        }

        public override string ToString ()
        {
            string realStr = DoubleToString (Real);
            string imagStr = DoubleToString (Imag);

            return "(" + realStr + ", " + imagStr + ")";
        }
    }
}
