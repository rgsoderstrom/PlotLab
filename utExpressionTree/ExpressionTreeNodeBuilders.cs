using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;
using PLCommon;

namespace Main
{
    internal partial class ExpressionTreeNode
    {
        //*************************************************************************************************
        //*************************************************************************************************

        internal void BuildNodeFrom_List (List<IToken> tokens)
        {
            int lowestPriority = 999;
            int index = -1;

            for (int i = 0; i<tokens.Count; i++) // find lowest priority operator. if a tie, take the right-most one
            {
                if (tokens [i].Type == TokenType.Operator)
                {
                    AnnotatedString oper = tokens [i].AnnotatedText;
                    int priority = TokenUtils.GetBinOpPriority (oper.Raw);

                    // two operators, equals and exponent, are evaluated right-to-left. e.g.: 2 ^ 3 ^ 4 evaluated as: 2 ^ (3 ^ 4)
                    // This priority adjustment increases priority as we move to the right so that happens

                    if (oper [0].IsEqualSign)
                        priority += i;

                    if (oper [0].IsExponent)
                        priority += i;

                    if (lowestPriority >= priority)
                    {
                        lowestPriority = priority;
                        index = i;
                    }
                }
            }

            if (index == -1) // no operator found, build and print error message
            {
                string str = "";

                foreach (Token t in tokens)
                    str += t.AnnotatedText.Raw + " ";

                throw new Exception ("Error: No operators found in expression: " + str);
            }

            Operator = tokens [index].AnnotatedText.Raw;
            NodeType = tokens [index].Type;

            List<IToken> left  = new List<IToken> ();
            List<IToken> right = new List<IToken> ();

            for (int i = 0; i<tokens.Count; i++)
            {
                if (i < index) left.Add (tokens [i]);
                if (i > index) right.Add (tokens [i]);
            }

            Operands.Add (new ExpressionTreeNode (left));
            Operands.Add (new ExpressionTreeNode (right));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_GroupingParens (List<IToken> tokens, IWorkspace workspace)
        {
            //
            // remove leading and trailing parens
            //
            int index = tokens [0].text.IndexOf ('(');
            if (index == -1) throw new Exception ("Syntax error, missing opening paren");
            string expr = tokens [0].text.Remove (0, index + 1);
            index = expr.LastIndexOf (')');
            if (index == -1) throw new Exception ("Syntax error, missing closing paren");
            expr = expr.Remove (index);

            //
            // parse what's left
            //
            TokenParsing parser = new TokenParsing ();
            ConstructorCommon (parser.StringToTokens (expr, workspace));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // passed a single token of the form: [xxxxxx]

        void BuildNodeFrom_Brackets (List<IToken> tokens, IWorkspace workspace)
        {
            TokenParsing parser = new TokenParsing ();
         //   List<string> args = new List<string> ();

            NodeType = tokens [0].Type;

            switch (NodeType)
            {
                case TokenType.BracketsColon:
                {
                    Operator = "RowVectorIterator";
                    List<string> args = parser.SplitBracketArgs_Colon (tokens [0].text);
                    foreach (string str in args)
                        Operands.Add (new ExpressionTreeNode (str));// workspace);//);
                }
                break;

                case TokenType.BracketsComma:
                {
                    Operator = "RowVectorElements";
                    List<string> args = parser.SplitBracketArgs_Comma (tokens [0].text);
                    foreach (string str in args)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                case TokenType.BracketsSemi:
                {
                    Operator = "ColVectorElements";
                    List<string> args = parser.SplitBracketArgs_Semi (tokens [0].text);

                    foreach (string str in args)
                    {
                        string editted = EnsureRowVector (str);
                        Operands.Add (new ExpressionTreeNode (editted, workspace));
                    }
                }
                break;

                case TokenType.Brackets:
                case TokenType.BracketsSpace:
                {
                    Operator = "RowVectorElements";
                    List<string> args = parser.SplitBracketArgs_Space (tokens [0].text);
                    foreach (string str in args)
                        if (str.Length > 0)                                 //   <==========================================================
                            Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                default: throw new Exception ("Unrecognized bracket syntax: " + tokens [0].text);
            }
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // 

        string EnsureRowVector (string orig)
        {
            if (orig [0] != '[')
                return "[" + orig.Trim () + "]";

            return orig.Trim ();

            //TokenParsing parser = new TokenParsing ();
            //List<Token> toks = parser.ParsingPassOne (orig);
            //toks = parser.ParsingPassTwo (toks, workspace);

            //foreach (Token tok in toks)
            //    if (tok.type == TokenType.ArithmeticOperator)
            //        return orig;

            //string editted = "";

            //for (int i = 0; i<toks.Count; i++)
            //{
            //    editted += toks [i].text + " ";

            //    if (i<toks.Count-1)
            //        editted += ", ";
            //}

            //return editted;
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_Numeric (List<IToken> tokens, IWorkspace workspace)
        {
            bool valid = double.TryParse (tokens [0].text, out double scalar);

            if (valid)
            {
                NodeType = TokenType.Numeric;
                Value = new PLDouble (scalar);
            }
            else
               throw new Exception (string.Format ("Numeric token {0} has invalid value", tokens [0].text));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_VariableName (List<IToken> tokens, IWorkspace workspace)
        {
            Operator = tokens [0].Text;
            NodeType = tokens [0].Type;

            if (tokens.Count > 1)
                for (int i = 1; i<tokens.Count; i++)
                    Operands.Add (new ExpressionTreeNode (tokens [i].text, workspace));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_String (List<IToken> tokens)
        {
            NodeType = tokens [0].type;
            Value = new PLString (tokens [0].text);
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // an operator with no operands

        void BuildNodeFrom_Operator (List<IToken> tokens)
        {
            Operator = tokens [0].text;
            NodeType = tokens [0].type;
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_FunctionPair (IToken tokens)
        {
            Operator = tokens.t0.text;
            NodeType = tokens.t0.type;

            TokenParsing parser = new TokenParsing ();

            switch (tokens.t1.type)
            {
                case TokenType.GroupingParens:
                case TokenType.FunctionParens:
                {
                    List<string> args = parser.SplitFunctionArgs (tokens.t1.text);

                    foreach (string str in args)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                case TokenType.VariableName:
                {
                    Operands.Add (new ExpressionTreeNode (tokens.t1.text, workspace));
                }
                break;

                case TokenType.Pair:
                {
                    List<Token> tok = new List<Token> () { tokens.t1 };
                    Operands.Add (new ExpressionTreeNode (tok, workspace));
                }
                break;
                
                case TokenType.BracketsComma:
                {
                    List<string> tok = parser.SplitBracketArgs_Comma (tokens.t1.text);

                    foreach (string str in tok)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                case TokenType.BracketsColon:
                {
                    List<string> tok = parser.SplitBracketArgs_Colon (tokens.t1.text);

                    foreach (string str in tok)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                case TokenType.BracketsSemi:
                {
                    List<string> tok = parser.SplitBracketArgs_Semi (tokens.t1.text);

                    foreach (string str in tok)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                case TokenType.BracketsSpace:
                {
                    List<string> tok = parser.SplitBracketArgs_Space (tokens.t1.text);

                    foreach (string str in tok)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
                }
                break;

                //case TokenType.String:
                //{

                //}
                //break;

                default: throw new Exception ("BuildNodeFrom_FunctionPair, Unsupported token type " + tokens.t1.type);
            }




        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_SubmatrixPair (TokenPair tokens)
        {
            Operator = tokens.t0.text;
            NodeType = tokens.t0.type;

            TokenParsing parser = new TokenParsing ();
            List<string> args = parser.SplitSubmatrixArgs (tokens.t1.text);

            foreach (string str in args)
            {
                Operands.Add (new ExpressionTreeNode (str, workspace));
            }

            // search the operand tree for "end". replace any with appropriate number
            // of rows or colums

            Compact ();

            string matrixName = Operator;

            int rows;
            int cols;

            if (workspace.Get (matrixName) is PLRMatrix)
            {
                PLRMatrix mat = workspace.Get (matrixName) as PLRMatrix;
                rows = mat.Rows; // (workspace.Rows (mat) as PLInteger).Data;
                cols = mat.Cols; // (workspace.Cols (mat) as PLInteger).Data;
            }

            else if (workspace.Get (matrixName) is PLCMatrix)
            {
                PLCMatrix mat = workspace.Get (matrixName) as PLCMatrix;
                rows = mat.Rows;
                cols = mat.Cols;
            }

            else
                throw new Exception ("Unrecognized matrix type");

            bool IsRowVector = rows == 1 && cols > 1;
            bool IsColVector = rows > 1  && cols == 1;

            for (int i = 0; i<Operands.Count; i++)
            {
                for (int j = 0; j<Operands [i].Operands.Count; j++)
                {
                    if (Operands [i].Operands [j].Operator == "end")
                    {
                        if (IsRowVector)
                        {
                            Operands [i].Operands [j] = new ExpressionTreeNode (cols.ToString (), workspace);
                        }

                        else if (IsColVector)
                        { 
                            Operands [i].Operands [j] = new ExpressionTreeNode (rows.ToString (), workspace);
                        }

                        else
                        {
                            if (i == 0)
                            {
                                Operands [i].Operands [j] = new ExpressionTreeNode (rows.ToString (), workspace);
                            }

                            else
                            {
                                Operands [i].Operands [j] = new ExpressionTreeNode (cols.ToString (), workspace);
                            }
                        }
                    }
                }
            }
        }
    }
}
