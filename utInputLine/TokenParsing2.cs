using System;
using System.Collections.Generic;   

////using PLCommon;
////using PLWorkspace;
////using PLLibrary;

namespace Main
{
    public partial class TokenParsing
    {
        internal List<Token> ParsingPassTwo (List<Token> initial, IWorkspace workspace, ILibrary library, IFileSystem files)
        {
            List<Token> edited = LookupAlphanumerics (initial, workspace, library, files);

            edited = IdentifyParens (edited, workspace); // grouping, function args, sub matrix

            edited = IdentifyBrackets (edited); // by separator: :, ;, etc.

            edited = ReplaceTransposeOps (edited); // A' => Transpose (A)

            edited = BindUnaryOperators (edited); // -, A => (-1 * A), -, 7 => -7

            //            edited = LookForTwoCharOperators (edited); // &, & => &&

            return edited;
        }

        //*************************************************************************************************

        List<Token> LookupAlphanumerics (List<Token> initial,
                                         IWorkspace workspace,
                                         ILibrary library,
                                         IFileSystem files)
        {
            for (int i = 0; i<initial.Count; i++)
            {
                if (initial [i].type == TokenType.Alphanumeric)
                {
                    if (workspace.IsDefined (initial [i].annotatedText.Raw))
                        initial [i].type = TokenType.VariableName;

                    else if (library.IsDefined (initial [i].annotatedText.Raw))
                        initial [i].type = TokenType.FunctionName;

                    else if (files.IsFunctionFile (initial [i].annotatedText.Raw))
                        initial [i].type = TokenType.FunctionName;

                    else if (files.IsScriptFile (initial [i].annotatedText.Raw))
                        initial [i].type = TokenType.ScriptFile;

                    else initial [i].type = TokenType.Undefined;
                }
            }

            return initial;
        }

        //*************************************************************************************************

        // Identify parenthesis as:
        //    GroupingParens,  // A * (B + C)
        //    FunctionParens,  // Func1 (P, Q, R, S)
        //    SubmatrixParens, // ZMat (Rs, Cs); % (row select, col select)

        List<Token> IdentifyParens (List<Token> tokens, IWorkspace final)
        {
            List<Token> edited = new List<Token> ();

            for (int i = 0; i<tokens.Count; i++)
            {
                if (tokens [i].type == TokenType.Parens)
                {
                    if (edited.Count == 0) 
                    {
                        Token tok = new Token (TokenType.GroupingParens, tokens [i].annotatedText);
                        edited.Add (tok);
                    }
                    else // look at previous type to see what type these parens are
                    {
                        switch (edited [edited.Count-1].type)
                        {
                            case TokenType.VariableName:
                                Token tok1 = new Token (TokenType.SubmatrixParens, tokens [i].annotatedText);
                                edited.Add (tok1);
                                break;

                            case TokenType.FunctionName:
                                Token tok2 = new Token (TokenType.FunctionParens, tokens [i].annotatedText);
                                edited.Add (tok2);
                                break;

                            default:
                                Token tok3 = new Token (TokenType.GroupingParens, tokens [i].annotatedText);
                                edited.Add (tok3);
                                break;
                        }
                    }
                }

                else
                    edited.Add (tokens [i]);
            }

            return edited;
        }

        //*************************************************************************************************

        // Identify brackets as:
        //      BracketsColon,  // [A : B : C] or [A : B]
        //      BracketsSemi,   // [a ; b ; c ; d] or [1 2 3 ; 4 5 6]
        //      BracketsComma,  // [1, 2, 3]
        //      BracketsSpace,  // [1 2 3]

        // for any Bracket tokens, identify top-level (i.e. same nesting level as opening bracket) separator

        List<Token> IdentifyBrackets (List<Token> initial)
        {
            const char NoneFound = '?'; // no separators found

            for (int i = 0; i<initial.Count; i++)
            {
                if (initial [i].type == TokenType.Brackets)
                {
                    AnnotatedString tokenText = initial [i].annotatedText;
                    char separatorFound = NoneFound;

                    int initalNesting = tokenText [0].NestingLevel;

                    for (int j = 0; j<tokenText.Count; j++)
                    {
                        AnnotatedChar tokenChar = tokenText [j];

                        if (tokenChar.NestingLevel == initalNesting)
                        { 
                            if (TokenUtils.bracketSeparators.Contains (tokenChar.Character))
                            { 
                                if (separatorFound == NoneFound)
                                    separatorFound = tokenChar.Character;

                                else if (separatorFound != tokenChar.Character)
                                    throw new Exception ("Bracket separator error");
                            }
                        }
                    }

                    switch (separatorFound)
                    {
                        case ':':
                            initial [i].type = TokenType.BracketsColon;
                            break;

                        case ';':
                            initial [i].type = TokenType.BracketsSemi;
                            break;

                        case ',':
                            initial [i].type = TokenType.BracketsComma;
                            break;

             //           case ' ':
               //             break;

                        case NoneFound:
                            initial [i].type = TokenType.BracketsSpace;
                            break;

                        default:
                            throw new Exception ("Error looking for bracket separators");

                    }
                }
            }

            return initial;
        }


