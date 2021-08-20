using System;
using System.Collections.Generic;

using PLCommon;
using PLWorkspace;
using PLLibrary;

namespace PLKernel
{
    public partial class TokenParsing
    {
        public List<Token> ParsingPassTwo (List<Token> initial, Workspace workspace) // public just for unit test
        {
            List<Token> editted = LookupAlphanumerics (initial, workspace);
            
            editted = IdentifyParens (editted, workspace); // grouping, function args, sub matrix

            editted = IdentifyBrackets (editted); // by separator: :, ;, etc.

            editted = ReplaceTransposeOps (editted); // A' => Transpose (A)

            editted = LookForDecimals (editted); // combine decimal point with subsequent numeric. DO BEFORE UNARY

            editted = BindUnaryOperators (editted); // -, A => (-1 * A), -, 7 => -7

            editted = LookForTwoCharOperators (editted); // &, & => &&

            return editted;
        }

        //*************************************************************************************************

        List<Token> LookupAlphanumerics (List<Token> initial, Workspace workspace) 
        {
            for (int i=0; i<initial.Count; i++)
            {
                if (initial [i].type == TokenType.Alphanumeric)
                {
                    SymbolicNameTypes ty = workspace.WhatIs (initial [i].text);

                    if (ty == SymbolicNameTypes.Unknown)
                        ty = LibraryManager.WhatIs (initial [i].text); 

                    if (ty == SymbolicNameTypes.Function) 
                        initial [i].type = TokenType.FunctionName;

                    else if (ty == SymbolicNameTypes.Variable) 
                        initial [i].type = TokenType.VariableName;

                    else if (ty == SymbolicNameTypes.FunctionFile) 
                        initial [i].type = TokenType.FunctionFile; 

                    else initial [i].type = TokenType.Undefined;
                }
            }

            return initial;
        }

        //*************************************************************************************************

        List<Token> IdentifyParens (List<Token> tokens, Workspace final)
        {
            List<Token> editted = new List<Token> ();

            for (int i=0; i<tokens.Count; i++)
            {
                if (tokens [i].type == TokenType.Parens)
                {
                    Token tok = new Token (TokenType.GroupingParens, tokens [i].text);
                    editted.Add (tok);
                }

                else if (tokens [i].type == TokenType.FunctionName || tokens [i].type == TokenType.FunctionFile)
                {
                    if (i + 1 == tokens.Count)
                    {
                        Token tok = new Token (TokenType.FunctionName, tokens [i].text);
                        editted.Add (tok);
                        i++;
                        
                        //throw new Exception ("Function " + tokens [i].text + " missing args");
                    }

                    else
                    {
                        TokenPair tp = new TokenPair ();
                        tp.pairType = TokenPairType.Function;
                        tp.t0 = tokens [i];
                        tp.t1 = tokens [i+1];
                        tp.t1.type = TokenType.FunctionParens;
                        tp.text = tp.t0.text + tp.t1.text;
                        editted.Add (tp);
                        i++;
                    }
                }

                else if (tokens [i].type == TokenType.VariableName || tokens [i].type == TokenType.Undefined)
                {
                    if (i + 1 < tokens.Count && tokens [i + 1].type == TokenType.Parens)
                    {
                        TokenPair tp = new TokenPair ();
                        tp.pairType = TokenPairType.Submatrix;
                        tp.t0 = tokens [i];
                        tp.t1 = tokens [i+1];
                        tp.t1.type = TokenType.SubmatrixParens;
                        tp.text = tp.t0.text + tp.t1.text;
                        editted.Add (tp);
                        i++;
                    }
                    else                
                        editted.Add (tokens [i]);
                }

                else                
                    editted.Add (tokens [i]);
            }

            return editted;
        }

        //*************************************************************************************************

        // for any Bracket tokens, identify top-level (i.e. no paren or bracket nesting) separator

