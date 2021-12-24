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
        public abstract int Size {get;} // in elements, not bytes
        public abstract int Rows {get;} // in elements, not bytes
        public abstract int Cols {get;} // in elements, not bytes

        public abstract string ToString (string fmt);

        public static PLVariable operator+ (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble)  + (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger) + (op2 as PLInteger);
            if (op1 is PLMatrix  && op2 is PLMatrix)  return (op1 as PLMatrix)  + (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble)  return (op1 as PLMatrix)  + (op2 as PLDouble);

            if (op1 is PLDouble  && op2 is PLMatrix)  return (op2 as PLMatrix)  + (op1 as PLDouble);

            if (op1 is PLString  && op2 is PLString)  return (op1 as PLString)  + (op2 as PLString);
            throw new Exception ("Addition of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator- (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble)  - (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger) - (op2 as PLInteger);
            if (op1 is PLMatrix  && op2 is PLMatrix)  return (op1 as PLMatrix)  - (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble)  return (op1 as PLMatrix)  - (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLMatrix)  return (op1 as PLDouble)  - (op2 as PLMatrix);

            if (op1 is PLInteger && op2 is PLDouble)  return new PLDouble ((op1 as PLInteger).Data - (op2 as PLDouble).Data);
            if (op1 is PLDouble  && op2 is PLInteger) return new PLDouble ((op1 as PLDouble).Data  - (op2 as PLInteger).Data);

            throw new Exception ("Subtraction of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator* (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLDouble  && op2 is PLDouble)  return (op1 as PLDouble)  * (op2 as PLDouble);
            if (op1 is PLInteger && op2 is PLInteger) return (op1 as PLInteger) * (op2 as PLInteger);
            if (op1 is PLMatrix  && op2 is PLMatrix)  return (op1 as PLMatrix)  * (op2 as PLMatrix);
            if (op1 is PLDouble  && op2 is PLMatrix)  return (op1 as PLDouble)  * (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble)  return (op2 as PLDouble)  * (op1 as PLMatrix);

            if (op1 is PLInteger && op2 is PLDouble)  return (op1 as PLInteger) * (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLInteger) return (op1 as PLDouble)  * (op2 as PLInteger);

            throw new Exception ("Multiplication of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator/ (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLDouble && op2 is PLDouble) return (op1 as PLDouble) / (op2 as PLDouble);
            if (op1 is PLMatrix && op2 is PLDouble) return (op1 as PLMatrix) / (op2 as PLDouble);

            if (op1 is PLDouble && op2 is PLMatrix) 
                return (op1 as PLDouble) / (op2 as PLMatrix);

            throw new Exception ("Division of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
        }

        public static PLVariable operator^ (PLVariable op1, PLVariable op2)
        {
            if (op1 is PLDouble  && op2 is PLDouble) return (op1 as PLDouble) ^ (op2 as PLDouble);
            if (op1 is PLDouble  && op2 is PLMatrix) return (op1 as PLDouble) ^ (op2 as PLMatrix);
            if (op1 is PLMatrix  && op2 is PLDouble) return (op1 as PLMatrix) ^ (op2 as PLDouble);
            if (op1 is PLMatrix  && op2 is PLMatrix) return (op1 as PLMatrix) ^ (op2 as PLMatrix);  // implments .^, element-by-element
            throw new Exception ("Exponentiation of " + op1.GetType () + " and " + op2.GetType () + " not implemented");
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

        public bool IsRowVector {get {return Rows == 1;}}
        public bool IsColVector {get {return Cols == 1;}}
        public bool IsVector    {get {return IsRowVector || IsColVector;}}
        public bool IsMatrix    {get {return Rows != 1 && Cols != 1;}}

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

        public static PLMatrix operator+ (PLMatrix op1, PLMatrix op2) {return new PLMatrix ("", op1.Data + op2.Data);}
        public static PLMatrix operator+ (PLMatrix op1, PLDouble op2) {return new PLMatrix ("", op1.Data + op2.Data);}
        public static PLMatrix operator- (PLMatrix op1, PLMatrix op2) {return new PLMatrix ("", op1.Data - op2.Data);}
        public static PLMatrix operator- (PLMatrix op1, PLDouble op2) {return new PLMatrix ("", op1.Data - op2.Data);}
        public static PLMatrix operator- (PLDouble op1, PLMatrix op2) {return new PLMatrix ("", op1.Data - op2.Data);}
        public static PLMatrix operator* (PLMatrix op1, PLMatrix op2) {return new PLMatrix ("", op1.Data * op2.Data);}
        public static PLMatrix operator* (PLDouble op1, PLMatrix op2) {return new PLMatrix ("", op1.Data * op2.Data);}
        public static PLMatrix operator/ (PLMatrix op1, PLDouble op2) {return new PLMatrix ("", op1.Data / op2.Data);}
        public static PLMatrix operator/ (PLDouble op1, PLMatrix op2) {return new PLMatrix ("", op1.Data / op2.Data);}
        public static PLMatrix operator^ (PLMatrix op1, PLDouble op2) {return new PLMatrix ("", op1.Data ^ op2.Data);}
        public static PLMatrix operator^ (PLMatrix op1, PLMatrix op2) {return new PLMatrix ("", op1.Data ^ op2.Data);}
        public static PLMatrix operator^ (PLDouble op1, PLMatrix op2) {return new PLMatrix ("", op1.Data ^ op2.Data);}

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

        public static PLDouble operator + (PLDouble op1, PLDouble op2) {return new PLDouble (op1.Data + op2.Data);}
        public static PLDouble operator - (PLDouble op1, PLDouble op2) {return new PLDouble (op1.Data - op2.Data);}
        public static PLDouble operator * (PLDouble op1, PLDouble op2) {return new PLDouble (op1.Data * op2.Data);}
        public static PLDouble operator / (PLDouble op1, PLDouble op2) {return new PLDouble (op1.Data / op2.Data);}
        public static PLDouble operator ^ (PLDouble op1, PLDouble op2) {return new PLDouble (Math.Pow (op1.Data, op2.Data));}


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
                return (string.Format ("{0:#.#####}", Data));
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

        public static PLInteger operator + (PLInteger op1, PLInteger op2) {return new PLInteger (op1.Data + op2.Data);}
        public static PLInteger operator - (PLInteger op1, PLInteger op2) {return new PLInteger (op1.Data - op2.Data);}
        public static PLInteger operator * (PLInteger op1, PLInteger op2) {return new PLInteger (op1.Data * op2.Data);}
        public static PLDouble  operator * (PLDouble  op1, PLInteger op2) {return new PLDouble  (op1.Data * op2.Data);}
        public static PLDouble  operator * (PLInteger op1, PLDouble  op2) {return new PLDouble  (op1.Data * op2.Data);}
        public static PLInteger operator / (PLInteger op1, PLInteger op2) {return new PLInteger (op1.Data / op2.Data);}

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

                if (text [0] == '\'')
                    text = text.Remove (0, 1);

                if (text [text.Length - 1] == '\'')
                    text = text.Remove (text.Length - 1, 1);

                return text;
            }
        }

        public override int Rows {get {return 1;}}
        public override int Cols {get {return Data.Length;}}
        public override int Size {get {return Data.Length;}}

        public static PLString operator+ (PLString op1, string   op2) {return new PLString (op1.Data + op2);}
        public static PLString operator+ (PLString op1, PLString op2) {return new PLString (op1.Data + op2.Data);}

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
                str += "(" + var.ToString () + ")" + "\n";
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



