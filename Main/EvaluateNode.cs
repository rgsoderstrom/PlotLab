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
                    Evaluate_Function (workspace);
                    break;

                case TokenType.FunctionFile:
                    Evaluate_MFileFunction (workspace);
                    break;


                default: throw new Exception ("Not found: " + Operator.ToString ());
                //default: throw new Exception ("Evaluate: node type not supported: " + NodeType.ToString ());
            }

            return Value;
        }

        //*********************************************************************************
        //*********************************************************************************

        PLVariable Evaluate_MFileFunction (Workspace global)
        {
       //   Value = new PLDouble (456);

            PLDouble num = (PLDouble) Operands [0].Evaluate (workspace);
            PLDouble den = (PLDouble) Operands [1].Evaluate (workspace);

            Value = new PLDouble (num.Data / den.Data);


            // Create new (empty) workspace
            // Copy passed-in operands into local workspace, with names of input parameters (from m-file)
            // Pass function lines to ScriptProcessor
            // Copy output parameters from local workspace to global


            Workspace local = new Workspace ();
            ScriptProcessor sp = new ScriptProcessor (local, PF);








            return new PLNull ();
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
                        if (Operands [0].Value is PLMatrix)
                            Value = InternalFunctions.Transpose (Operands [0].Value as PLMatrix);
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
        }
    }
}
