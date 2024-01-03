
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonMath;

namespace PLCommon
{
    public abstract class PLVariable
    {
        public string Name = "";
        public virtual int Size {get {return 1;}} // in elements, not bytes
        public virtual int Rows {get {return 1;}} // in elements, not bytes
        public virtual int Cols {get {return 1;}} // in elements, not bytes

        public virtual bool IsMatrix    {get {return Rows > 1 && Cols > 1;}}
        public virtual bool IsRowVector {get {return Rows == 1;}}
        public virtual bool IsColVector {get {return Cols == 1;}}
        public virtual bool IsVector    {get {return IsRowVector || IsColVector;}}

        public abstract string ToString (string fmt);

        public static double ToNumber (PLVariable plv, ref bool failFlag)
        {
            if (plv is PLDouble) return (plv as PLDouble).Data;
            if (plv is PLInteger) return (plv as PLInteger).Data;

            if (plv is PLMatrix)
            {
                PLMatrix mat = plv as PLMatrix;
                if (mat.Rows == 1 && mat.Cols == 1)
                    return mat [0, 0];
            }

            failFlag = true;
            return 0;
        }

        public static PLVariable operator + (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLMatrix  && op2 is PLCMatrix) return (op1 as PLMatrix).Add  (op2 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLCMatrix) return (op1 as PLCMatrix).Add (op2 as PLCMatrix);
            if (op1 is PLDouble  && op2 is PLCMatrix) return (op1 as PLDouble).Add  (op2 as PLCMatrix);
            if (op1 is PLCMatrix && op2 is PLDouble)  return (op2 as PLDouble).Add  (op1 as PLCMatrix);
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Add (op2 as PLComplex);
            if (op1 is PLComplex && op2 is PLDouble)  return (op1 as PLComplex).Add (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble).Add  (op2 as PLComplex);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble).Add  (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger).Add (op2 as PLInteger);
            if (op1 is PLMatrix  && op2 is PLMatrix)  return (op1 as PLMatrix).Add  (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble)  return (op1 as PLMatrix).Add  (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLMatrix)  return (op2 as PLMatrix).Add  (op1 as PLDouble);
            if (op1 is PLString  && op2 is PLString)  return (op1 as PLString).Add  (op2 as PLString);

