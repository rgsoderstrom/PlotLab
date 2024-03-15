
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Common;
using CommonMath;

namespace PLCommon
{
    public abstract class PLVariable
    {
        public string Name = "";
        public abstract string ToString (string fmt);

        public static double ToNumber (PLVariable plv, ref bool failFlag)
        {
            if (plv is PLDouble) return (plv as PLDouble).Data;
            if (plv is PLInteger) return (plv as PLInteger).Data;
            if (plv is PLMatrix) { PLRMatrix mat = plv as PLRMatrix; if (mat.Rows == 1 && mat.Cols == 1) return mat [0, 0]; }
            failFlag = true;
            return 0;
        }

        public virtual int Rows {get {return 1;}}
        public virtual int Cols {get {return 1;}}
        public virtual int Size {get {return 1;}}
        public virtual bool IsMatrix {get {return false;}}
        public virtual bool IsVector {get {return false;}}
        public virtual bool IsRowVector {get {return false;}}
        public virtual bool IsColVector {get {return false;}}

        public static PLVariable operator + (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLRMatrix && op2 is PLRMatrix) return (op1 as PLRMatrix).Add (op2 as PLRMatrix);
            if (op1 is PLRMatrix && op2 is PLCMatrix) return (op1 as PLRMatrix).Add (op2 as PLCMatrix);
            if (op1 is PLRMatrix && op2 is PLDouble)  return (op1 as PLRMatrix).Add (op2 as PLDouble);

            if (op1 is PLCMatrix && op2 is PLRMatrix) return (op2 as PLRMatrix).Add (op1 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLCMatrix) return (op1 as PLCMatrix).Add (op2 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLDouble)  return (op2 as PLDouble). Add (op1 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLComplex) return (op1 as PLCMatrix).Add (op2 as PLComplex);

            if (op1 is PLDouble  && op2 is PLRMatrix) return (op2 as PLRMatrix).Add (op1 as PLDouble);
            if (op1 is PLDouble  && op2 is PLCMatrix) return (op1 as PLDouble). Add (op2 as PLCMatrix);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble). Add (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble). Add (op2 as PLComplex);
            if (op1 is PLDouble  && op2 is PLInteger) return (op2 as PLInteger).Add (op1 as PLDouble);