        List<Token> IdentifyBrackets (List<Token> initial)
        {
            for (int i=0; i<initial.Count; i++)
            {
                if (initial [i].type == TokenType.Brackets)
                {
                    int parenNesting = 0, bracketNesting = 0;
                    int i1 = initial [i].text.IndexOf ('[');
                    int i2 = initial [i].text.LastIndexOf (']');

                    string textCopy = initial [i].text.Substring (i1 + 1, i2 - i1 - 1); // w/o outer brackets
                    List<char> separatorsFound = new List<char> ();

                    for (int j = 0; j<textCopy.Length; j++)
                    {
                        if (textCopy [j] == '(') parenNesting++;
                        if (textCopy [j] == '[') bracketNesting++;

                        if (textCopy [j] == ')') {parenNesting--;   if (parenNesting < 0)   throw new Exception ("Parenthesis error: " + initial [i].text); }
                        if (textCopy [j] == ']') {bracketNesting--; if (bracketNesting < 0) throw new Exception ("Bracket error: " + initial [i].text); }

                        if (parenNesting == 0 && bracketNesting == 0)
                        {
                            if (TokenUtils.bracketSeparators.Contains (textCopy [j]))
                            {
                                if (separatorsFound.Contains (textCopy [j]) == false)
                                    separatorsFound.Add (textCopy [j]);
                            }
                        }
                    }

                    // determine lowest priority separator
                    TokenUtils.BracketSeparatorPriority lowestBSP = new TokenUtils.BracketSeparatorPriority (' ', 99999, TokenType.Brackets);

                    foreach (char c in separatorsFound)
                    {
                        TokenUtils.BracketSeparatorPriority bsp = TokenUtils.GetBspForOperator (c);
                        if (lowestBSP.priority > bsp.priority) lowestBSP = bsp;
                    }

                    initial [i].type = lowestBSP.tokenType;
                }
            }

            return initial;
        }

        //*************************************************************************************************

        // replace transpose operator by function call

        List<Token> ReplaceTransposeOps (List<Token> initial)
        {
            for (int i = 0; i<initial.Count;)
            {
                if (initial [i].type == TokenType.TransposeOperator)
                {
                    TokenPair tp = new TokenPair ();
                    tp.pairType = TokenPairType.Function;

                    tp.t0 = new Token (TokenType.FunctionName, "Transpose");
                    tp.t1 = new Token (TokenType.FunctionParens, "(" + initial [i - 1].text + ")");
                    tp.text = tp.t0.text + tp.t1.text;

                    initial [i - 1] = tp;
                    initial.RemoveAt (i);
                }

                else
                    i++;
            }

            return initial;
        }

        //List<Token> ReplaceTransposeOps (List<Token> initial)
        //{
        //    for (int i = 0; i<initial.Count;)
        //    {
        //        if (initial [i].type == TokenType.TransposeOperator)
        //        {
        //            TokenPair tp = new TokenPair ();
        //            tp.pairType = TokenPairType.Function;

        //            tp.t0 = new Token (TokenType.FunctionName, "Transpose");
        //            tp.t1 = initial [i - 1];
        //            initial [i - 1] = tp;
        //            initial.RemoveAt (i);
        //        }

        //        else
        //            i++;
        //    }

        //    return initial;
        //}

        //*************************************************************************************************

        // operators

