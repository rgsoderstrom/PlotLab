using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;
using PLLibrary;

namespace Main
{
    internal partial class ExpressionTreeNode
    {
        //*********************************************************************************
        //*********************************************************************************

        public PLVariable Evaluate (Workspace workspace)
        {
            if (ValueValid)
                return Value;

            switch (NodeType)
            {
                case TokenType.VariableName:
                    GetVariable (Operator);
                    break;

                //case TokenType.Numeric:
                //break;

                //case TokenType.String:
                //    break;


                case TokenType.Brackets:
                case TokenType.BracketsColon:
                case TokenType.BracketsSemi:
                case TokenType.BracketsComma:
                case TokenType.BracketsSpace:
                case TokenType.ArithmeticOperator:
                    Evaluate_Operator (Operator, workspace);
                    break;

                case TokenType.FunctionName:
                    Evaluate_Function (workspace);//, expression);
                    break;

                case TokenType.FunctionFile:
                    Evaluate_MFileFunction (workspace);//, expression);
                    break;


                default: throw new Exception ("Not found: " + Operator.ToString ());
                //default: throw new Exception ("Evaluate: node type not supported: " + NodeType.ToString ());
            }

            return Value;
        }

        //*********************************************************************************
        //*********************************************************************************

        PLVariable Evaluate_MFileFunction (Workspace callersWorkspace)//, string expression)
        {
            throw new Exception ("M-file not implemented");

            //// Create new (empty) local workspace
            //// Copy passed-in operands into that workspace, with names of input parameters (from m-file)
            //// Pass function lines to ScriptProcessor
            //// Copy output parameters to Value, as ordered list
            ////  - PLList


            //Workspace functionsWorkspace = new Workspace ();
            //ScriptProcessor sp = new ScriptProcessor (functionsWorkspace, PF);

            //string funcName = Operator; // a = f1 (b); 
            //string fullName = ""; // full path to f1.m, filled-in below
            //MFileFunctionProcessor mfileProc = null;

            //try
            //{
            //    if (MFileFunctionMgr.IsMFileFunction (funcName, ref fullName, ref mfileProc))
            //    {
            //        if (mfileProc == null)
            //        {
            //            try
            //            {
            //                mfileProc = MFileFunctionMgr.ParseMFile (funcName, fullName); // Split the m-file into inputs, executable and outputs
            //            }

            //            catch (Exception ex)
            //            {
            //                PF ("Error parsing m-file: " + ex.Message);
            //            }
            //        }

            //        //*****************************************************************************************
            //        //
            //        // evaluate the arguments passed to the function using the caller's workspace and get them in one list
            //        //
            //        List<PLVariable> inputArguments = new List<PLVariable> ();

            //        foreach (ExpressionTreeNode etn in Operands)
            //            inputArguments.Add (etn.Evaluate (callersWorkspace));

            //        //
            //        // attach the names to the values and put into the function's local workspace
            //        //
            //        for (int i=0; i<inputArguments.Count; i++)
            //        {
            //            inputArguments [i].Name = mfileProc.InputFormalParams [i];
            //            functionsWorkspace.Add (inputArguments [i]);
            //        }

            //        //
            //        // run the executable lines just like a script
            //        //
            //        ScriptProcessor scriptProcessor = new ScriptProcessor (functionsWorkspace, PF);
            //        scriptProcessor.RunScriptLines (mfileProc.ExecutableScript);

            //        //
            //        // If there is more than one output we place the outputs in the caller's workspace, using the names
            //        // the caller specifies.
            //        //
            //        // Caller must specify either 0 outputs or the same number as the number of formal output arguments
            //        //

            //        // look for an equal sign
            //        int equalIndex = -1;

            //        for (int i = 0; i<expression.Length; i++) {if (expression [i] == '=') {equalIndex = i; break;}}

            //        if (equalIndex != -1) // then some outputs were specified
            //        {
            //            string [] outputsAsTokens = expression.Substring (0, equalIndex - 1).Split (new char [] { '[', ']', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //            if (outputsAsTokens.Length != mfileProc.OutputFormalParams.Count)
            //                throw new Exception ("Function call must have 0 outputs or the same number as function formal param list: " + funcName);

            //            if (mfileProc.OutputFormalParams.Count > 1)
            //            {
            //                for (int i = 0; i<mfileProc.OutputFormalParams.Count; i++)
            //                {
            //                    PLDouble dbl = new PLDouble (functionsWorkspace.Get (mfileProc.OutputFormalParams [i]));
            //                    dbl.Name = outputsAsTokens [i];
            //                    callersWorkspace.Add (dbl);
            //                }
            //            }
            //        }

            //        //
            //        // ... and in this node's Value field
            //        //

            //        if (mfileProc.OutputFormalParams.Count == 1)
            //        {
            //            Value = functionsWorkspace.Get (mfileProc.OutputFormalParams [0]);
            //        }
            //        else
            //        {
            //            Value = new PLList ();

            //            foreach (string str in mfileProc.OutputFormalParams)
            //            {
            //                (Value as PLList).Add (functionsWorkspace.Get (str));
            //            }
            //        }
            //    }
            //}

            //catch (Exception ex)
            //{
            //    throw new Exception ("Error evaluating m-file function " + funcName + ": " + ex.Message);
            //}

            //return new PLNull ();
        }

