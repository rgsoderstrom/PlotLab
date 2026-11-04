
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;

using Common;
using CommonMath;

namespace PLCommon
{
    public abstract class PLVariable
    {
        public string Name = "";
        public abstract string ToString (string fmt);

        public enum PrintFormatType {Short, Long};

        public static PrintFormatType PrintFormat = PrintFormatType.Short;

        public static double ToNumber (PLVariable plv, ref bool failFlag)
        {
            if (plv is PLDouble) return (plv as PLDouble).Data;
            if (plv is PLInteger) return (plv as PLInteger).Data;
            if (plv is PLMatrix) { PLRMatrix mat = plv as PLRMatrix; if (mat.Rows == 1 && mat.Cols == 1) return mat [0, 0]; }
            failFlag = true;
            return 0;
        }


        //*************************************************************************************
        //*************************************************************************************
        //*************************************************************************************

        internal static string DoubleToString (double dbl)
        {
            if (Math.Abs (dbl) < 100 * Double.Epsilon)
                return ("0");

            int digits = (int) Math.Ceiling (Math.Log10 (Math.Abs (dbl)));

            string formatString;

            if (PrintFormat == PrintFormatType.Short)
            {
                if (digits > 3 || digits < -1) formatString = "{0:E2}";
                else formatString = "{0:0.######}";
            }

            else if (PrintFormat == PrintFormatType.Long)
            {
                if (digits > 5 || digits < -2) formatString = "{0:E6}";
                else formatString = "{0:0.###############}";
            }

            else
                throw new Exception ("Unsupported print format");

            return string.Format (formatString, dbl);
        }

        //*************************************************************************************
        //*************************************************************************************
        //*************************************************************************************

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
            if (op1 is PLRMatrix && op2 is PLComplex)  return (op1 as PLRMatrix).Add (op2 as PLComplex);

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
            if (op1 is PLCMatrix && op2 is PLComplex) return (op1 as PLCMatrix).Sub (op2 as PLComplex);

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

        /*
        public static PLComplex operator/ (double op1, PLVariable op2)
        {  
            if (op2 is PLComplex)
            { 
                PLComplex c2 = op2 as PLComplex;
                double mag = op1 / c2.Magnitude;
                double ang = -1 * c2.Angle;
                PLComplex r = new PLComplex (mag * Math.Cos (ang), mag * Math.Sin (ang));
                return r;
            }

            throw new Exception ("Division of real and " + op2.GetType () + " not supported");
        }
        */







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

            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger).Div (op2 as PLDouble);

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

    //****************************************************************************************************
    //****************************************************************************************************
    //****************************************************************************************************


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
        public PLDouble  Div (PLDouble  op2) {return new PLDouble  (Data / op2.Data);}

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

        public PLBool (string n, bool d)
        {
            Name = n;
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


        public PLString Add (string op2)
        {
            Data += op2;
            return this;
        }

        public PLString Add (PLString op2) 
        {
            string s1 = Data;
            s1 = s1.Remove (s1.Length-1, 1);
            string s2 = op2.Data;
            s2 = s2.Remove (0, 1);
            Data = s1 + s2;
            return this;
        }

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
            string str = "{"; // "[";

            for (int i=0; i<Data.Count; i++)
            {
                if (i < Data.Count - 1)
                    str += Data [i].ToString () +", ";
                else
                    str += Data [i].ToString ();
            }

            str += '}';
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