        //*************************************************************************************************

        // replace transpose operator by function call

        private bool IsTransposeToken (Token tok) {return tok.type == TokenType.Transpose;}

        private List<Token> ReplaceTransposeOps (List<Token> initial)
        {
            List<int> transposeIndices = new List<int> ();

            int start = 0;

            while (start < initial.Count)
            {
                int index = initial.FindIndex (start, IsTransposeToken);

                if (index == -1)
                    break;

                transposeIndices.Add (index);
                start = index + 1;
            }

            // if none found, just return original list
            if (transposeIndices.Count == 0)
                return initial;  

            List<Token> edited = new List<Token> ();
            int get = 0;

            foreach (int index in transposeIndices)
            {
                while (get < index - 1)
                    edited.Add (initial [get++]);

                edited.Add (new Token (TokenType.FunctionName, new AnnotatedString ("transpose"))); // NESTING LEVELS NEEDED?

                // add parens unless outer level is already parens                
                if (initial [get].type != TokenType.GroupingParens) edited.Add (new Token (TokenType.FunctionParens, initial [get].annotatedText.AddOuterParens ()));
                else                                                edited.Add (new Token (TokenType.FunctionParens, initial [get].annotatedText));

                get += 2;
            }

            // move tokens after last transpose
            while (get < initial.Count)
                edited.Add (initial [get++]);

        //  return initial;
            return edited;
        }

        //*************************************************************************************************
        //*************************************************************************************************
        //*************************************************************************************************

        // unary operators

        private void UnaryTokenMover (List<Token> initial, List<Token> edited, ref int getIndex)
        {
            if (initial [getIndex].type == TokenType.Operator)
            {
                switch (initial [getIndex+1].type)
                {
                    case TokenType.Numeric:
                    { 
                        if (initial [getIndex].annotatedText [0].IsPlusMinus)
                        { 
                            Token t1 = new Token (TokenType.Numeric, initial [getIndex].annotatedText + initial [getIndex + 1].annotatedText);
                            edited.Add (t1);
                            getIndex += 2;
                        }
                        break;
                    }

                    case TokenType.VariableName:
                    case TokenType.GroupingParens:
                    case TokenType.BracketsColon:
                    case TokenType.BracketsSemi: 
                    case TokenType.BracketsComma:
                    case TokenType.BracketsSpace:
                    case TokenType.FunctionName:
                    {
                        if (initial [getIndex].annotatedText [0].IsPlusMinus)
                        { 
                            Token t1 = new Token (TokenType.Numeric, initial [getIndex].annotatedText + new AnnotatedString ("1"));
                            edited.Add (t1);
                            Token t2 = new Token (TokenType.Operator, new AnnotatedString ("*"));
                            edited.Add (t2);
                            edited.Add (initial [getIndex+1]);
                            getIndex += 2;
                        }
                        else if (initial [getIndex].annotatedText [0].IsTilde) // "not" function
                        {
                            edited.Add (new Token (TokenType.FunctionName, new AnnotatedString ("not"))); // NESTING LEVELS NEEDED?

                            // add parens unless outer level is already parens                
                            if (initial [getIndex].type != TokenType.GroupingParens) 
                                edited.Add (new Token (TokenType.FunctionParens, initial [getIndex + 1].annotatedText.AddOuterParens ()));
                            else                                                     
                                edited.Add (new Token (TokenType.FunctionParens, initial [getIndex + 1].annotatedText));

                            getIndex += 2;
                        }
                        break;
                    }

                    default: throw new Exception ("Token parsing error");
                }
            }
        }

     //   private bool IsUnaryOpToken (Token tok) {return tok.type == TokenType.Operator && tok.annotatedText [0].IsUnaryOp;}
        private bool IsOpToken (Token tok) {return tok.type == TokenType.Operator;}