            if (op1 is PLComplex && op2 is PLCMatrix) return (op2 as PLCMatrix).Add (op1 as PLComplex);
            if (op1 is PLComplex && op2 is PLDouble) return (op1 as PLComplex).Add (op2 as PLDouble);
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Add (op2 as PLComplex);

            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger).Add (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger).Add (op2 as PLInteger);

            if (op1 is PLString  && op2 is PLString)  return (op1 as PLString). Add (op2 as PLString);

            //return new PLString ("Addition of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
            throw new Exception ("Addition of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator- (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLRMatrix && op2 is PLRMatrix) return (op1 as PLRMatrix).Sub (op2 as PLRMatrix);
            if (op1 is PLRMatrix && op2 is PLCMatrix) return (op1 as PLRMatrix).Sub (op2 as PLCMatrix);
            if (op1 is PLRMatrix && op2 is PLDouble)  return (op1 as PLRMatrix).Sub (op2 as PLDouble);

            if (op1 is PLCMatrix && op2 is PLRMatrix) return (op1 as PLCMatrix).Sub (op2 as PLRMatrix);
            if (op1 is PLCMatrix && op2 is PLCMatrix) return (op1 as PLCMatrix).Sub (op2 as PLCMatrix);

            if (op1 is PLDouble  && op2 is PLRMatrix) return (op1 as PLDouble). Sub (op2 as PLRMatrix);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble). Sub (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble). Sub (op2 as PLComplex);

            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Sub (op2 as PLComplex);
            if (op1 is PLComplex && op2 is PLDouble)  return (op1 as PLComplex).Sub (op2 as PLDouble);

            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger).Sub (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger).Sub (op2 as PLInteger);

            //return new PLString ("Subtraction of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
            throw new Exception ("Subtraction of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator* (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLRMatrix && op2 is PLRMatrix) return (op1 as PLRMatrix).Mul (op2 as PLRMatrix);
            if (op1 is PLRMatrix && op2 is PLCMatrix) return (op1 as PLRMatrix).Mul (op2 as PLCMatrix);
            if (op1 is PLRMatrix && op2 is PLDouble)  return (op2 as PLDouble). Mul (op1 as PLRMatrix);
            if (op1 is PLRMatrix && op2 is PLComplex) return (op2 as PLComplex).Mul (op1 as PLRMatrix);

            if (op1 is PLCMatrix && op2 is PLRMatrix) return (op1 as PLCMatrix).Mul (op2 as PLRMatrix);
            if (op1 is PLCMatrix && op2 is PLCMatrix) return (op1 as PLCMatrix).Mul (op2 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLDouble) return (op2 as PLDouble).Mul (op1 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLComplex) return (op1 as PLCMatrix).Mul (op2 as PLComplex);

            if (op1 is PLDouble  && op2 is PLRMatrix) return (op1 as PLDouble).Mul (op2 as PLRMatrix);
            if (op1 is PLDouble  && op2 is PLCMatrix) return (op1 as PLDouble).Mul (op2 as PLCMatrix);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble).Mul (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble).Mul (op2 as PLComplex);
            if (op1 is PLDouble  && op2 is PLInteger) return (op1 as PLDouble).Mul (op2 as PLInteger);

            if (op1 is PLComplex && op2 is PLRMatrix) return (op1 as PLComplex).Mul (op2 as PLRMatrix);
            if (op1 is PLComplex && op2 is PLCMatrix) return (op1 as PLComplex).Mul (op2 as PLCMatrix);
            if (op1 is PLComplex && op2 is PLDouble) return (op1 as PLComplex).Mul (op2 as PLDouble);
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Mul (op2 as PLComplex);

            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger).Mul (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger).Mul (op2 as PLInteger);

            //return new PLString ("Multiplication of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
            throw new Exception ("Multiplication of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator/ (PLVariable op1, PLVariable op2)
        {  
            if (op1 is PLRMatrix && op2 is PLDouble)  return (op1 as PLRMatrix).Div (op2 as PLDouble);
            if (op1 is PLCMatrix && op2 is PLDouble)  return (op1 as PLCMatrix).Div (op2 as PLDouble);
            if (op1 is PLCMatrix && op2 is PLComplex) return (op1 as PLCMatrix).Div (op2 as PLComplex);
          //if (op1 is PLComplex && op2 is PLCMatrix) return (op1 as PLComplex).Div (op2 as PLCMatrix);

            if (op1 is PLDouble  && op2 is PLRMatrix) return (op1 as PLDouble). Div (op2 as PLRMatrix);
            if (op1 is PLDouble  && op2 is PLCMatrix) return (op1 as PLDouble). Div (op2 as PLCMatrix);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble). Div (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble). Div (op2 as PLComplex);

            if (op1 is PLComplex && op2 is PLDouble)  return (op1 as PLComplex).Div (op2 as PLDouble);
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Div (op2 as PLComplex);

            //return new PLString ("Division of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
            throw new Exception ("Division of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator^ (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLRMatrix  && op2 is PLRMatrix) return (op1 as PLRMatrix).Pwr (op2 as PLRMatrix);  // implments .^, element-by-element
            if (op1 is PLRMatrix  && op2 is PLDouble)  return (op1 as PLRMatrix).Pwr (op2 as PLDouble);
            if (op1 is PLDouble   && op2 is PLRMatrix) return (op1 as PLDouble). Pwr (op2 as PLRMatrix);
            if (op1 is PLDouble   && op2 is PLDouble)  return (op1 as PLDouble). Pwr (op2 as PLDouble);
            if (op1 is PLComplex  && op2 is PLDouble)  return (op1 as PLComplex).Pwr (op2 as PLDouble);

            //return new PLString ("Power of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
            throw new Exception ("Power of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        //****************************************************************************************
        //
        // ToString helper - called by derived classes
        //
        //  - e.g: %5.3f => 5, 3, f
        //
        protected List<string> SplitFormatString (string fmt)
        {
            fmt = fmt.Trim ();
            if (fmt [0] != '%') throw new Exception ("Syntax error in format: " + fmt);

            char letter = fmt [fmt.Length - 1];
            fmt = fmt.Substring (1, fmt.Length - 2);
            string [] numbers = fmt.Split (new char [] {'.'});

            List<string> parts = new List<string> ();
            foreach (string str in numbers)
                parts.Add (str);

            parts.Add (new string (new char [] {letter}));
            return parts;
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    //
    // Abstract base class for real and complex matrices
    //
    public abstract class PLMatrix : PLVariable
    {
        public override bool IsRowVector {get {return Rows == 1 && Cols > 1;}}
        public override bool IsColVector {get {return Cols == 1 && Rows > 1;}}
        public override bool IsVector    {get {return IsRowVector || IsColVector;}}
        public override bool IsMatrix    {get {return true;}}

        // C++ style setter & getter simplify code that handles both real and complex matrices
        public abstract PLVariable Get (int r, int c);
        public abstract void       Set (int r, int c, PLVariable plv);
        public abstract PLMatrix   CollapseToColumn ();

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            int a = 8, b = 5; // defaults
            string str = "";

            //PLVariable p1; 
            //PLComplex  p2;
            //PLDouble   p3;

            try
            {
                List<string> cfmtParts = base.SplitFormatString (cfmt);

                if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
                {
                    a = int.Parse (cfmtParts [0]);
                    b = int.Parse (cfmtParts [1]);
                }

                string fmt = "{0," + a + ":0.";
                for (int i = 0; i<b; i++) fmt += '#';
                fmt += "}";

                for (int r = 0; r<Rows; r++)
                {
                    for (int c = 0; c<Cols; c++)
                    {
                        //p1 = Get (r, c);
                        //p2 = p1 as PLComplex;
                        //p3 = p1 as PLDouble;

                        str += string.Format (fmt, Get (r, c)) + ", ";
                    }

                    if (r < Rows - 1)
                        str += "\n";
                }
            }

            catch (Exception ex)
            {
                EventLog.WriteLine ("Exception: " + ex.Message);
              //EventLog.WriteLine (ex.StackTrace);
            }

            return str;
        }

        public override string ToString ()
        {
            return ToString ("%8.3f");
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************
    //
    //  Real matrix
    //
    public class PLRMatrix : PLMatrix
    {
        public CommonMath.Matrix Data;

        public PLRMatrix (int r, int c)
        {
            Data = new Matrix (r, c);
        }

        public PLRMatrix (CommonMath.Matrix src)
        {
            Data = src;
        }

        public PLRMatrix (PLDouble src)
        {
            Data = new Matrix (1, 1);
            Data [0, 0] = src.Data;
        }

        public PLRMatrix (string name, CommonMath.Matrix data)
        {
            Name = name;
            Data = data;
        }

        public List<double> ReadByColumn ()
        {
            List<double> results = new List<double> (Rows * Cols);

            for (int j = 0; j<Cols; j++)
            {
                for (int i = 0; i<Rows; i++)
                    results.Add (Data [i, j]);
            }

            return results;
        }

        public override PLMatrix CollapseToColumn ()
        {
            PLRMatrix result = new PLRMatrix (Size, 1);

            List<double> data = ReadByColumn ();

            for (int i = 0; i<Size; i++)
            {
                result.Data [i, 0] = data [i];
            }

            return result;
        }

        public override int Rows {get {return Data.Rows;}}
        public override int Cols {get {return Data.Cols;}}
        public override int Size {get {return Rows * Cols;}}

        public double this [int row, int col]  // zero-based indices
        {
            get {return Data [row, col];}
            set {Data [row, col] = value;}
        }

        public double this [int sel]  // zero-based indices
        {
            get
            {
                int col = sel / Rows;
                int row = sel % Rows;
                return Data [row, col];
            }

            set
            {
                int col = sel / Rows;
                int row = sel % Rows;
                Data [row, col] = value;
            }
        }

        public override PLVariable Get (int row, int col)
        {
            return new PLDouble (Data [row, col]);
        }

        public override void Set (int row, int col, PLVariable dbl)
        {
            if (dbl is PLDouble)
                this [row, col] = (dbl as PLDouble).Data;

            else
                throw new Exception ("Real matrix can only store real numbers");
        }

        //*********************************************************************************
        //
        // Arithmetic on real matrices
        //

        public PLRMatrix Add (PLRMatrix op2) {return new PLRMatrix ("", Data + op2.Data);}
        public PLRMatrix Add (PLDouble op2)  {return new PLRMatrix ("", Data + op2.Data);}

        public PLRMatrix Sub (PLRMatrix op2) {return new PLRMatrix ("", Data - op2.Data);}
        public PLRMatrix Sub (PLDouble op2)  {return new PLRMatrix ("", Data - op2.Data);}
        public PLRMatrix Sub (PLInteger op2) {return new PLRMatrix ("", Data - op2.Data);}

        public PLRMatrix Mul (PLDouble op2)  {return new PLRMatrix ("", Data * op2.Data);}
        public PLRMatrix Mul (PLRMatrix op2) {return new PLRMatrix ("", Data * op2.Data);}

        public PLRMatrix Div (PLDouble op2)  {return new PLRMatrix ("", Data / op2.Data);}

        public PLRMatrix Pwr (PLDouble op2)  {return new PLRMatrix ("", Data ^ op2.Data);}
        public PLRMatrix Pwr (PLRMatrix op2) {return new PLRMatrix ("", Data ^ op2.Data);}

        public PLCMatrix Add (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j] + op2 [i, j].Real, op2 [i, j].Imag);

            return results;
        }

        public PLCMatrix Sub (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j] - op2 [i, j].Real, -op2 [i, j].Imag);

            return results;
        }

        public PLCMatrix Mul (PLCMatrix op2)
        {
            if (Cols != op2.Rows)
                throw new Exception ("Real-Complex matrix multiply size error");

            int M = Rows;
            int N = Cols;
            int P = op2.Cols;

            PLCMatrix results = new PLCMatrix (Rows, op2.Cols);

            for (int i = 0; i<M; i++)
            {
                for (int j = 0; j<P; j++)
                {
                    PLComplex acc = new PLComplex (0, 0);

                    for (int k = 0; k<N; k++)
                    {
                        acc = (acc + new PLComplex (this [i, k] * op2 [k, j].Real, this [i, k] * op2 [k, j].Imag)) as PLComplex;
                    }

                    results [i, j] = acc;
                }
            }

            return results;
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************
    //
    // Complex matrix.
    //
    public class PLCMatrix : PLMatrix
    {
        public CommonMath.CMatrix Data;

        public PLCMatrix (int r, int c)
        {
            Data = new CMatrix (r, c);
        }

        public PLCMatrix (CommonMath.CMatrix src)
        {
            Data = src;
        }

        public PLCMatrix (PLComplex src)
        {
            Data = new CMatrix (1, 1);
            Data [0, 0] = new CommonMath.Complex (src.Real, src.Imag);
        }

        public PLCMatrix (string name, PLCMatrix src)
        {
            Name = name;
            Data = src.Data;
        }

        public PLComplex this [int row, int col]  // zero-based indices
        {
            get {return new PLComplex (Data [row, col]);}
            set {Data [row, col] = new CommonMath.Complex (value.Real, value.Imag);}
        }

        public PLComplex this [int sel]  // zero-based indices
        {
            get
            {
                int col = sel / Rows;
                int row = sel % Rows;
                return this [row, col];
            }

            set
            {
                int col = sel / Rows;
                int row = sel % Rows;
                this [row, col] = value;
            }
        }

        public override PLVariable Get (int row, int col)
        {
            return new PLComplex (Data [row, col]);
        }

        public override void Set (int row, int col, PLVariable cmp)
        {
            if (cmp is PLComplex)
                this [row, col] = cmp as PLComplex;

            else if (cmp is PLDouble)
                this [row, col] = new PLComplex ((cmp as PLDouble).Data, 0);

            else
                throw new Exception ("Complex matrix can not store type " + cmp.GetType ());
        }

        public override int Rows {get {return Data.Rows;}}
        public override int Cols {get {return Data.Cols;}}
        public override int Size {get {return Rows * Cols;}}

        public override PLMatrix CollapseToColumn ()
        {
            PLCMatrix result = new PLCMatrix (Size, 1);
            List<PLComplex> data = ReadByColumn ();
            for (int i = 0; i<Size; i++)
            {
                result [i, 0] = data [i];
            }
            return result;
        }

        public List<PLComplex> ReadByColumn ()
        {
            List<PLComplex> results = new List<PLComplex> (Rows * Cols);
            for (int j = 0; j<Cols; j++)
            {
                for (int i = 0; i<Rows; i++)
                    results.Add (this [i, j]);
            }
            return results;
        }

        public PLCMatrix Add (PLCMatrix op2)
        {
            return new PLCMatrix (Data + op2.Data);
        }

        public PLCMatrix Add (PLComplex op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = this [i, j].Add (op2);

            return results;
        }

        public PLCMatrix Sub (PLRMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j].Real - op2 [i, j], this [i, j].Imag);

            return results;
        }

        public PLCMatrix Sub (PLCMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j].Real - op2 [i, j].Real, this [i, j].Imag - op2 [i, j].Imag);

            return results;
        }

        public PLCMatrix Mul (PLComplex op2)
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
            {
                for (int j = 0; j<Cols; j++)
                {
                    results [i, j] = this [i, j] * op2 as PLComplex;
                }
            }

            return results;
        }

        public PLCMatrix Mul (PLRMatrix op2)
        {
            if (Cols != op2.Rows)
                throw new Exception ("Complex-Real matrix multiply size error");

            PLCMatrix results = new PLCMatrix (Rows, op2.Cols);

            for (int i = 0; i<Rows; i++)
            {
                for (int j = 0; j<op2.Cols; j++)
                {
                    PLComplex acc = new PLComplex (0, 0);

                    for (int k = 0; k<Cols; k++)
                        acc = acc.Add (this [i, k].Mul (op2 [k, j]));

                    results [i, j] = acc;
                }
            }

            return results;
        }

        public PLCMatrix Mul (PLCMatrix op2)
        {
            return new PLCMatrix (Data * op2.Data);
        }

        public PLCMatrix Div (PLDouble op2)
        {
            return new PLCMatrix (Data / op2.Data);
        }

        public PLCMatrix Div (PLComplex op2)
        {
            return new PLCMatrix (Data / new CommonMath.Complex (op2.Data.Real, op2.Data.Imaginary));
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public abstract class PLScalar : PLVariable
    {
        abstract public int    DataAsInteger {get;}
        abstract public double DataAsDouble  {get;}
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    // use this instead of C# "null"

    public class PLNull : PLScalar
    {
        public object Data = null;

        public override int    DataAsInteger {get {return 0;}}
        public override double DataAsDouble  {get {return 0;}}

        public override string ToString (string fmt)
        {
            return "null";
        }

        public override string ToString ()
        {
            string str = base.ToString ();
            str += "null";
            str += "\n";
            return str;
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLDouble : PLScalar
    {
        readonly public double Data;

        public override int    DataAsInteger {get {return (int) Math.Round (Data);}}
        public override double DataAsDouble  {get {return Data;}}

        public PLDouble (double d)
        {
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
            return new PLComplex (Data + op2.Data.Real, op2.Data.Imaginary);
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

        public PLRMatrix  Sub (PLRMatrix  op2) {return new PLRMatrix  ("", Data - op2.Data); }
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
            int a = 8, b = 5; // defaults

            List<string> cfmtParts = SplitFormatString (cfmt);

            if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
            {
                a = int.Parse (cfmtParts [0]);
                b = int.Parse (cfmtParts [1]);
            }

            string fmt = "{0," + a + ":0.";
            for (int i = 0; i<b; i++) fmt += '#';
            fmt += "}";

            return string.Format (fmt, Data);
        }

        public override string ToString ()
        {
            if (Math.Abs (Data) > 1e4 || Math.Abs (Data) < 1e-4)
                return string.Format ("{0:E3}", Data);
            else
                return string.Format ("{0:0.#####}", Data);
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLComplex : PLScalar
    {
        public System.Numerics.Complex Data = new System.Numerics.Complex (0, 0);

        public double Real {get {return Data.Real;}      set {Data = new System.Numerics.Complex (value, Data.Imaginary);}}
        public double Imag {get {return Data.Imaginary;} set {Data = new System.Numerics.Complex (Data.Real, value);}}

        public PLComplex (double re, double im)
        {
            Real = re;
            Imag = im;
        }

        public override int    DataAsInteger {get {throw new Exception ("DataAsInteger not implemented for complex");}}
        public override double DataAsDouble  {get {throw new Exception ("DataAsDouble not implemented for complex");}}

        public PLComplex (System.Numerics.Complex src)
        {
            Data = new System.Numerics.Complex (src.Real, src.Imaginary);
        }

        public PLComplex (CommonMath.Complex src)
        {
            Data = new System.Numerics.Complex (src.Real, src.Imag);
        }

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
            get {return Data.Phase;}
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
            int a = 4, b = 5; // defaults

            List<string> cfmtParts = SplitFormatString (cfmt);

            if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
            {
                a = int.Parse (cfmtParts [0]);
                b = int.Parse (cfmtParts [1]);
            }

            string fmt1 = "{0," + a + ":0.";
            for (int i = 0; i<b; i++) fmt1 += '#';
            fmt1 += "}";

            string fmt2 = "{1," + a + ":0.";
            for (int i = 0; i<b; i++) fmt2 += '#';
            fmt2 += "}";

            string fmt = "(" + fmt1 + ", " + fmt2 + ")";

            return string.Format (fmt, Real, Imag);
        }

        public override string ToString ()
        {
            return "(" + Real.ToString () + ", " + Imag.ToString () + ")";
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLInteger : PLScalar
    {
        readonly public int Data;

        public PLInteger (int d)
        {
            Data = d;
        }

        public override int    DataAsInteger {get {return Data;}}
        public override double DataAsDouble  {get {return Data;}}

        public PLInteger Add (PLInteger op2) {return new PLInteger (Data + op2.Data);}
        public PLDouble  Add (PLDouble  op2) {return new PLDouble  (Data + op2.Data);}
        public PLInteger Sub (PLInteger op2) {return new PLInteger (Data - op2.Data);}
        public PLDouble  Sub (PLDouble  op2) {return new PLDouble  (Data - op2.Data);}
        public PLInteger Mul (PLInteger op2) {return new PLInteger (Data * op2.Data);}
        public PLDouble  Mul (PLDouble  op2) {return new PLDouble  (Data * op2.Data);}
        public PLInteger Div (PLInteger op2) {return new PLInteger (Data / op2.Data);}

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {

            int a = 8, b = 5; // defaults

            List<string> cfmtParts = SplitFormatString (cfmt);

            if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
            {
                a = int.Parse (cfmtParts [0]);
                b = int.Parse (cfmtParts [1]);
            }

            string fmt = "{0," + a + ":0.";
            for (int i = 0; i<b; i++) fmt += '#';
            fmt += "}";

            return string.Format (fmt, Data);
        }

        public override string ToString ()
        {
            return ToString ("%8d");
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLFunctionWrapper : PLScalar
    {
        public PLFunction Data;

        public PLFunctionWrapper (PLFunction func)
        {
            Data = func;
        }

        public override int    DataAsInteger {get {throw new Exception ("DataAsInteger not implemented for function wrapper");}}
        public override double DataAsDouble  {get {throw new Exception ("DataAsDouble not implemented for function wrapper");}}

        public override string ToString (string _)
        {
            return "function";
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLBool : PLScalar
    {
        public bool Data;

        public PLBool (bool d)
        {
            Data = d;
        }

        public override int    DataAsInteger {get {return Data == true ? 1 : 0;}}
        public override double DataAsDouble  {get {return Data == true ? 1 : 0;}}

        public PLBool (PLVariable d)
        {
            if (d is PLInteger)
                Data = (d as PLInteger).Data != 0;

            else if (d is PLBool)
                Data = (d as PLBool).Data;

            else if (d is PLDouble)
                Data = (d as PLDouble).Data != 0;

            else
                throw new Exception ("PLBool - can't make from " + d.GetType ());
        }

        public override string ToString (string _)
        {
            return Data.ToString ();
        }

        public override string ToString ()
        {
            return ToString ("%8b"); // b?
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLString : PLVariable
    {
        public string Data;

        public PLString (string str)
        {
            Data = str;
        }

        public string Text // returns Data string without the surrounding single quotes
        {
            get
            {
                string text = Data;

                if (text.Length > 0)
                    if (text [0] == '\'')
                        text = text.Remove (0, 1);

                if (text.Length > 0)
                    if (text [text.Length - 1] == '\'')
                        text = text.Remove (text.Length - 1, 1);

                return text;
            }
        }

        //public override int Rows {get {return 1;}}
        //public override int Cols {get {return Data.Length;}}
        //public override int Size {get {return Data.Length;}}


        public PLString Add (string op2)   {Data += op2; return this;}
        public PLString Add (PLString op2) {Data += op2.Data; return this;}

        public override string ToString (string _)
        {
            return Data;
        }

        public override string ToString ()
        {
            return Data;
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLList : PLVariable, IEnumerable
    {
        public List<PLVariable> Data;

        public PLList ()
        {
            Data = new List<PLVariable> ();
        }

        public void Add (PLVariable var)
        {
            Data.Add (var);
        }

        public void Insert (int index, PLVariable var)
        {
            Data.Insert (index, var);
        }

        public PLVariable this [int select]
        {
            get {return Data [select];}
            set {Data [select] = value;}
        }

        //public override int Rows { get { throw new Exception ("Trying to read number rows in a list"); } }
        //public override int Cols { get { throw new Exception ("Trying to read number cols in a list"); } }
        public override int Size { get { int size = 0; foreach (PLVariable pl in Data) size += pl.Size; return size; } }

        public int Count {get {return Data.Count;}}

        public override string ToString (string cfmt)
        {
            string str = "[";

            foreach (PLVariable var in Data)
            {
                str += var.ToString (cfmt) +"\n";
            }

            str += ']';

            return str;
        }

        public override string ToString ()
        {
            string str = ""; // "[";

            foreach (PLVariable var in Data)
            {
                str += var.ToString () + " \n";
              //str += "(" + var.ToString () + ")" + "\n";
            }

            //str += ']';
            return str;
        }

        //
        // Enumeration
        //

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator ();
        }

        internal PLVarEnum GetEnumerator ()
        {
            return new PLVarEnum (Data);
        }
    }

    //**************************************************************************

    //
    // PLVarEnum - used by PLList iterator
    //

    internal class PLVarEnum : IEnumerator
    {
        public List<PLVariable> _PLVariables;

        int position = -1;

        public PLVarEnum (List<PLVariable> lst)
        {
            _PLVariables = lst;
        }

        public bool MoveNext ()
        {
            position++;
            return (position < _PLVariables.Count);
        }

        public void Reset ()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public PLVariable Current
        {
            get {return _PLVariables [position];}
        }
    }
}

