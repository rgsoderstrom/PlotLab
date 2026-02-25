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

            edited = CombineTokensIntoPairs (edited); // combine FuncName (FuncArgs) or Matrix [range] into TokenPairs

            edited = IdentifyOperatorType (edited);

            edited = BindUnaryOperators (edited); // -, A => (-1 * A),
                                                  // -, 7 => -7

            edited = RenameTwoCharOperator (edited); // rename to BinaryOperator

            return edited;
        }

        //*************************************************************************************************

        // label operators as binary or unary. in-place

        List<IToken> IdentifyOperatorType (List<IToken> initial)
        {
            //
            // find all operator tokens
            //
            List<int> operatorIndices = new List<int> ();

            int start = 0;

            while (start < initial.Count)
            {
                int index = initial.FindIndex (start, delegate (IToken tok) {return tok.Type == TokenType.Operator;});

                if (index == -1)
                    break;

                operatorIndices.Add (index);
                start = index + 1;
            }

            // if none found, just return
            if (operatorIndices.Count == 0)
                return initial;  

            //
            // determine whether each operator is unary or binary
            //
            for (int i=0; i<operatorIndices.Count; i++)
            {
                int index = operatorIndices [i];                
                TokenType prevType = index > 0 ? initial [index-1].Type : TokenType.None;

                switch (prevType)
                {
                    case TokenType.None:
                    case TokenType.Operator:
                    case TokenType.BinaryOperator:
                    case TokenType.TwoCharOperator:
                    case TokenType.EqualSign:
                        initial [index].Type = TokenType.UnaryOperator; // a * -b
                        break;

                    default:
                        initial [index].Type = TokenType.BinaryOperator; // a * b
                        break;
                }
            }

            return initial;
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
                int index = initial.FindIndex (start, delegate (IToken tok) {return tok.Type == TokenType.UnaryOperator;});

                if (index == -1)
                    break;

                operatorIndices.Add (index);
                start = index + 1;
            }

            // if none found, just return original list
            if (operatorIndices.Count == 0)
                return initial;  

            int get = 0; // index used to copy out of initial

            for (int i=0; i<operatorIndices.Count; i++)
            {
                int index = operatorIndices [i];

                while (get < index)
                    edited.Add (initial [get++]);

                if (initial [index].AnnotatedText.Count > 1)
                    throw new Exception ("Error in unary operator string: " + initial [index].AnnotatedText.Plain [0]);

                switch (initial [index].AnnotatedText.Plain [0])
                {
                    case '+':
                    case '-':
                        Token t1 = new Token (TokenType.Numeric, initial [get].AnnotatedText + new AnnotatedString ("1"));
                        edited.Add (t1);
                        Token t2 = new Token (TokenType.BinaryOperator, new AnnotatedString ("*"));
                        edited.Add (t2);
                        edited.Add (initial [get+1]);
                        get += 2;
                        break;

                    case '~': // "not" function
                        Token t3 = new Token (TokenType.FunctionName, new AnnotatedString ("not")); // NESTING LEVELS NEEDED?

                        // add parens unless outer level is already parens                
                        Token t4 = initial [get].Type != TokenType.GroupingParens ? 
                                   new Token (TokenType.FunctionParens, initial [get + 1].AnnotatedText.AddOuterParens ()) :
                                   new Token (TokenType.FunctionParens, initial [get + 1].AnnotatedText);

                        TokenPair funcPair = new TokenPair (TokenPairType.Function, t3, t4);
                        edited.Add (funcPair);
                        get += 2;
                        break;

                    default:
                        throw new Exception ("Unsupported unary operator: " + initial [index].AnnotatedText.Plain [0]);
                }
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

        //*************************************************************************************************

        private List<IToken> RenameTwoCharOperator (List<IToken> initial)
        {
            for (int i=0; i<initial.Count; i++)
                if (initial [i].Type == TokenType.TwoCharOperator)
                    initial [i].Type = TokenType.BinaryOperator;

            return initial;
        }
    }
}











