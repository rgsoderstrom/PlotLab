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

            Console.WriteLine ("BuildNodeFrom_List of tokens:");
            foreach (IToken it in tokens)
                Console.WriteLine (it.ToString ());
            Console.WriteLine ();

            int lowestPriority = 999;
            int index = -1;

            for (int i = 0; i<tokens.Count; i++) // find lowest priority operator. if a tie, take the right-most one
            {
                if (tokens [i].Type == TokenType.Operator)
                {
                    AnnotatedString oper = tokens [i].AnnotatedText;
                    int priority = TokenUtils.GetBinOpPriority (oper.Plain);

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
                    str += t.AnnotatedText.Plain + " ";

                throw new Exception ("Error: No operators found in expression: " + str);
            }

            Operator = tokens [index].AnnotatedText.Plain;
            NodeType = tokens [index].Type;

            Console.WriteLine ("Operator: " + Operator + " Type: " + NodeType);

            List<IToken> left  = new List<IToken> ();
            List<IToken> right = new List<IToken> ();

            for (int i = 0; i<tokens.Count; i++)
            {
                if (i < index) left.Add (tokens [i]);
                if (i > index) right.Add (tokens [i]);
            }


            Console.WriteLine ("left operand:");
            foreach (IToken it in left)
                Console.WriteLine (it.ToString ());
            Console.WriteLine ();

            Console.WriteLine ("right operand:");
            foreach (IToken it in right)
                Console.WriteLine (it.ToString ());
            Console.WriteLine ();

            Console.WriteLine ("-------------------------------");


            Operands.Add (new ExpressionTreeNode (left));
            Operands.Add (new ExpressionTreeNode (right));
        
            Console.WriteLine ("leaving BuildNodeFrom_List, " + tokens.Count + " tokens, " + Operands.Count + " operands");    
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_GroupingParens (List<IToken> tokens, IWorkspace workspace)
        {
            //
            // remove leading and trailing parens
            //
            
            //tokens [0].StripOuter ();
            AnnotatedString as1 = tokens [0].AnnotatedText;
            AnnotatedString as2 = as1.RemoveWrapper ();

            //
            // parse what's left
            //
            TokenParsing parser = new TokenParsing ();
            ConstructorCommon (parser.StringToTokens (as2, Workspace, Library, FileSystem));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // passed a single token of the form: [xxxxxx]

        void BuildNodeFrom_Brackets (List<IToken> tokens, IWorkspace workspace)
        {
            TokenParsing parser = new TokenParsing ();

            NodeType = tokens [0].Type;

            switch (NodeType)
            {
                case TokenType.BracketsColon:
                {
                    Operator = "RowVectorIterator";
                    List<AnnotatedString> args = parser.SplitBracketArgs_Colon (tokens [0].AnnotatedText);
                    foreach (AnnotatedString str in args)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                case TokenType.BracketsComma:
                {
                    Operator = "RowVectorElements";
                    List<AnnotatedString> args = parser.SplitBracketArgs_Comma (tokens [0].AnnotatedText);
                    foreach (AnnotatedString str in args)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                case TokenType.BracketsSemi:
                {
                    Operator = "ColVectorElements";
                    List<AnnotatedString> args = parser.SplitBracketArgs_Semi (tokens [0].AnnotatedText);

                    foreach (AnnotatedString str in args)
                    {
                      //AnnotatedString editted = EnsureRowVector (str);
                        Operands.Add (new ExpressionTreeNode (str));//(editted));
                    }
                }
                break;

                case TokenType.Brackets:
                case TokenType.BracketsSpace:
                {
                    Operator = "RowVectorElements";
                    List<AnnotatedString> args = parser.SplitBracketArgs_Space (tokens [0].AnnotatedText);

                    foreach (AnnotatedString str in args)
                        if (str.Count > 0)                                 //   <==========================================================
                            Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                default: throw new Exception ("Unrecognized bracket syntax: " + tokens [0].AnnotatedText.Plain);
            }
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // 

        //AnnotatedString EnsureRowVector (AnnotatedString orig)
        //{
        //    if (orig [0] != '[')
        //        return "[" + orig.Trim () + "]";

        //    return orig.Trim ();

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
        //}

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_Numeric (List<IToken> tokens)
        {
            bool valid = double.TryParse (tokens [0].AnnotatedText.Plain, out double scalar);

            if (valid)
            {
                NodeType = TokenType.Numeric;
                Value = new PLDouble (scalar);
            }
            else
               throw new Exception (string.Format ("Numeric token {0} has invalid value", tokens [0].AnnotatedText.Plain));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_VariableName (List<IToken> tokens, IWorkspace workspace)
        {
            Operator = tokens [0].AnnotatedText.Plain;
            NodeType = tokens [0].Type;

            if (tokens.Count > 1)
                for (int i = 1; i<tokens.Count; i++)
                    Operands.Add (new ExpressionTreeNode (tokens [i].AnnotatedText));
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_String (List<IToken> tokens)
        {
            NodeType = tokens [0].Type;
            Value = new PLString (tokens [0].AnnotatedText.Plain);
        }

        //*************************************************************************************************
        //*************************************************************************************************

        // an operator with no operands

        void BuildNodeFrom_Operator (List<IToken> tokens)
        {
            Operator = tokens [0].AnnotatedText.Plain;
            NodeType = tokens [0].Type;
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_FunctionPair (IToken tokens)
        {
            if (tokens is TokenPair == false)
                throw new Exception ("BuildNodeFrom_FunctionPair must be passed a token pair. Got " + tokens.AnnotatedText.Plain);

            TokenParsing parser = new TokenParsing ();
            TokenPair Pair = tokens as TokenPair;

            Operator = Pair.Get1.AnnotatedText.Plain;
            NodeType = Pair.Get1.Type;

            switch (Pair.Get2.Type)
            {
                case TokenType.GroupingParens:
                case TokenType.FunctionParens:
                {
                    List<AnnotatedString> args = parser.SplitFunctionArgs (Pair.Get2.AnnotatedText);

                    foreach (AnnotatedString str in args)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                case TokenType.VariableName:
                {
                    Operands.Add (new ExpressionTreeNode (Pair.Get2.AnnotatedText));
                }
                break;

                case TokenType.Pair:
                {
                    throw new Exception ("BuildNodeFrom_FunctionPair, TokenType.Pair not implemented");
                    //List<Token> tok = new List<Token> () { tokens.t1 };
                    //Operands.Add (new ExpressionTreeNode (tok));
                }
                //break;
                
                case TokenType.BracketsComma:
                {
                    List<AnnotatedString> tok = parser.SplitBracketArgs_Comma (Pair.Get2.AnnotatedText);

                    foreach (AnnotatedString str in tok)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                case TokenType.BracketsColon:
                {
                    List<AnnotatedString> tok = parser.SplitBracketArgs_Colon (Pair.Get2.AnnotatedText);

                    foreach (AnnotatedString str in tok)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                case TokenType.BracketsSemi:
                {
                    List<AnnotatedString> tok = parser.SplitBracketArgs_Semi (Pair.Get2.AnnotatedText);

                    foreach (AnnotatedString str in tok)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                case TokenType.BracketsSpace:
                {
                    List<AnnotatedString> tok = parser.SplitBracketArgs_Space (Pair.Get2.AnnotatedText);

                    foreach (AnnotatedString str in tok)
                        Operands.Add (new ExpressionTreeNode (str));
                }
                break;

                //case TokenType.String:
                //{

                //}
                //break;

                default: throw new Exception ("BuildNodeFrom_FunctionPair, Unsupported token type " + Pair.Get2.Type);
            }
        }

        //*************************************************************************************************
        //*************************************************************************************************

        void BuildNodeFrom_SubmatrixPair (TokenPair tokens)
        {
            throw new Exception ("BuildNodeFrom_SubmatrixPair not implemented");

            //if (tokens is TokenPair == false)
            //    throw new Exception ("BuildNodeFrom_SubmatrixPair must be passed a token pair. Got " + tokens.AnnotatedText.Plain);

            //TokenPair Pair = tokens as TokenPair;

            //Operator = Pair.Get1.AnnotatedText.Plain;
            //NodeType = Pair.Get1.Type;

            //List<AnnotatedString> args = TokenParsing.SplitSubmatrixArgs (Pair.Get2.AnnotatedText);

            //foreach (AnnotatedString str in args)
            //{
            //    Operands.Add (new ExpressionTreeNode (str));
            //}

            //// search the operand tree for "end". replace any with appropriate number
            //// of rows or colums

            //Compact ();

            //string matrixName = Operator;

            //int rows;
            //int cols;

            //if (Workspace.Get (matrixName) is PLRMatrix)
            //{
            //    PLRMatrix mat = Workspace.Get (matrixName) as PLRMatrix;
            //    rows = mat.Rows; // (workspace.Rows (mat) as PLInteger).Data;
            //    cols = mat.Cols; // (workspace.Cols (mat) as PLInteger).Data;
            //}

            //else if (workspace.Get (matrixName) is PLCMatrix)
            //{
            //    PLCMatrix mat = workspace.Get (matrixName) as PLCMatrix;
            //    rows = mat.Rows;
            //    cols = mat.Cols;
            //}

            //else
            //    throw new Exception ("Unrecognized matrix type");

            //bool IsRowVector = rows == 1 && cols > 1;
            //bool IsColVector = rows > 1  && cols == 1;

            //for (int i = 0; i<Operands.Count; i++)
            //{
            //    for (int j = 0; j<Operands [i].Operands.Count; j++)
            //    {
            //        if (Operands [i].Operands [j].Operator == "end")
            //        {
            //            if (IsRowVector)
            //            {
            //                Operands [i].Operands [j] = new ExpressionTreeNode (cols.ToString (), workspace);
            //            }

            //            else if (IsColVector)
            //            { 
            //                Operands [i].Operands [j] = new ExpressionTreeNode (rows.ToString (), workspace);
            //            }

            //            else
            //            {
            //                if (i == 0)
            //                {
            //                    Operands [i].Operands [j] = new ExpressionTreeNode (rows.ToString (), workspace);
            //                }

            //                else
            //                {
            //                    Operands [i].Operands [j] = new ExpressionTreeNode (cols.ToString (), workspace);
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}
