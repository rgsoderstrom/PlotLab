using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;

namespace PLKernel
{
    internal partial class ExpressionTreeNode
    {
        //*************************************************************************************************
        //*************************************************************************************************

        internal void BuildNodeFrom_List (List<Token> tokens)
        {
            int lowestPriority = 999;
            int index = -1;

            for (int i = 0; i<tokens.Count; i++) // find lowest priority operator. if a tie, take the right-most one
            {
                if (tokens [i].type == TokenType.ArithmeticOperator)
                {
                    string oper = tokens [i].text;
                    int priority = TokenUtils.GetBinOpPriority (oper);

                    // two operators, equals and exponent, are evaluated right-to-left. e.g.: 2 ^ 3 ^ 4 = 2 ^ (3 ^ 4)
                    // This priority adjustment increases priority as we move to the right so that happens

                    if (TokenUtils.IsEqualSign (oper))
                        priority += i;

                    if (TokenUtils.IsExponent (oper))
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
                    str += t.text + " ";

                throw new Exception ("Error: No operators found in expression: " + str);
            }

            Operator = tokens [index].text;
            NodeType = tokens [index].type;

            List<Token> left = new List<Token> ();
            List<Token> right = new List<Token> ();

            for (int i = 0; i<tokens.Count; i++)
            {
                if (i < index) left.Add (tokens [i]);
                if (i > index) right.Add (tokens [i]);
            }

            Operands.Add (new ExpressionTreeNode (left, workspace));
            Operands.Add (new ExpressionTreeNode (right, workspace));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_GroupingParens (List<Token> tokens, Workspace workspace)
        {
            //
            // remove leading and trailing parens
            //
            int index = tokens [0].text.IndexOf ('(');
            if (index == -1) throw new Exception ("Syntax error");
            string expr = tokens [0].text.Remove (0, index + 1);
            index = expr.LastIndexOf (')');
            if (index == -1) throw new Exception ("Syntax error");
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

        void BuildNodeFrom_Brackets (List<Token> tokens, Workspace workspace)
        {
            TokenParsing parser = new TokenParsing ();
         //   List<string> args = new List<string> ();

            NodeType = tokens [0].type;

            switch (NodeType)
            {
                case TokenType.BracketsColon:
                {
                    Operator = "RowVectorIterator";
                    List<string> args = parser.SplitBracketArgs_Colon (tokens [0].text);
                    foreach (string str in args)
                        Operands.Add (new ExpressionTreeNode (str, workspace));
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

        void BuildNodeFrom_Numeric (List<Token> tokens, Workspace workspace)
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

        void BuildNodeFrom_VariableName (List<Token> tokens, Workspace workspace)
        {
            Operator = tokens [0].text;
            NodeType = tokens [0].type;

            if (tokens.Count > 1)
                for (int i = 1; i<tokens.Count; i++)
                    Operands.Add (new ExpressionTreeNode (tokens [i].text, workspace));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_String (List<Token> tokens)
        {
            NodeType = tokens [0].type;
            Value = new PLString (tokens [0].text);
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // an operator with no operands

        void BuildNodeFrom_Operator (List<Token> tokens)
        {
            Operator = tokens [0].text;
            NodeType = tokens [0].type;
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_FunctionPair (TokenPair tokens)
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
                Operands.Add (new ExpressionTreeNode (str, workspace));
        }



    }
}
