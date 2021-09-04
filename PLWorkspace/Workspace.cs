using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLWorkspace
{
    public partial class Workspace
    {
        Dictionary<string, PLVariable> Variables = new Dictionary<string, PLVariable> ();
        Dictionary<string, PLFunction> Commands  = new Dictionary<string, PLFunction> ();

        public Dictionary<string, PLFunction> Functions = new Dictionary<string, PLFunction> ();

        static Dictionary<string, PLVariable> Constants = new Dictionary<string, PLVariable> ();

        public static PrintFunction Print = null;

        public Workspace ()
        {
            Dictionary<string, PLFunction> workCmnds = GetCommands ();
            foreach (string str in workCmnds.Keys) Commands.Add (str, workCmnds [str]);

            Dictionary<string, PLFunction> workFuncs = GetFunctions ();
            foreach (string str in workFuncs.Keys) Functions.Add (str, workFuncs [str]);
        }

        //***************************************************************************************************

        static Workspace ()
        {
            PLDouble PI = new PLDouble (3.14159265358979); PI.Name = "PI"; Constants.Add ("PI", PI); Constants.Add ("pi", PI);
            PLDouble e  = new PLDouble (2.71828182845905); e.Name = "e";   Constants.Add ("e", e);

            PLBool TRUE  = new PLBool (true);  TRUE.Name = "true";   Constants.Add ("true", TRUE);
            PLBool FALSE = new PLBool (false); FALSE.Name = "false"; Constants.Add ("false", FALSE);

            Constants.Add ("equal",  new PLString ("equal"));
            Constants.Add ("tight",  new PLString ("tight"));
            Constants.Add ("frozen", new PLString ("frozen"));
            Constants.Add ("auto",   new PLString ("auto"));
        }

    //***************************************************************************************************

        public PLVariable RunCommand (PLString cmnd, PLList args)
        {
            if (Commands.ContainsKey (cmnd.Text))
            {
                PLFunction func = Commands [cmnd.Text];
                return func (args);
            }

            return new PLNull ();
        }

        public PLVariable Evaluate (PLString funcName, PLVariable args)
        {
            if (Functions.ContainsKey (funcName.Text))
            {
                PLFunction func = Functions [funcName.Text];
                return func (args);
            }

            throw new Exception ("Workspace function " + funcName + " not found");
        }

        //***************************************************************************************************

        public void PrintKeys (PrintFunction pf)
        {
            string ostr = "";

            System.Collections.IDictionary dict = Variables;

            foreach (string str in dict.Keys)
            {
                ostr += str;
                ostr += "\t";

                if (ostr.Length > 40)
                {
                    pf (ostr + "\n");
                    ostr = "";
                }
            }

            if (ostr.Length > 0)
                pf (ostr + "\n");
        }

        //***************************************************************************************************

        public void PrintKeysAndSizes (PrintFunction pf)
        {
            System.Collections.IDictionary dict = Variables;

            foreach (string str in dict.Keys)
            {
                string printString = "";
                object val = dict [str];
                string typeStr = val.GetType ().Name;

                typeStr = typeStr.Replace ("PL", "");
                typeStr = typeStr.ToLower ();

                if (typeStr.Length < 8)
                    typeStr = typeStr.PadRight (8);  // pad short strings so all line up

                printString += (str + "\t" + typeStr + "\t");

                if (val is PLMatrix)
                {
                    PLMatrix var = val as PLMatrix;
                    printString += (string.Format ("    {0} x {1}", var.Rows, var.Cols));
                }

                pf (printString);
                pf ("\n");
            }
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************
        
        /// <summary>
        /// WhatIs - determine whether a name represents a variable, a workspace operation or neither
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        
        public SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = SymbolicNameTypes.Unknown;

            if      (Variables.ContainsKey (str)) {type = SymbolicNameTypes.Variable;}
            else if (Constants.ContainsKey (str)) {type = SymbolicNameTypes.Variable;}
            else if (Commands.ContainsKey  (str)) {type = SymbolicNameTypes.WorkspaceCommand;}
            else if (Functions.ContainsKey (str)) {type = SymbolicNameTypes.Function;}  // WorkspaceFunction

            return type;
        }

        //*****************************************************************************************
        //
        // Add or change a variable
        //
        public void Add (PLVariable var)
        {
            if (Variables.ContainsKey (var.Name)) Variables [var.Name] = var;
            else Variables.Add (var.Name, var);
        }

        //*****************************************************************************************
        //
        // See whether workspace contains a variable
        //
        public bool Contains (string var)
        {
            return Variables.ContainsKey (var);
        }

        //*****************************************************************************************
        //
        // Replace part of a matrix
        //    - LIMITATION: assumes data will overwrite consecutive rows, cols
        //
        public void OverwriteSubmatrix (string name,            // name of matrix already in workspace
                                        int tlcRow, int tlcCol, // 1-based
                                        PLVariable var)         // new data to overwrite some of old
        {
            PLMatrix mat = Get (name) as PLMatrix;

            PLMatrix  submat = var as PLMatrix; // see what type the input data is
            PLDouble  value  = var as PLDouble;
            PLInteger iValue = var as PLInteger;

            if (mat == null) throw new Exception (name + " is not a matrix");

            if (submat != null)
            {
                // ensure it will fit
                if (tlcRow + submat.Rows > mat.Rows + 1 || tlcCol + submat.Cols > mat.Cols + 1)
                    throw new Exception ("Attempt to write outside of matrix " + name);

                for (int c=0; c<submat.Cols; c++)
                    for (int r=0; r<submat.Rows; r++)
                        mat [tlcRow - 1 + r, tlcCol - 1 + c] = submat [r, c];
            }

            else if (value != null)
                mat [tlcRow - 1, tlcCol - 1] = value.Data;

            else if (iValue != null)
                mat [tlcRow - 1, tlcCol - 1] = iValue.Data;
        }

        //*****************************************************************************************
        //
        // Get a variable
        //
        public PLVariable Get (string name)
        {
            if (Variables.ContainsKey (name))
                return Variables [name];

            if (Constants.ContainsKey (name))
                return Constants [name];

            throw new Exception ("Variable " + name + " undefined");
        }

        //***************************************************************************************************
        //***************************************************************************************************
        //***************************************************************************************************
        
        //
        // Workspace Commands
        //

        public Dictionary<string, PLFunction> GetCommands ()
        {
            return new Dictionary<string, PLFunction> ()
            {
                {"clear",  Clear},
                {"who",    Who},
                {"whos",   Whos},
                {"dump",   Dump},
            };
        }
        
        //
        // Workspace Functions
        //

        public Dictionary<string, PLFunction> GetFunctions ()
        {
            return new Dictionary<string, PLFunction> ()
            {
                {"exists", Exists},
                {"rows",   Rows},
                {"cols",   Cols},
                {"length", Length},
                {"size",   Size},
            };
        }

        //***********************************************************************************************

        public PLVariable Clear (PLVariable sel)
        {
            if (sel != null)
            {
                PLList lst = sel as PLList;

                if (lst.Count == 0)
                    Variables.Clear ();

                else
                {
                    foreach (PLString str in lst)
                    {
                        if (str.Data == "all")
                        {
                            Variables.Clear ();
                            break;
                        }

                        Variables.Remove (str.Data);
                    }
                }
            }

            else
                Variables.Clear ();

            return new PLNull ();
        }

        //***********************************************************************************************

        public PLVariable Exists (PLVariable arg)
        {
            if (arg != null)
            {
                PLList lst = arg as PLList;
                PLString str = lst [0] as PLString;
                if (Variables.ContainsKey (str.Data))
                    return new PLBool (true);
            }

            return new PLBool (false);
        }

        //***********************************************************************************************

        public PLVariable Who (PLVariable _)
        {
            PrintKeys (Print);
            return new PLNull ();
        }

        public PLVariable Whos (PLVariable _)
        {
            PrintKeysAndSizes (Print);
            return new PLNull (); 
        }

        public PLVariable Dump (PLVariable _)
        {
            Print ("Workspace contents:\n");

            System.Collections.IDictionary dict = Variables;

            foreach (string str in dict.Keys)
            {
                string printString = "";
                object val = dict [str];
                string typeStr = val.GetType ().Name;

                typeStr = typeStr.Replace ("PL", "");
                typeStr = typeStr.ToLower ();

                if (typeStr.Length < 8)
                    typeStr = typeStr.PadRight (8);  // pad short strings so all line up

                printString += (str + "\t" + typeStr + "\t");

                if (val is PLMatrix)
                {
                    PLMatrix var = val as PLMatrix;
                    printString += (string.Format ("    {0} x {1}", var.Rows, var.Cols));
                }

                Print (printString);
                Print ("\n");
                Print (val.ToString ());
                Print ("\n");
            }

            return null;
        }

        //***********************************************************************************************

        public PLVariable Rows (PLVariable a)
        {
            PLMatrix  p1 = a as PLMatrix;  if (p1 != null) return new PLInteger (p1.Rows); 
            PLList    p2 = a as PLList;    if (p2 != null) return new PLInteger (1); 
            PLDouble  p3 = a as PLDouble;  if (p3 != null) return new PLInteger (1); 
            PLInteger p4 = a as PLInteger; if (p4 != null) return new PLInteger (1);  
            PLString  p5 = a as PLString;  if (p5 != null) return new PLInteger (1);  
            throw new Exception ("Unsupported type " + a.ToString () + " to rows function");
        }

        public PLVariable Cols (PLVariable a)
        {
            PLMatrix  p1 = a as PLMatrix;  if (p1 != null) return new PLInteger (p1.Cols); 
            PLList    p2 = a as PLList;    if (p2 != null) return new PLInteger (p2.Count); 
            PLDouble  p3 = a as PLDouble;  if (p3 != null) return new PLInteger (1); 
            PLInteger p4 = a as PLInteger; if (p4 != null) return new PLInteger (1);  
            PLString  p5 = a as PLString;  if (p5 != null) return new PLInteger (p5.Text.Length);  
            throw new Exception ("Unsupported type " + a.ToString () + " to cols function");
        }

        public PLVariable Size (PLVariable a)
        {
            PLList lst = a as PLList;

            if (lst != null)
            {
                if (lst.Count == 1)
                    return Length (lst);

                if (lst.Count == 2)
                {
                    PLInteger intg = lst [1] as PLInteger;
                    PLDouble  doub = lst [1] as PLDouble;
                    int select = 1; // 1 => return number rows, 2 => return number cols

                    if (intg != null) select = intg.Data;
                    if (doub != null) select = (int) doub.Data;

                    if (select == 1) return Rows (lst [0]);
                    if (select == 2) return Cols (lst [0]);
                    throw new Exception ("Argument error in \"size\" function");
                }
            }

            return Rows (a) * Cols (a);
        }

        public PLVariable Length (PLVariable a)
        {
            return Rows (a) * Cols (a);
        }

        //***********************************************************************************************

    }
}


