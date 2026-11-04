using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCommon
{
    public class PLDouble : PLScalar
    {
        /*readonly*/ public double Data;

        public override int    DataAsInteger {get {return (int) Math.Round (Data);}}
        public override double DataAsDouble  {get {return Data;}}

        public PLDouble (double d)
        {
            Data = d;
        }

        public PLDouble (string name, double d)
        {
            Name = name;
            Data = d;
        }

        public PLDouble (PLVariable src)
        {
            PLDouble  pld = src as PLDouble;
            PLRMatrix mat = src as PLRMatrix;
            PLInteger igr = src as PLInteger;

            if (pld != null)
            {
                Data = pld.Data;
            }

            else if (mat != null)
            {
                if (mat.Rows == 1 && mat.Cols == 1)
                    Data = mat [0, 0];
                else
                    throw new Exception ("Error constructing PLDouble from PLRMatrix");
            }

            else if (igr != null)
            {
                Data = igr.Data;
            }

            else
                throw new Exception ("Unsupported PLDouble ctor: " + src.GetType ().ToString ());
        }

        public PLComplex Add (PLComplex op2)
        {
            return new PLComplex (Data + op2.Data.Real, op2.Data.Imag);
        }

        public PLCMatrix Add (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);

            for (int i = 0; i<op2.Rows; i++)
            {
                for (int j = 0; j<op2.Cols; j++)
                {
                    results [i, j] = new PLComplex (Data + op2 [i, j].Real, op2 [i, j].Imag);
                }
            }

            return results;
        }

        public PLCMatrix Mul (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);

            for (int i = 0; i<op2.Rows; i++)
            {
                for (int j = 0; j<op2.Cols; j++)
                {
                    results [i, j] = new PLComplex (Data * op2 [i, j].Real, Data * op2 [i, j].Imag);
                }
            }

            return results;
        }

        public PLDouble  Add (PLDouble  op2)  {return new PLDouble  (Data + op2.Data);}
        public PLDouble  Add (PLInteger op2)  {return new PLDouble  (Data + op2.Data);}

        public PLDouble  Sub (PLDouble  op2) {return new PLDouble  (Data - op2.Data);}
        public PLDouble  Sub (PLInteger op2) {return new PLDouble  (Data - op2.Data);}

        public PLRMatrix Sub (PLRMatrix  op2) {return new PLRMatrix  ("", Data - op2.Data); }
        public PLComplex Sub (PLComplex op2) {return new PLComplex (Data - op2.Real, -op2.Imag);}

        public PLDouble  Mul (double    op2) {return new PLDouble  (Data * op2);}
        public PLDouble  Mul (PLInteger op2) {return new PLDouble  (Data * op2.Data);}
        public PLDouble  Mul (PLDouble  op2) {return new PLDouble  (Data * op2.Data);}

        public PLRMatrix Mul (PLRMatrix  op2) {return new PLRMatrix  ("", Data * op2.Data); }
        public PLComplex Mul (PLComplex op2)  {return new PLComplex (Data * op2.Real, Data * op2.Imag);}

        public PLDouble  Div (PLDouble  op2)  {return new PLDouble   (Data / op2.Data);}
        public PLRMatrix Div (PLRMatrix  op2) {return new PLRMatrix  ("", Data / op2.Data);}
        public PLCMatrix Div (PLCMatrix  op2) {return new PLCMatrix  (Data / op2.Data);}

        public PLDouble  Pwr (PLDouble  op2) {return new PLDouble  (Math.Pow (Data, op2.Data));}
        public PLRMatrix  Pwr (PLRMatrix  op2) {return new PLRMatrix  ("", Data ^ op2.Data); }

        public PLComplex  Div (PLComplex  op2) 
        {
            double dm = op2.Magnitude; // denominator magnitude
            double da = op2.Angle;     // denominator angle, radians
            double rm = Data / dm; // results magnitude
            double ra = -da;       // results angle
            return new PLComplex (rm * Math.Cos (ra), rm * Math.Sin (ra));
        }

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            return Data.ToString ();
            //throw new Exception ("PLDouble.ToString (cfmt) not implemented");

            //int a = 8, b = 5; // defaults

            //List<string> cfmtParts = SplitFormatString (cfmt);

            //if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
            //{
            //    a = int.Parse (cfmtParts [0]);
            //    b = int.Parse (cfmtParts [1]);
            //}

            //string fmt = "{0," + a + ":0.";
            //for (int i = 0; i<b; i++) fmt += '#';
            //fmt += "}";

            //return string.Format (fmt, Data);
        }

        //*************************************************************************************
        //*************************************************************************************
        //*************************************************************************************

        public override string ToString ()
        {
            return DoubleToString (Data);
        }
    }
}
