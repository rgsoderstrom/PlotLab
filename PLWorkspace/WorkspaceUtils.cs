using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommonMath;

using PLCommon;

namespace PLWorkspace
{
    internal class WorkspaceUtils
    {
        //***********************************************************************************************

        internal static PLVariable Rows (PLVariable a)
        {
            if (a is PLMatrix p1) return new PLInteger (p1.Rows);
            if (a is PLScalar)    return new PLInteger (1);
            if (a is PLList)      return new PLInteger (1);
            if (a is PLString)    return new PLInteger (1);
            throw new Exception ("Can\'t count the rows of a " + a.GetType ().ToString ());
        }

        internal static PLVariable Cols (PLVariable a)
        {
            if (a is PLMatrix p1) return new PLInteger (p1.Cols);
            if (a is PLScalar)    return new PLInteger (1);
            if (a is PLList   p3) return new PLInteger (p3.Count);
            if (a is PLString p4) return new PLInteger (p4.Text.Length);
            throw new Exception ("Can\'t count the columns of a " + a.GetType ().ToString ());
        }

        internal static PLVariable Size (PLVariable a)
        {
            if (a is PLList lst)
            {
                if (lst.Count == 1)
                    return Length (lst);

                if (lst.Count == 2)
                {
                    PLInteger intg = lst [1] as PLInteger;
                    PLDouble doub = lst [1] as PLDouble;
                    int select = 1; // 1 => return number rows, 2 => return number cols

                    if (intg != null) select = intg.Data;
                    if (doub != null) select = (int)doub.Data;

                    if (select == 1) return Rows (lst [0]);
                    if (select == 2) return Cols (lst [0]);
                    throw new Exception ("Argument error in \"size\" function");
                }
            }

            PLRMatrix s = new PLRMatrix (1, 2);
            s [0, 0] = (Rows (a) as PLInteger).Data; 
            s [0, 1] = (Cols (a) as PLInteger).Data; 

            return s;
        }

        internal static PLVariable Length (PLVariable a)
        {
            int rows = (Rows (a) as PLInteger).Data;
            int cols = (Cols (a) as PLInteger).Data;
            return new PLInteger (Math.Max (rows, cols));
        }

        //*****************************************************************************************
        //
        // Replace part of a matrix
        //    - LIMITATION: assumes data will overwrite consecutive rows, cols
        //
        internal static void OverwriteSubmatrix (WorkspaceBase ws,
                                                 string name,            // name of matrix already in workspace
                                                 int tlcRow, int tlcCol, // 1-based
                                                 PLVariable newData)     // new data to overwrite some of old
        {
            // get data currently in workspace and do some size chexks
            PLVariable oldData = null; // the target, matrix or vector
            bool found = ws.Get (name, ref oldData);

            if (found == false)
                throw new Exception ("Faild to find " + name + " in workspace " + ws.Name);

            if (tlcRow < 1 || tlcCol < 1 || tlcRow > oldData.Rows || tlcCol > oldData.Cols)
                throw new Exception ("Top left corner indices out of range");

            if (tlcRow + newData.Rows > oldData.Rows + 1 || tlcCol + newData.Cols > oldData.Cols + 1)
                throw new Exception ("Attempt to write outside of matrix " + name);

            //***************************************************************************
            
            if (newData is PLScalar)
            {
                if (newData is PLDouble && oldData is PLRMatrix) // real scalar written into real matrix
                    (oldData as PLRMatrix) [tlcRow - 1, tlcCol - 1] = (newData as PLDouble).Data;

                else if (newData is PLComplex && oldData is PLCMatrix) // complex scalar written into complex matrix
                    (oldData as PLCMatrix) [tlcRow - 1, tlcCol - 1] = new PLComplex ((newData as PLComplex).Data);

                else if (newData is PLComplex && oldData is PLRMatrix) // complex scalar written into real matrix
                {
                    // make a complex matrix from old real data
                    CMatrix cmatrix = new CMatrix ((oldData as PLRMatrix).Data);

                    // do the overwrite
                    cmatrix [tlcRow - 1, tlcCol - 1] = (newData as PLComplex).Data;

                    // replace original with cmatrix in Workspace
                    oldData = new PLCMatrix (cmatrix);
                    oldData.Name = name;
                    ws.Add (oldData);
                }

                else if (newData is PLDouble && oldData is PLCMatrix) // real scalar written into complex matrix
                    (oldData as PLCMatrix) [tlcRow - 1, tlcCol - 1] = new PLComplex ((newData as PLDouble).Data);

                else
                    throw new Exception ("Unsupported matrix-scalar overwrite option");
            }

            else // newData is a matrix or vector
            {
                if (newData is PLRMatrix && oldData is PLRMatrix) // real matrix written into real matrix
                {
                    for (int c = 0; c<newData.Cols; c++)
                        for (int r = 0; r<newData.Rows; r++)
                            (oldData as PLRMatrix) [tlcRow - 1 + r, tlcCol - 1 + c] = (newData as PLRMatrix) [r, c];
                }

                else if (newData is PLCMatrix && oldData is PLCMatrix) // complex matrix written into complex matrix
                {
                    for (int c = 0; c<newData.Cols; c++)
                        for (int r = 0; r<newData.Rows; r++)
                            (oldData as PLCMatrix) [tlcRow - 1 + r, tlcCol - 1 + c] = (newData as PLCMatrix) [r, c];
                }

                else if (newData is PLCMatrix && oldData is PLRMatrix) // complex matrix written into real matrix
                {
                    // make a complex matrix from old real data
                    CMatrix cmatrix = new CMatrix ((oldData as PLRMatrix).Data);

                    // do the overwrite
                    for (int c = 0; c<newData.Cols; c++)
                        for (int r = 0; r<newData.Rows; r++)
                            cmatrix [tlcRow - 1 + r, tlcCol - 1 + c] = (newData as PLCMatrix) [r, c].Data;

                    // replace original with cmatrix in Workspace
                    oldData = new PLCMatrix (cmatrix);
                    oldData.Name = name;
                    ws.Add (oldData);
                }

                else if (newData is PLRMatrix && oldData is PLCMatrix) // real matrix written into complex matrix
                {
                    for (int c = 0; c<newData.Cols; c++)
                        for (int r = 0; r<newData.Rows; r++)
                            (oldData as PLCMatrix) [tlcRow - 1 + r, tlcCol - 1 + c] = new PLComplex ((newData as PLRMatrix) [r, c]);
                }

                else
                    throw new Exception ("Unsupported matrix-matrix overwrite option");
            }
        }

        //************************************************************************


    }
}