        List<Token> BindUnaryOperators (List<Token> initial)
        {
            // Look for consecutive operands ending in a unary op

            for (int i = initial.Count-1; i>0; i--)
            {
                try
                {
                    TokenType im2 = i >= 2 ? initial [i-2].type : TokenType.ArithmeticOperator;
                    TokenType im1 = initial [i-1].type;

                    if (im2 == TokenType.ArithmeticOperator && im1 == TokenType.ArithmeticOperator)
                    {
                        if (TokenUtils.IsUnaryOp (initial [i-1].text))
                        {
                            switch (initial [i-1].text [0])
                            {
                                case '+':
                                    initial.RemoveAt (i-1);
                                    break;

                                case '-':
                                {
                                    string op = "";
                                    TokenType ty = initial [i].type;

                                    switch (ty)
                                    {
                                        case TokenType.VariableName:
                                        case TokenType.Alphanumeric:
                                        case TokenType.GroupingParens:
                                        {
                                            Token tok = new Token (TokenType.GroupingParens, "(-1 * " + initial [i].text + ")");
                                            initial [i-1] = tok;
                                            initial.RemoveAt (i);
                                        }
                                        break;

                                        case TokenType.Numeric:
                                        {
                                            if (initial [i].text [0] == '-') op = initial [i].text.Remove (0, 1);
                                            else if (initial [i].text [0] == '+') {op = initial [i].text.Remove (0, 1); op = "-" + op;}
                                            else op = "-" + initial [i].text;

                                            Token tok = new Token (TokenType.Numeric, op);
                                            initial [i-1] = tok;
                                            initial.RemoveAt (i);
                                        }
                                        break;

                                        case TokenType.FunctionFile:
                                        case TokenType.FunctionName:
                                        case TokenType.Pair:
                                        {
                                            initial.RemoveAt (i-1);
                                            initial.Insert (i-1, new Token (TokenType.Numeric, "-1"));
                                            initial.Insert (i, new Token (TokenType.ArithmeticOperator, "*"));
                                        }
                                        break;

                                        default:
                                            throw new Exception ("Unary minus error, type = " + ty.ToString ());
                                    }
                                }
                                break;

                                case '~':
                                {
                                    TokenPair tok = new TokenPair (); // TokenType.GroupingParens, op);
                                    tok.pairType = TokenPairType.Function;

                                    tok.t0 = new Token (TokenType.FunctionName, "Not");
                                    tok.t1 = initial [i];
                                    //tok.t1 = new Token (TokenType.FunctionParens, "(" + initial [i].text + ")");

                                    initial [i-1] = tok;
                                    initial.RemoveAt (i);
                                }
                                break;
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw new Exception ("Error binding unary ops: " + ex.Message); // initial [i+1].text);
                }
            }

            return initial;
        }

        //*************************************************************************************************

        // combine consecutive operator characters into single 2-char operator

        List<Token> LookForTwoCharOperators (List<Token> initial)
        {
            List<Token> final = new List<Token> ();

            for (int i=0; i<initial.Count-1; i++)
            {
                TokenType t0 = initial [i].type;
                TokenType t1 = initial [i+1].type;

                if ((t0 == TokenType.ArithmeticOperator || t0 == TokenType.Decimal) && t1 == TokenType.ArithmeticOperator)
                {
                    string op = initial [i].text + initial [i+1].text;
                    
                    if (TokenUtils.IsTwoCharOperator (op) == true)
                    {
                        final.Add (new Token (TokenType.ArithmeticOperator, op));
                        i++;
                    }
                    else
                        throw new Exception ("Illegal operator: " + op);
                }
                else
                    final.Add (initial [i]);
            }

            final.Add (initial [initial.Count - 1]);
            return final;
        }

        //*************************************************************************************************

        // combine isolated decimal points with following numeric

        List<Token> LookForDecimals (List<Token> initial)
        {
            List<Token> final = new List<Token> ();

            int lastAdded = -1;

            for (int i=0; i<initial.Count-1; i++)
            {
                TokenType t0 = initial [i].type;
                TokenType t1 = initial [i+1].type;

                if (t0 == TokenType.Decimal && t1 == TokenType.Numeric)
                {
                    string op = initial [i].text + initial [i+1].text;
                    lastAdded = i + 1;
                    
                    final.Add (new Token (TokenType.Numeric, op));
                    i++;
                }
                else
                    final.Add (initial [i]);
            }

            // see if the final token has already been moved
            if (lastAdded != initial.Count - 1)
                final.Add (initial [initial.Count - 1]); 

            return final;
        }
    }
}





/******************************


namespace PLKernel
{
    public partial class TokenParsing
    {


*************************/
