        //*********************************************************************************
        //*********************************************************************************

         PLVariable Evaluate_Operator (string Operator, Workspace workspace)
        {
            switch (Operator)
            {
                case "=":
                    Operator_Equal ();
                    break;

                case "&":
                case "&&":
                    Operator_Logical_And ();
                    break;

                //case "Not":
                //    Operator_Logical_Not ();
                //    break;

                case "|":
                case "||":
                    Operator_Logical_Or ();
                    break;

                case "==":
                    Operator_EqualityTest ();
                    break;

                case "~=":
                    Operator_Inequality ();
                    break;

                case ">":
                    Operator_GreaterTest ();
                    break;

                case ">=":
                    Operator_GreaterOrEqualTest ();
                    break;

                case "<":
                    Operator_LessTest ();
                    break;

                case "<=":
                    Operator_LessOrEqualTest ();
                    break;

                case "+":
                    Operator_Plus ();
                    break;

                case "-":
                    Operator_Minus ();
                    break;

                case "/":
                    Operator_Divide ();
                    break;

                case "./":
                    Operator_DotDivide ();
                    break;

                case "*":
                    Operator_Multiply ();
                    break;

                case ".*":
                    Operator_DotMultiply ();
                    break;

                case "^":
                    Operator_Exponent ();
                    break;

                case ".^":
                    Operator_DotExponent ();
                    break;

                case "RowVectorElements":
                case "Comma":
                case ",":
                    Operator_RowVectorElements ();
                    break;

                case "RowVectorIterator":
                case ":":
                    Operator_RowVectorIterator ();
                    break;

                case "ColVectorElements":
                case ";":
                    Operator_ColVectorElements ();
                    break;

                default:
                    throw new Exception ("Evaluate: Unsupported operator " + Operator + ", " + NodeType);
            }

            return Value;
        }

        //*****************************************************************************************************
        //*****************************************************************************************************
        //*****************************************************************************************************

        void Evaluate_Function (Workspace workspace)
        {
            bool forcePrint = false;

            if (Operands.Count == 0)
            {
                PLFunction func = LibraryManager.GetFunctionDelegate (Operator);

                if (LibraryManager.IsZeroArgFunction (Operator)) // if function can be invoked with zero args ...
                    Value = func (new PLNull ()); 
                else
                    Value = new PLFunctionWrapper (func);
            }

            else if (Operands.Count == 1)
            {
                Operands [0].Evaluate (workspace);

                PLString functionName = new PLString (Operator);

                if (LibraryManager.Contains (functionName))
                    Value = LibraryManager.Evaluate (new PLString (Operator), Operands [0].Value, ref forcePrint);

                else if (workspace.Functions.ContainsKey (functionName.Data))
                    Value = workspace.Evaluate (functionName, Operands [0].Value);
                
                else // if (ValueValid == false)
                {
                    if (Operator == "Transpose")
                    {
                        if (Operands [0].Value is PLRMatrix)
                            Value = InternalFunctions.Transpose (Operands [0].Value as PLRMatrix);
                        else
                            throw new Exception ("Can only transpose matrices");
                    }

                    if (Operator == "Not")
                    {
                        Operator_Logical_Not ();
                    }
                }

                if (ValueValid == false) throw new Exception ("Error evaluating expression");                

            }
            else // operand count > 1
            {
                PLList args = new PLList ();

                foreach (ExpressionTreeNode op in Operands)
                    args.Add (op.Evaluate (workspace));

                PLString oper = new PLString (Operator);

                if (LibraryManager.Contains (oper))
                    Value = LibraryManager.Evaluate (oper, args, ref forcePrint);

                else if (workspace.Functions.ContainsKey (Operator))
                    Value = workspace.Evaluate (oper, args);

                else
                    throw new Exception ("Can't find function " + Operator);
            }

            if (Value is PLList)
            {
                throw new Exception ("Not implemented");

                //PLList lst = Value as PLList;

                //// look for an equal sign in output args
                //int equalIndex = -1;

                //for (int i = 0; i<expression.Length; i++) {if (expression [i] == '=') { equalIndex = i; break;}}

                //if (equalIndex != -1) // then some outputs were specified
                //{
                //    string [] outputsAsTokens = expression.Substring (0, equalIndex - 1).Split (new char [] { '[', ']', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //    //if (outputsAsTokens.Length != lst.Count)
                //    //    throw new Exception ("Function call must have 0 outputs or the same number as function formal param list: " + expression);

                //    int count = Math.Min (outputsAsTokens.Length, lst.Count);

                //    for (int i = 0; i<count; i++)
                //    {
                //        lst [i].Name = outputsAsTokens [i];
                //        workspace.Add (lst [i]);
                //    }
                //}

                //Value = lst [0]; // PlotLab code expects only a single value returned. Other output parameters have
                //                 // been placed in the caller's workspace so code will function as expected
            }
        }
    }
}
