using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
//using PLWorkspace;
//using PLLibrary;

namespace Main
{
    internal partial class ExpressionTreeNode
    {
        //*******************************************************************************************
        //*******************************************************************************************

        // return true if all operands are same size or are scalars

        private bool AllOperandsSameSize (out int rows, out int cols, Workspace workspace)
        {
            rows = 1;
            cols = 1;

            // find first matrix operand. use it to set numb rows & cols
            foreach (ExpressionTreeNode op in Operands)
            {
                op.Evaluate (workspace);

                if (op.Value is PLRMatrix || op.Value is PLCMatrix)
                {
                    rows = op.Value.Rows;
                    cols = op.Value.Cols;
                    break;
                }
            }

            foreach (ExpressionTreeNode op in Operands)
            {
                if (op.Value is PLRMatrix || op.Value is PLCMatrix)
                    if (op.Value.Rows != rows || op.Value.Cols != cols)
                        return false;
            }

            return true;
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void GetVariable (string variable)
        {
            Value = Workspace.Get (variable);            

            if (Value is PLScalar)
                return;

            if (Operands.Count == 0)
                return;    

            // save a copy of the input
            PLMatrix matValue  = Value as PLMatrix;

            if (matValue == null) 
                throw new Exception ("Cannot take submatrix of a scalar");

            bool srcIsRowVector = Value.IsRowVector;
            bool srcIsColVector = Value.IsColVector;

            //************************************************************************

            // A (3), A (5:9), A (:)

            if (Operands.Count == 1)
            {
                PLVariable select = Operands [0].Evaluate (workspace);

                PLRMatrix selectVect   = select as PLRMatrix;
                PLDouble  selectDouble = select as PLDouble;
                PLString  selectString = select as PLString;
                PLInteger selectInt    = select as PLInteger;

                if (selectString != null && selectString.Data == "All")
                {
                    Value = matValue.CollapseToColumn ();
                    return;
                }

                if (selectDouble != null)
                {
                    int i = (int)selectDouble.Data - 1;
                    Value = srcIsRowVector ? matValue.Get (0, i) : matValue.Get (i, 0);
                    return;
                }

                if (selectInt != null)
                {
                    int i = selectInt.Data - 1;
                    Value = srcIsRowVector ? matValue.Get (0, i) : matValue.Get (i, 0);
                    return;
                }

                if (selectVect != null)
                {
                    if (srcIsRowVector)
                    {
                        if (Value is PLRMatrix) Value = new PLRMatrix (1, selectVect.Size);
                        else                    Value = new PLCMatrix (1, selectVect.Size);

                        for (int i = 0; i<selectVect.Size; i++)
                        {
                            PLVariable v = matValue.IsRowVector ? matValue.Get (0, (int) selectVect [0, i] - 1)
                                                                : matValue.Get ((int) selectVect [0, i] - 1, 0);

                            (Value as PLMatrix).Set (0, i, v);
                        }
                    }

                    else if (srcIsColVector)
                    {
                        if (Value is PLRMatrix) Value = new PLRMatrix (selectVect.Size, 1);
                        else                    Value = new PLCMatrix (selectVect.Size, 1);

                        for (int i = 0; i<selectVect.Size; i++)
                        { 
                            PLVariable v = matValue.IsRowVector ? matValue.Get (0, (int) selectVect [0, i] - 1)
                                                                : matValue.Get ((int) selectVect [0, i] - 1, 0);

                            (Value as PLMatrix).Set (i, 0, v);
                        }
                    }

                    else throw new Exception ("Error extracting sub-matrix");

                    return;
                }

                throw new Exception ("Error extracting sub-matrix");
            }

            //************************************************************************

            // A (3, 4), A (2:4, 5:7)

            else if (Operands.Count == 2)
            {
                PLVariable selectRows = Operands [0].Evaluate (Workspace);
                PLVariable selectCols = Operands [1].Evaluate (Workspace);

                PLRMatrix selectRowsVect  = selectRows as PLRMatrix; // actually a vector
                PLDouble  selectRowScalar = selectRows as PLDouble;
                PLString  selectRowString = selectRows as PLString;

                List<int> rows = new List<int> ();

                if (selectRowsVect  != null) {for (int i = 0; i<selectRowsVect.Size; i++) rows.Add ((int) selectRowsVect [i] - 1); }
                if (selectRowScalar != null) {rows.Add ((int) selectRowScalar.Data - 1); }
                if (selectRowString != null) {for (int i = 0; i<matValue.Rows; i++) rows.Add (i); }

                PLRMatrix selectColsVect = selectCols as PLRMatrix; // actually a vector
                PLDouble selectColScalar = selectCols as PLDouble;
                PLString selectColString = selectCols as PLString;

                List<int> cols = new List<int> ();

                if (selectColsVect  != null) {for (int i = 0; i<selectColsVect.Size; i++) cols.Add ((int) selectColsVect [i] - 1); }
                if (selectColScalar != null) {cols.Add ((int) selectColScalar.Data - 1); }
                if (selectColString != null) {for (int i = 0; i<matValue.Cols; i++) cols.Add (i); }

                if (Value is PLRMatrix) Value = new PLRMatrix (rows.Count, cols.Count);
                else                    Value = new PLCMatrix (rows.Count, cols.Count);

                for (int r=0; r<rows.Count; r++)
                {
                    for (int c=0; c<cols.Count; c++)
                    {
                        PLVariable v = matValue.Get (rows [r], cols [c]);
                        (Value as PLMatrix).Set (r, c, v);
                    }
                }
            }
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Logical_And ()
        {
            bool val = true;

            foreach (ExpressionTreeNode node in Operands)
            {
                if      (node.Value is PLDouble)  val &= ((node.Value as PLDouble).Data  != 0);
                else if (node.Value is PLInteger) val &= ((node.Value as PLInteger).Data != 0);
                else if (node.Value is PLBool)    val &=  (node.Value as PLBool).Data;
                else throw new Exception ("Logical AND argument type error");
                Value = new PLBool (val);
            }
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Logical_Not ()
        {
            Value = new PLBool (!(Operands [0].Value as PLBool).Data);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Logical_Or ()
        {
            bool val = false;

            foreach (ExpressionTreeNode node in Operands)
            {
                if      (node.Value is PLDouble)  val |= ((node.Value as PLDouble).Data  != 0);
                else if (node.Value is PLInteger) val |= ((node.Value as PLInteger).Data != 0);
                else if (node.Value is PLBool)    val |=  (node.Value as PLBool).Data;
                else throw new Exception ("Logical OR argument type error");
                Value = new PLBool (val);
            }
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_EqualityTest ()
        {
            if (Operands.Count != 2) 
                throw new Exception ("Equality test requires two operands");
            
            bool failFlag = false;

            double v1 = PLVariable.ToNumber (Operands [0].Value, ref failFlag);
            double v2 = PLVariable.ToNumber (Operands [1].Value, ref failFlag);

            if (failFlag == true)
                throw new Exception ("Inequality test argument type error");

            Value = new PLBool (v1 == v2);
        }
        
        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Inequality ()
        {
            if (Operands.Count != 2) 
                throw new Exception ("Inequality test requires two operands");
            
            bool failFlag = false;

            double v1 = PLVariable.ToNumber (Operands [0].Value, ref failFlag);
            double v2 = PLVariable.ToNumber (Operands [1].Value, ref failFlag);

            if (failFlag == true)
                throw new Exception ("Inequality test argument type error");

            Value = new PLBool (v1 != v2);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Equal ()
        {
            if (Operands.Count != 2)
                throw new Exception ("Operand count error at \'=\'");
            /***/
            string LeftSideName = Operands [0].Operator;

            if (workspace.Contains (LeftSideName))
            {
                PLRMatrix LeftSide = workspace.Get (LeftSideName) as PLRMatrix;

                if (LeftSide != null) // left side is a matrix
                {
                    if (Operands [0].Operands.Count > 0) // left side is a aub-matrix
                    {
                        PLVariable overwriteData = Operands [1].Value;  // new data to replace some of the old
                        PLDouble overwrite1 = overwriteData as PLDouble;
                        PLRMatrix overwrite2 = overwriteData as PLRMatrix;


                        PLVariable r; // rows to overwrite
                        PLVariable c; // cols to overwrite

                        if (Operands [0].Operands.Count == 1 && LeftSide.Rows == 1)
                        {
                            r = new PLDouble (1);
                            c = Operands [0].Operands [0].Value;
                        }

                        else if (Operands [0].Operands.Count == 1 && LeftSide.Cols == 1)
                        {
                            r = Operands [0].Operands [0].Value;
                            c = new PLDouble (1);
                        }

                        else if (Operands [0].Operands.Count == 2)
                        {
                            r = Operands [0].Operands [0].Value; // rows to overwrite
                            c = Operands [0].Operands [1].Value; // cols to overwrite
                        }

                        else
                            throw new Exception ("Left side operand error");

                        PLDouble rr  = r as PLDouble;
                        PLDouble cc  = c as PLDouble;
                        PLRMatrix rrr = r as PLRMatrix;
                        PLRMatrix ccc = c as PLRMatrix;

                        List<int> rows = new List<int> (); // to be overwritten
                        List<int> cols = new List<int> ();

                        if (rr != null) rows.Add ((int) rr.Data);
                        else for (int i = 0; i<rrr.Size; i++) rows.Add ((int)rrr [0, i]);
                        
                        if (cc != null) cols.Add ((int) cc.Data);
                        else for (int i = 0; i<ccc.Size; i++) cols.Add ((int)ccc [0, i]);


                        workspace.OverwriteSubmatrix (LeftSideName,       // name of matrix already in workspace
                                                      rows [0], cols [0], // 1-based   <-------------------------------- ONLY FIRST NUMBER PASSED
                                                      overwriteData);     // new data to overwrite some of old

                        Value = workspace.Get (LeftSideName);
                        return;
                    }
                }
            }
            /***/

            Value = Operands [1].Value;
            Value.Name = Operands [0].Operator;
            workspace.Add (Value);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_GreaterTest ()
        {
            if (Operands.Count != 2) 
                throw new Exception ("GT test requires two operands");
            
            bool failFlag = false;

            double v1 = PLVariable.ToNumber (Operands [0].Value, ref failFlag);
            double v2 = PLVariable.ToNumber (Operands [1].Value, ref failFlag);

            if (failFlag == true)
                throw new Exception ("GT test argument type error");

            Value = new PLBool (v1 > v2);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_GreaterOrEqualTest ()
        {
            if (Operands.Count != 2) 
                throw new Exception ("GE test requires two operands");
            
            bool failFlag = false;

            double v1 = PLVariable.ToNumber (Operands [0].Value, ref failFlag);
            double v2 = PLVariable.ToNumber (Operands [1].Value, ref failFlag);

            if (failFlag == true)
                throw new Exception ("GE test argument type error");

            Value = new PLBool (v1 >= v2);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_LessTest ()
        {
            if (Operands.Count != 2) 
                throw new Exception ("LT test requires two operands");
            
            bool failFlag = false;

            double v1 = PLVariable.ToNumber (Operands [0].Value, ref failFlag);
            double v2 = PLVariable.ToNumber (Operands [1].Value, ref failFlag);

            if (failFlag == true)
                throw new Exception ("LT test argument type error");

            Value = new PLBool (v1 < v2);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_LessOrEqualTest ()
        {
            if (Operands.Count != 2) 
                throw new Exception ("LE test requires two operands");
            
            bool failFlag = false;

            double v1 = PLVariable.ToNumber (Operands [0].Value, ref failFlag);
            double v2 = PLVariable.ToNumber (Operands [1].Value, ref failFlag);

            if (failFlag == true)
                throw new Exception ("LE test argument type error");

            Value = new PLBool (v1 <= v2);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        // LIMITATION: assumes result is same type as first operand

        private void Operator_Plus ()
        {
            //if (AllOperandsSameSize (out int rows, out int cols, workspace))
            //{
            //    if (Operands [0].Value is PLString) Value = new PLString ("");
            //    else if (rows == 1 && cols == 1) Value = new PLDouble (0);

            //    else if (Operands [0].Value is PLRMatrix) Value = new PLRMatrix (rows, cols);
            //    else if (Operands [0].Value is PLCMatrix) Value = new PLCMatrix (rows, cols);

            //    else
            //        throw new Exception ("Operator_Plus error");

            Value = new PLDouble (0);

                foreach (ExpressionTreeNode op in Operands)
                    Value += op.Value;
            //}
            //else
            //    throw new Exception ("Matrix size error in \"+\"");
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Minus ()
        {
            //if (AllOperandsSameSize (out int rows, out int cols, workspace))
            //{
            //    if (rows == 1 && cols == 1) Value = new PLDouble (0);
            //    else                        Value = new PLRMatrix (rows, cols);

                Value = Operands [0].Value;
                            
                for (int i=1; i<Operands.Count; i++)
                        Value -= Operands [i].Value;
            //}
            //else
            //    throw new Exception ("Matrix size error in \"-\"");
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Divide ()
        {
            Value = Operands [0].Evaluate (workspace);

            for (int i = 1; i<Operands.Count; i++)
            {
                Operands [i].Evaluate (workspace);
                Value /= Operands [i].Value;
            }
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_DotDivide ()
        {
            Value = InternalFunctions.DotDivide (Operands [0].Value, Operands [1].Value);

            for (int i = 2; i<Operands.Count; i++)
                Value = InternalFunctions.DotDivide (Value, Operands [i].Evaluate (workspace));
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Multiply ()
        {
            Value = Operands [0].Value * Operands [1].Value;

            for (int i = 2; i<Operands.Count; i++)
                Value *= Operands [i].Value;
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_DotMultiply ()
        {
            Value = InternalFunctions.DotTimes (Operands [0].Value, Operands [1].Value);

            for (int i = 2; i<Operands.Count; i++)
                Value = InternalFunctions.DotTimes (Value, Operands [i].Value);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_Exponent ()
        {
            Value = Operands [0].Value ^ Operands [1].Value;
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_DotExponent ()
        {
            Value = Operands [0].Value ^ Operands [1].Value;
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_RowVectorElements ()
        {
            PLList values = new PLList ();

            foreach (ExpressionTreeNode node in Operands)
                values.Add (node.Evaluate (workspace));

            Value = InternalFunctions.RowVector (values);
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_RowVectorIterator ()
        {
            if (Operands.Count == 0)
            {
                Value = new PLString ("All");
            }

            else
            {
                double start, step, stop;

                PLDouble op1 = new PLDouble (Operands [0].Value);
                PLDouble op2 = new PLDouble (Operands [1].Value);
                start = op1.Data;

                if (Operands.Count == 2)
                {
                    step  = 1;
                    stop  = op2.Data;
                }
                else if (Operands.Count == 3)
                {
                    PLDouble op3 = new PLDouble (Operands [2].Value);
                    step = op2.Data;
                    stop = op3.Data;
                }
                else
                    throw new Exception ("Syntax error at \":\", operands");

                Value = InternalFunctions.RowVector (start, step, stop);
            }
        }

        //*******************************************************************************************
        //*******************************************************************************************

        private void Operator_ColVectorElements ()
        {
            PLList ops = new PLList ();

            foreach (ExpressionTreeNode node in Operands)
                ops.Add (node.Value);

            Value = InternalFunctions.ColVector (ops); 
        }

        //*******************************************************************************************
        //*******************************************************************************************

        //*******************************************************************************************
        //*******************************************************************************************



    }
}