        List<Token> BindUnaryOperators (List<Token> initial)
        {
            List<Token> edited = new List<Token> ();

            List<int> operatorIndices = new List<int> ();

            int start = 0;

            while (start < initial.Count)
            {
                int index = initial.FindIndex (start, IsOpToken);// IsUnaryOpToken);

                if (index == -1)
                    break;

                operatorIndices.Add (index);
                start = index + 1;
            }

            // if none found, just return original list
            if (operatorIndices.Count == 0)
                return initial;  


            int operatorIndex;// = operatorIndices [0];
            int get = 0; // where we are reading from initial list


            if (operatorIndices [0] == 0) // means first token is a unary op so bind it to the second token
            { 
                operatorIndex = operatorIndices [0];
                UnaryTokenMover (initial, edited, ref get);
                operatorIndices.RemoveRange (0, 1);
            }

            // Look for consecutive operators ending in a unary op
            List<int> unaryOpIndices = new List<int> ();

            while (operatorIndices.Count > 1)
            {
                int i1 = operatorIndices [0];
                int i2 = operatorIndices [1];

                if (i2 == i1 + 1)
                {
                    unaryOpIndices.Add (i2);
                    operatorIndices.RemoveRange (0, 2);
                }
                else
                    operatorIndices.RemoveRange (0, 1);
            }


            while (unaryOpIndices.Count > 0)
            {
                while (get < unaryOpIndices [0])
                    edited.Add (initial [get++]);

                UnaryTokenMover (initial, edited, ref get);
                unaryOpIndices.RemoveRange (0, 1);
            }




            while (get < initial.Count)
                edited.Add (initial [get++]);




            //// Look for consecutive operators ending in a unary op

            //for (int i = 0; i<initial.Count-1; i++)
            //{
            //    if (initial [i].type != TokenType.Operator)
            //        edited.Add (initial [i]);
            //    else
            //    {
            //        if (initial [i+1].type != TokenType.Operator)
            //            edited.Add (initial [i]);

            //        else // found 2 consecutive operator tokens
            //        {

            //        }
            //    }


            //}


                //            for (int i = initial.Count-1; i>0; i--)
                //            {
                //                try
                //                {
                //                    TokenType im2 = i >= 2 ? initial [i-2].type : TokenType.ArithmeticOperator;
                //                    TokenType im1 = initial [i-1].type;

                //                    if (im2 == TokenType.ArithmeticOperator && im1 == TokenType.ArithmeticOperator)
                //                    {
                //                        if (TokenUtils.IsUnaryOp (initial [i-1].text))
                //                        {
                //                            switch (initial [i-1].text [0])
                //                            {
                //                                case '+':
                //                                    initial.RemoveAt (i-1);
                //                                    break;

                //                                case '-':
                //                                {
                //                                    string op = "";
                //                                    TokenType ty = initial [i].type;

                //                                    switch (ty)
                //                                    {
                //                                        case TokenType.VariableName:
                //                                        case TokenType.Alphanumeric:
                //                                        case TokenType.GroupingParens:
                //                                        {
                //                                            Token tok = new Token (TokenType.GroupingParens, "(-1 * " + initial [i].text + ")");
                //                                            initial [i-1] = tok;
                //                                            initial.RemoveAt (i);
                //                                        }
                //                                        break;

                //                                        case TokenType.Numeric:
                //                                        {
                //                                            if (initial [i].text [0] == '-') op = initial [i].text.Remove (0, 1);
                //                                            else if (initial [i].text [0] == '+') {op = initial [i].text.Remove (0, 1); op = "-" + op;}
                //                                            else op = "-" + initial [i].text;

                //                                            Token tok = new Token (TokenType.Numeric, op);
                //                                            initial [i-1] = tok;
                //                                            initial.RemoveAt (i);
                //                                        }
                //                                        break;

                //                                        case TokenType.FunctionFile:
                //                                        case TokenType.FunctionName:
                //                                        case TokenType.Pair:
                //                                        {
                //                                            initial.RemoveAt (i-1);
                //                                            initial.Insert (i-1, new Token (TokenType.Numeric, "-1"));
                //                                            initial.Insert (i, new Token (TokenType.ArithmeticOperator, "*"));
                //                                        }
                //                                        break;

                //                                        default:
                //                                            throw new Exception ("Unary minus error, type = " + ty.ToString ());
                //                                    }
                //                                }
                //                                break;

                //                                case '~':
                //                                {
                //                                    TokenPair tok = new TokenPair (); // TokenType.GroupingParens, op);
                //                                    tok.pairType = TokenPairType.Function;

                //                                    tok.t0 = new Token (TokenType.FunctionName, "Not");
                //                                    tok.t1 = initial [i];
                //                                    //tok.t1 = new Token (TokenType.FunctionParens, "(" + initial [i].text + ")");

                //                                    initial [i-1] = tok;
                //                                    initial.RemoveAt (i);
                //                                }
                //                                break;
                //                            }
                //                        }
                //                    }
                //                }

                //                catch (Exception ex)
                //                {
                //                    throw new Exception ("Error binding unary ops: " + ex.Message); // initial [i+1].text);
                //                }
                //            }

                return edited;
        }

        //        //*************************************************************************************************

        //        // combine consecutive operator characters into single 2-char operator

        //        List<Token> LookForTwoCharOperators (List<Token> initial)
        //        {
        //            List<Token> final = new List<Token> ();

        //            for (int i=0; i<initial.Count-1; i++)
        //            {
        //                TokenType t0 = initial [i].type;
        //                TokenType t1 = initial [i+1].type;

        //                if ((t0 == TokenType.ArithmeticOperator || t0 == TokenType.Decimal) && t1 == TokenType.ArithmeticOperator)
        //                {
        //                    string op = initial [i].text + initial [i+1].text;

        //                    if (TokenUtils.IsTwoCharOperator (op) == true)
        //                    {
        //                        final.Add (new Token (TokenType.ArithmeticOperator, op));
        //                        i++;
        //                    }
        //                    else
        //                        throw new Exception ("Illegal operator: " + op);
        //                }
        //                else
        //                    final.Add (initial [i]);
        //            }

        //            final.Add (initial [initial.Count - 1]);
        //            return final;
        //        }
    }
}











