using System;
using System.Collections.Generic;   

////using PLCommon;
////using PLWorkspace;
using PLLibrary;

namespace Main
{
    public partial class TokenParsing
    {
        internal List<IToken> ParsingPassTwo (List<IToken> initial, IWorkspace workspace, IFileSystem files)
        {
            List<IToken> edited = LookupAlphanumerics (initial, workspace, files);

            edited = IdentifyParens (edited, workspace); // grouping, function args, sub matrix

            edited = IdentifyBrackets (edited); // by separator: :, ;, etc.

            edited = ReplaceTransposeOps (edited); // A' => Transpose (A)

            edited = BindUnaryOperators (edited); // -, A => (-1 * A),
                                                  // -, 7 => -7

            edited = CombineTokensIntoPairs (edited); // combine FuncName (FuncArgs) or Matrix [range] into TokenPairs

            return edited;
        }

        //*************************************************************************************************

        // Assign a more specific type to an Alphanumeric

        List<IToken> LookupAlphanumerics (List<IToken> initial,
                                          IWorkspace workspace,
                                          IFileSystem files)
        {
            for (int i = 0; i<initial.Count; i++)
            {
                if (initial [i].Type == TokenType.Alphanumeric)
                {
                    if (workspace.IsDefined (initial [i].AnnotatedText.Plain))
                        initial [i].Type = TokenType.VariableName;

                    else if (LibraryManager.Contains (initial [i].AnnotatedText.Plain))
                        initial [i].Type = TokenType.FunctionName;

                    else if (files.IsFunctionFile (initial [i].AnnotatedText.Plain))
                        initial [i].Type = TokenType.FunctionName;

                    else if (files.IsScriptFile (initial [i].AnnotatedText.Plain))
                        initial [i].Type = TokenType.ScriptFile;

                    else initial [i].Type = TokenType.Undefined;
                }
            }

            return initial;
        }

        //*************************************************************************************************

        // Identify parenthesis as:
        //    GroupingParens,  // A * (B + C)
        //    FunctionParens,  // Func1 (P, Q, R, S)
        //    SubmatrixParens, // ZMat (Rs, Cs); % (row select, col select)