            throw new Exception ("Addition of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator- (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Sub (op2 as PLComplex);
            if (op1 is PLComplex && op2 is PLDouble)  return (op1 as PLComplex).Sub (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble).Sub  (op2 as PLComplex);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble).Sub  (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger).Sub (op2 as PLInteger);
            if (op1 is PLMatrix  && op2 is PLMatrix)  return (op1 as PLMatrix).Sub  (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble)  return (op1 as PLMatrix).Sub  (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLMatrix)  return (op1 as PLDouble).Sub  (op2 as PLMatrix);
            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger).Sub (op2 as PLDouble);

            throw new Exception ("Subtraction of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator* (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLCMatrix && op2 is PLCMatrix) return (op1 as PLCMatrix).Mul (op2 as PLCMatrix);
            if (op1 is PLComplex && op2 is PLMatrix)  return (op1 as PLComplex).Mul (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLComplex) return (op2 as PLComplex).Mul (op1 as PLMatrix);
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Mul (op2 as PLComplex);
            if (op1 is PLComplex && op2 is PLDouble)  return (op1 as PLComplex).Mul (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLComplex) return (op1 as PLDouble).Mul  (op2 as PLComplex);
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble).Mul  (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLCMatrix) return (op1 as PLDouble).Mul  (op2 as PLCMatrix);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger).Mul (op2 as PLInteger);
            if (op1 is PLMatrix  && op2 is PLMatrix)  return (op1 as PLMatrix).Mul  (op2 as PLMatrix);
            if (op1 is PLDouble  && op2 is PLMatrix)  return (op1 as PLDouble).Mul  (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble)  return (op2 as PLDouble).Mul  (op1 as PLMatrix);
            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger).Mul (op2 as PLDouble);
          //if (op1 is PLDouble  && op2 is PLInteger) return (op1 as PLDouble).Mul  (op2 as PLInteger);

            throw new Exception ("Multiplication of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator/ (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLComplex && op2 is PLComplex) return (op1 as PLComplex).Div (op2 as PLComplex);
            if (op1 is PLComplex && op2 is PLDouble)  return (op1 as PLComplex).Div (op2 as PLDouble);
            if (op1 is PLDouble && op2 is PLDouble)   return (op1 as PLDouble).Div  (op2 as PLDouble);
            if (op1 is PLMatrix && op2 is PLDouble)   return (op1 as PLMatrix).Div  (op2 as PLDouble);
            if (op1 is PLDouble && op2 is PLMatrix)   return (op1 as PLDouble).Div  (op2 as PLMatrix);

            throw new Exception ("Division of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator^ (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLDouble  && op2 is PLDouble) return (op1 as PLDouble).Pwr (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLMatrix) return (op1 as PLDouble).Pwr (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble) return (op1 as PLMatrix).Pwr (op2 as PLDouble);
            if (op1 is PLMatrix  && op2 is PLMatrix) return (op1 as PLMatrix).Pwr (op2 as PLMatrix);  // implments .^, element-by-element

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

    public class PLMatrix : PLVariable
    {
        public CommonMath.Matrix Data;

        public PLMatrix (int r, int c)
        {
            Data = new Matrix (r, c);
        }

        public PLMatrix (CommonMath.Matrix src)
        {
            Data = src;
        }

        public PLMatrix (PLDouble src)
        {
            Data = new Matrix (1, 1);
            Data [0, 0] = src.Data;
        }

        public PLMatrix (string name, CommonMath.Matrix data)
        {
            Name = name;
            Data = data;
        }

        public List<double> ReadByColumn ()
        {
            List<double> results = new List<double> (Rows * Cols);

            for (int j=0; j<Cols; j++)
            {
                for (int i = 0; i<Rows; i++)
                    results.Add (Data [i, j]);
            }

            return results;
        }

        public PLMatrix CollapseToColumn ()
        {
            PLMatrix result = new PLMatrix (Size, 1);

            List<double> data = ReadByColumn ();

            for (int i=0; i<Size; i++)
            {
                result.Data [i, 0] = data [i];
            }

            return result;
        }

        public override int Rows { get { return Data.Rows; } }
        public override int Cols { get { return Data.Cols; } }
        public override int Size { get { return Rows * Cols; } }

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

        public PLMatrix Add (PLMatrix op2) {return new PLMatrix ("", Data + op2.Data);}
        public PLMatrix Add (PLDouble op2) {return new PLMatrix ("", Data + op2.Data);}
        public PLMatrix Sub (PLMatrix op2) {return new PLMatrix ("", Data - op2.Data);}
        public PLMatrix Sub (PLDouble op2) {return new PLMatrix ("", Data - op2.Data);}
        public PLMatrix Mul (PLDouble op2) {return new PLMatrix ("", Data * op2.Data);}
        public PLMatrix Mul (PLMatrix op2) {return new PLMatrix ("", Data * op2.Data);}
        public PLMatrix Div (PLDouble op2) {return new PLMatrix ("", Data / op2.Data);}
        public PLMatrix Pwr (PLDouble op2) {return new PLMatrix ("", Data ^ op2.Data);}
        public PLMatrix Pwr (PLMatrix op2) {return new PLMatrix ("", Data ^ op2.Data);}

        public PLCMatrix Add (PLCMatrix op2) 
        {
            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = new PLComplex (this [i, j] + op2 [i, j].Real, this [i, j] + op2 [i, j].Imag);

            return results;
        }

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            int a = 8, b = 5; // defaults

            List<string> cfmtParts = base.SplitFormatString (cfmt);

            if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
            {
                a = int.Parse (cfmtParts [0]);
                b = int.Parse (cfmtParts [1]);
            }

            string str = "";

            string fmt = "{0," + a + ":0.";
            for (int i = 0; i<b; i++) fmt += '#';
            fmt += "}";

            for (int r = 0; r<Rows; r++)
            {
                for (int c = 0; c<Cols; c++)
                    str += string.Format (fmt, Data [r, c]) + ", ";

                if (r < Rows - 1)
                    str += "\n";
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

    // PLCMatrix - complex matrix.

    public class PLCMatrix : PLVariable
    {
        public CommonMath.Matrix Real;
        public CommonMath.Matrix Imag;

        public PLCMatrix (int r, int c)
        {
            Real = new Matrix (r, c);
            Imag = new Matrix (r, c);
        }

        //public PLCMatrix (CommonMath.Matrix src)
        //{
        //    Data = src;
        //}

        public PLCMatrix (PLComplex src)
        {
            Real = new Matrix (1, 1);
            Imag = new Matrix (1, 1);
            Real [0, 0] = src.Real;
            Imag [0, 0] = src.Imag;
        }

        //public PLCMatrix (string name, CommonMath.Matrix data)
        //{
        //    Name = name;
        //    Data = data;
        //}

        public List<PLComplex> ReadByColumn ()
        {
            List<PLComplex> results = new List<PLComplex> (Rows * Cols);

            for (int j=0; j<Cols; j++)
            {
                for (int i = 0; i<Rows; i++)
                    results.Add (this [i, j]);
            }

            return results;
        }

        public PLCMatrix CollapseToColumn ()
        {
            PLCMatrix result = new PLCMatrix (Size, 1);

            List<PLComplex> data = ReadByColumn ();

            for (int i=0; i<Size; i++)
            {
                result [i, 0] = data [i];
            }

            return result;
        }

        public override int Rows { get { return Real.Rows; } }
        public override int Cols { get { return Real.Cols; } }
        public override int Size { get { return Rows * Cols; } }

        public PLComplex this [int row, int col]  // zero-based indices
        {
            get {return new PLComplex (Real [row, col], Imag [row, col]);}
            set {Real [row, col] = value.Real; Imag [row, col] = value.Imag; }
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

        public PLCMatrix Add (PLCMatrix op2)
        {
            if (Rows != op2.Rows || Cols != op2.Cols)
                throw new Exception ("Complex matrix addition size error");

            PLCMatrix results = new PLCMatrix (Rows, Cols);

            for (int i = 0; i<Rows; i++)
                for (int j = 0; j<Cols; j++)
                    results [i, j] = this [i, j].Add (op2 [i, j]);

            return results;
        }


        public PLCMatrix Mul (PLCMatrix op2)
        {
            if (Cols != op2.Rows)
                throw new Exception ("Complex matrix multiply size error");

            PLCMatrix results = new PLCMatrix (Rows, op2.Cols);

            for (int i = 0; i<Rows; i++)
            {
                for (int j = 0; j<op2.Cols; j++)
                {
                    PLComplex acc = new PLComplex (0, 0);

                    for (int k = 0; k<Cols; k++)
                        acc.Add (this [i, k].Mul (op2 [k, j]));

                    results [i, j] = acc;
                }
            }

            return results;
        }

        public static PLCMatrix operator+ (PLDouble op1, PLCMatrix op2) 
        {
            PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);
            
            for (int i=0; i<op2.Rows; i++)
            {
                for (int j=0; j<op2.Cols; j++)
                {
                    results [i, j] = new PLComplex (op1.Data + op2 [i, j].Real, op2 [i, j].Imag);
                }
            }

            return results;
        }


        //public static PLMatrix operator/ (PLMatrix op1, PLDouble op2) {return new PLMatrix ("", op1.Data / op2.Data);}
        //public static PLMatrix operator/ (PLDouble op1, PLMatrix op2) {return new PLMatrix ("", op1.Data / op2.Data);}
        //public static PLMatrix operator^ (PLMatrix op1, PLDouble op2) {return new PLMatrix ("", op1.Data ^ op2.Data);}
        //public static PLMatrix operator^ (PLMatrix op1, PLMatrix op2) {return new PLMatrix ("", op1.Data ^ op2.Data);}
        //public static PLMatrix operator^ (PLDouble op1, PLMatrix op2) {return new PLMatrix ("", op1.Data ^ op2.Data);}

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            int a = 8, b = 5; // defaults

            List<string> cfmtParts = base.SplitFormatString (cfmt);

            if (cfmtParts.Count == 3)  // --------------------- use defaults if error in format
            {
                a = int.Parse (cfmtParts [0]);
                b = int.Parse (cfmtParts [1]);
            }

            string str = "";

            //string fmt = "{0," + a + ":0.";
            //for (int i = 0; i<b; i++) fmt += '#';
            //fmt += "}";

            string fmt = "({0:0.###}, {1:0.###})";

            for (int r = 0; r<Rows; r++)
            {
                for (int c = 0; c<Cols; c++)
                {
                    double re = this [r, c].Real;
                    double im = this [r, c].Imag;
                    str += string.Format (fmt, re, im) + ", ";
                }

                if (r < Rows - 1)
                    str += "\n";
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

    public abstract class PLScalar : PLVariable
    {
        public override int Size {get {return 1;}}
        public override int Rows {get {return 1;}}
        public override int Cols {get {return 1;}}
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    // use this instead of C# "null"

    public class PLNull : PLScalar
    {
        public object Data = null;

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

        public PLDouble (double d)
        {
            Data = d;
        }

        public PLDouble (PLVariable src)
        {
            PLDouble pld = src as PLDouble;
            PLMatrix mat = src as PLMatrix;
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
                    throw new Exception ("Error constructing PLDouble from PLMatrix");
            }

            else if (igr != null)
            {
                Data = igr.Data;
            }

            else
                throw new Exception ("Unsupported PLDouble ctor: " + src.GetType ().ToString ());
        }

        public PLDouble  Add (PLDouble op2)  {return new PLDouble  (Data + op2.Data);}
        public PLComplex Add (PLComplex op2) {return new PLComplex (Data + op2.Real, op2.Imag);}
        public PLMatrix  Add (PLMatrix op2)  {return new PLMatrix  ("", Data + op2.Data); }
        public PLCMatrix  Add (PLCMatrix op2)
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

        public PLCMatrix  Mul (PLCMatrix op2)
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

        public PLDouble  Sub (PLDouble op2)   {return new PLDouble  (Data - op2.Data);}
        public PLDouble  Sub (PLInteger op2)  {return new PLDouble  (Data - op2.Data);}
        public PLMatrix  Sub (PLMatrix op2)   {return new PLMatrix  ("", Data - op2.Data); }
        public PLComplex Sub (PLComplex op2)  {return new PLComplex (Data - op2.Real, -op2.Imag);}
        public PLDouble  Mul (PLDouble op2)   {return new PLDouble  (Data * op2.Data);}
        public PLMatrix  Mul (PLMatrix op2)   {return new PLMatrix  ("", Data * op2.Data); }
        public PLComplex Mul (PLComplex op2)  {return new PLComplex (Data * op2.Real, Data * op2.Imag);}
        public PLDouble  Div (PLDouble op2)   {return new PLDouble  (Data / op2.Data);}
        public PLMatrix  Div (PLMatrix op2)   {return new PLMatrix  ("", Data / op2.Data); }
        public PLDouble  Pwr (PLDouble op2)   {return new PLDouble  (Math.Pow (Data, op2.Data));}
        public PLMatrix  Pwr (PLMatrix op2)   {return new PLMatrix  ("", Data ^ op2.Data); }

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
                return (string.Format ("{0:E5}", Data));
            else
                return (string.Format ("{0:0.#####}", Data));
        }
    }

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************

    public class PLComplex : PLScalar
    {
        readonly public double Real, Imag;

        public PLComplex (double re, double im)
        {
            Real = re;
            Imag = im;
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
            get {return Math.Sqrt (Real * Real + Imag * Imag);}
        }

        public double Angle // in radians
        {
            get {return Math.Atan2 (Imag, Real);}
        }

        public PLCMatrix Mul (PLMatrix op2)
        {
            PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);

            for (int i = 0; i<op2.Rows; i++)
                for (int j = 0; j<op2.Cols; j++)
                    results [i, j] = new PLComplex (Real * op2 [i, j], Imag * op2 [i,j]);

            return results;
        }

        //public PLCMatrix Mul (PLCMatrix op2) 
        //{
        //    PLCMatrix results = new PLCMatrix (op2.Rows, op2.Cols);
            
        //    for (int i=0; i<op2.Rows; i++)
        //    {
        //        for (int j=0; j<op2.Cols; j++)
        //        {
        //            results [i, j] = Data * op2 [i, j];
        //        }
        //    }

        //    return results;
        //}


     // public static PLComplex operator * (double    op1, PLComplex op2) {return new PLComplex (op1 * op2.Real, op1 * op2.Imag);}


        public PLComplex Add (PLComplex op2) {return new PLComplex (Real + op2.Real, Imag + op2.Imag);}
        public PLComplex Add (PLDouble  op2) {return new PLComplex (Real + op2.Data, Imag);}
        public PLComplex Sub (PLComplex op2) {return new PLComplex (Real - op2.Real, Imag - op2.Imag);}
        public PLComplex Sub (PLDouble  op2) {return new PLComplex (Real - op2.Data, Imag);}
        public PLComplex Mul (PLComplex op2) {return new PLComplex (Real * op2.Real - Imag * op2.Imag, Real * op2.Imag + Imag * op2.Real);}
        public PLComplex Mul (PLDouble op2)  {return new PLComplex (Real * op2.Data, Imag * op2.Data);}
        public PLComplex Div (PLDouble  op2) {return new PLComplex (Real / op2.Data, Imag / op2.Data);}

        public PLComplex Div (PLComplex op2) {double mag = Magnitude * op2.Magnitude; double ang = Angle - op2.Angle; 
            return new PLComplex (mag * Math.Cos (ang), mag * Math.Sin (ang));}

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

            return string.Format (fmt, 123); // Data);
        }

        public override string ToString ()
        {
            double mag2 = Real * Real + Imag * Imag;
            if (Math.Abs (mag2) > 1e8 || Math.Abs (mag2) < 1e-8)
                return (string.Format ("{(0:E5}, {1:E5)}", Real, Imag));
            else
                return (string.Format ("({0:0.####}, {1:0.####})", Real, Imag));
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

        public PLInteger Add (PLInteger op2) {return new PLInteger (Data + op2.Data);}
        public PLInteger Sub (PLInteger op2) {return new PLInteger (Data - op2.Data);}
        public PLDouble  Sub (PLDouble  op2) {return new PLDouble  (Data - op2.Data);}
        public PLInteger Mul (PLInteger op2) {return new PLInteger (Data * op2.Data);}
        public PLDouble  Mul (PLDouble  op2) {return new PLDouble  (Data * op2.Data);}
        public PLInteger Div (PLInteger op2) {return new PLInteger (Data / op2.Data);}

     // public static PLDouble  operator * (PLDouble  op1, PLInteger op2) {return new PLDouble  (op1.Data * op2.Data);}

        //*************************************************************************************

        // %A.Bf - A is total width, B is number of chars after decimal

        public override string ToString (string cfmt)
        {
            int w = 8;

            List<string> cfmtParts = base.SplitFormatString (cfmt);

            if (cfmtParts.Count == 2)  // --------------------- use defaults if error in format
                w = int.Parse (cfmtParts [0]);

            string str = Data.ToString ();

            while (str.Length < w)
                str = " " + str;

            return str;
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

        public override int Rows {get {return 1;}}
        public override int Cols {get {return Data.Length;}}
        public override int Size {get {return Data.Length;}}


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

        public override int Rows {get {throw new Exception ("Trying to read number rows in a list");}}
        public override int Cols {get {throw new Exception ("Trying to read number cols in a list");}}
        public override int Size {get {int size = 0; foreach (PLVariable pl in Data) size += pl.Size; return size;}}

        public int Count {get {return Data.Count;}}

        public override string ToString (string cfmt)
        {
            string str = "";

            foreach (PLVariable var in Data)
            {
                str += var.ToString (cfmt) +"\n";
            }

            return str;
        }

        public override string ToString ()
        {
            string str = "";

            foreach (PLVariable var in Data)
            {
                str += var.ToString () + " \n";
              //str += "(" + var.ToString () + ")" + "\n";
            }

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

