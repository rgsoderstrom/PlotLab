using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;
using PLLibrary;

namespace PLMain
{
    internal partial class ExpressionTreeNode
    {
        //*********************************************************************************
        //*********************************************************************************

        public PLVariable Evaluate ()//IWorkspace workspace)
        {
            if (ValueValid)
                return Value;

            switch (NodeType)
            {
                case TokenType.VariableName:
                    GetVariable (Operator);
                    break;

                case TokenType.Numeric:
                    break;

                //case TokenType.String:
                //    break;


                case TokenType.Brackets:
                case TokenType.BracketsColon:
                case TokenType.BracketsSemi:
                case TokenType.BracketsComma:
                case TokenType.BracketsSpace:
                case TokenType.BinaryOperator:
                case TokenType.EqualSign:
                    Evaluate_Operator (Operator);
                    break;

                case TokenType.FunctionWithArgs:
                    Evaluate_Function ();
                    break;

                case TokenType.ZeroArgFunction:
                    Evaluate_ZeroArgFunction ();
                    break;

                case TokenType.FunctionFile:
                    Evaluate_MFileFunction ();
                    break;


                default: throw new Exception ("Can't find : " + Operator.ToString ());
                //default: throw new Exception ("Evaluate: node type not supported: " + NodeType.ToString ());
            }

            return Value;
        }

        //*********************************************************************************
        //*********************************************************************************

        // Caller's syntax:
        // [actual output parameters] = functionName (actual input parameters)

        // Function Name - In this node's Operator field
        // Actual input parameters - In this node's list of Operands
        // Actual output parameters - Not known at this level. This function returns a list of outputs, with one 
        //                            entry corresponding to each formal output parameter. Caller extracts them
        //                            and assigns each to an actual output parameter.

        PLVariable Evaluate_MFileFunction ()
        {
            //***********************************************

            // Evaluate the input args in caller's workspace

            string functionName = Operator;

            // function inputs
            List<PLVariable> actualInputsParameters = new List<PLVariable> ();

            foreach (ExpressionTreeNode operand in Operands)
                actualInputsParameters.Add (operand.Value);

            //***********************************************

            // Create a workspace for the function
            Workspace.Push (functionName);

            //***********************************************

            // Load and run the function
            FunctionProcessor processor = new FunctionProcessor (functionName);
            PLVariable outputs = processor.Run (actualInputsParameters);

            //***********************************************

            // Pop and discard the function's workspace
            Workspace.Pop ();

            //***********************************************

            // Put the function outputs in this node's Value field

            Value = outputs;
            return new PLNull ();
        }

        //*********************************************************************************
        //*********************************************************************************

         PLVariable Evaluate_Operator (string Operator)
        {
            try
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

            catch (Exception ex)
            {
                throw new Exception (ex.Message);
             // throw new Exception ("Error " + ex.Message + " evaluating " + Operator);
            }
        }

        //*****************************************************************************************************
        //*****************************************************************************************************
        //*****************************************************************************************************

        void Evaluate_ZeroArgFunction ()
        {
            if (LibraryManager.IsZeroArgFunction (Operator))
                Value = LibraryManager.RunZeroArgFunction (Operator);
            else
                throw new Exception ("Error evaluating " + Operator + " as a zero-arg function");
        }

        //*****************************************************************************************************
        //*****************************************************************************************************
        //*****************************************************************************************************

        void Evaluate_Function ()
        {
            if (Operands.Count == 0)
            {
                PLFunction func = LibraryManager.GetFunctionDelegate (Operator);

                //if (LibraryManager.IsZeroArgFunction (Operator)) // if function can be invoked with zero args ...
                //    Value = func (new PLNull ());
                //else
                    Value = new PLFunctionWrapper (func);
            }

            else if (Operands.Count == 1)
            {
                Operands [0].Evaluate ();

                if (LibraryManager.IsFunctionWithArgs (Operator))
                    Value = LibraryManager.Evaluate (Operator, Operands [0].Value);

                else if (Workspace.IsFunction (Operator))
                    Value = Workspace.EvaluateFunction (Operator, Operands [0].Value);


                else if (Operator == "not")
                {
                    Operator_Logical_Not ();
                }

                else 
                    throw new Exception ("Error evaluating node, function \"" + Operator + "\" not found");
                    //throw new Exception ("EvaluateNode, Operands.Count == 1 not implemented for operator " + Operator);
            }

            else // operand count > 1
            {
                PLList args = new PLList ();

                foreach (ExpressionTreeNode op in Operands)
                    args.Add (op.Evaluate ());

                if (LibraryManager.IsFunctionWithArgs (Operator))
                    Value = LibraryManager.Evaluate (Operator, args);

                else if (Workspace.IsFunction (Operator))
                    Value = Workspace.EvaluateFunction (Operator, args);

                else
                    throw new Exception ("Can't find function " + Operator);
            }

            if (Value is PLList)
                return;
        //      throw new Exception ("EvaluateNode, function returning list not implemented");

                ////if (Value is PLList)
                ////{
                ////    PLList lst = Value as PLList;

                ////    // look for an equal sign in output args
                ////    int equalIndex = -1;

                ////    for (int i = 0; i<expression.Length; i++) {if (expression [i] == '=') { equalIndex = i; break;}}

                ////    if (equalIndex != -1) // then some outputs were specified
                ////    {
                ////        string [] outputsAsTokens = expression.Substring (0, equalIndex - 1).Split (new char [] { '[', ']', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                ////        //if (outputsAsTokens.Length != lst.Count)
                ////        //    throw new Exception ("Function call must have 0 outputs or the same number as function formal param list: " + expression);

                ////        int count = Math.Min (outputsAsTokens.Length, lst.Count);

                ////        for (int i = 0; i<count; i++)
                ////        {
                ////            lst [i].Name = outputsAsTokens [i];
                ////            workspace.Add (lst [i]);
                ////        }
                ////    }

                ////    Value = lst [0]; // PlotLab code expects only a single value returned. Other output parameters have
                ////                     // been placed in the caller's workspace so code will function as expected
            //}
        }
    }
}