        List<IToken> IdentifyParens (List<IToken> tokens, IWorkspace final)
        {
            List<IToken> edited = new List<IToken> ();

            for (int i = 0; i<tokens.Count; i++)
            {
                if (tokens [i].Type == TokenType.Parens)
                {
                    if (edited.Count == 0) 
                    {
                        Token tok = new Token (TokenType.GroupingParens, tokens [i].AnnotatedText);
                        edited.Add (tok);
                    }
                    else // look at previous type to see what type these parens are
                    {
                        switch (edited [edited.Count-1].Type)
                        {
                            case TokenType.VariableName:
                                Token tok1 = new Token (TokenType.SubmatrixParens, tokens [i].AnnotatedText);
                                edited.Add (tok1);
                                break;

                            case TokenType.FunctionName:
                                Token tok2 = new Token (TokenType.FunctionParens, tokens [i].AnnotatedText);
                                edited.Add (tok2);
                                break;

                            default:
                                Token tok3 = new Token (TokenType.GroupingParens, tokens [i].AnnotatedText);
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

        List<IToken> IdentifyBrackets (List<IToken> initial)
        {
            const char NoneFound = '?'; // no separators found

            for (int i = 0; i<initial.Count; i++)
            {
                if (initial [i].Type == TokenType.Brackets)
                {
                    AnnotatedString tokenText = initial [i].AnnotatedText;
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
                            initial [i].Type = TokenType.BracketsColon;
                            break;

                        case ';':
                            initial [i].Type = TokenType.BracketsSemi;
                            break;

                        case ',':
                            initial [i].Type = TokenType.BracketsComma;
                            break;

             //           case ' ':
               //             break;

                        case NoneFound:
                            initial [i].Type = TokenType.BracketsSpace;
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

        private bool IsTransposeToken (IToken tok) {return tok.Type == TokenType.Transpose;}

        private List<IToken> ReplaceTransposeOps (List<IToken> initial)
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

            List<IToken> edited = new List<IToken> ();
            int get = 0;

            foreach (int index in transposeIndices)
            {
                while (get < index - 1)
                    edited.Add (initial [get++]);

                edited.Add (new Token (TokenType.FunctionName, new AnnotatedString ("transpose"))); // NESTING LEVELS NEEDED?

                // add parens unless outer level is already parens                
                if (initial [get].Type != TokenType.GroupingParens) edited.Add (new Token (TokenType.FunctionParens, initial [get].AnnotatedText.AddOuterParens ()));
                else                                                edited.Add (new Token (TokenType.FunctionParens, initial [get].AnnotatedText));

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

        private void UnaryTokenMover (List<IToken> initial, List<IToken> edited, ref int getIndex)
        {
            if (initial [getIndex].Type == TokenType.Operator)
            {
                switch (initial [getIndex+1].Type)
                {
                    case TokenType.Numeric:
                    { 
                        if (initial [getIndex].AnnotatedText [0].IsPlusMinus)
                        { 
                            Token t1 = new Token (TokenType.Numeric, initial [getIndex].AnnotatedText + initial [getIndex + 1].AnnotatedText);
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
                        if (initial [getIndex].AnnotatedText [0].IsPlusMinus)
                        { 
                            Token t1 = new Token (TokenType.Numeric, initial [getIndex].AnnotatedText + new AnnotatedString ("1"));
                            edited.Add (t1);
                            Token t2 = new Token (TokenType.Operator, new AnnotatedString ("*"));
                            edited.Add (t2);
                            edited.Add (initial [getIndex+1]);
                            getIndex += 2;
                        }
                        else if (initial [getIndex].AnnotatedText [0].IsTilde) // "not" function
                        {
                            edited.Add (new Token (TokenType.FunctionName, new AnnotatedString ("not"))); // NESTING LEVELS NEEDED?

                            // add parens unless outer level is already parens                
                            if (initial [getIndex].Type != TokenType.GroupingParens) 
                                edited.Add (new Token (TokenType.FunctionParens, initial [getIndex + 1].AnnotatedText.AddOuterParens ()));
                            else                                                     
                                edited.Add (new Token (TokenType.FunctionParens, initial [getIndex + 1].AnnotatedText));

                            getIndex += 2;
                        }
                        break;
                    }

                    default: throw new Exception ("Token parsing error: " + 
                                                  " " + initial [getIndex+1].Type +
                                                  " " + initial [getIndex+1].AnnotatedText.Plain);
                }
            }
        }

       // private bool IsOpToken (IToken tok) {return tok.Type == TokenType.Operator;}

        //
        //
        //
        List<IToken> BindUnaryOperators (List<IToken> initial)
        {
            List<IToken> edited = new List<IToken> ();

            List<int> operatorIndices = new List<int> ();

            int start = 0;

            while (start < initial.Count)
            {
                int index = initial.FindIndex (start, delegate (IToken tok) {return tok.Type == TokenType.Operator;});
             // int index = initial.FindIndex (start, IsOpToken);

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

            return edited;
        }

        //*************************************************************************************************

        private List<IToken> CombineTokensIntoPairs (List<IToken> initial)
        {
            List<IToken> edited = new List<IToken> ();

            for (int i=0; i<initial.Count-1; i++)
            {
                switch (initial [i].Type)
                {
                    case TokenType.VariableName:
                        if (initial [i+1].Type == TokenType.SubmatrixParens)
                        {
                            TokenPair submatPair = new TokenPair (TokenPairType.Submatrix, initial [i], initial [i+1]);
                            edited.Add (submatPair);
                            i++; // don't look at the parens token a second time
                        }
                        else
                            edited.Add (initial [i]);
                        break;

                    case TokenType.FunctionName:
                        if (initial [i+1].Type != TokenType.FunctionParens)
                            throw new Exception ("Function name " + initial [i].AnnotatedText.Plain + " without arguments");

                        TokenPair funcPair = new TokenPair (TokenPairType.Function, initial [i], initial [i+1]);
                        edited.Add (funcPair);
                        i++; // don't look at the function parens token a second time
                        break;

                    default:
                        edited.Add (initial [i]);
                        break;
                }
            }

            // add last initial token if it isn't part of a token pair 
            if ((initial [initial.Count - 1].Type != TokenType.SubmatrixParens) && (initial [initial.Count - 1].Type != TokenType.FunctionParens))
                edited.Add (initial [initial.Count - 1]); 

            return edited;
        }
    }
}











