using System;
using System.Collections.Generic;
using System.Reflection;

using PLCommon;

using PLFileSystem;

using PLLibrary;

using PLWorkspace;

namespace PLMain
{
    public partial class TokenParsing
    {
        public readonly List<TokenSet> History = new List<TokenSet> ();
        public               TokenSet  Results {get {return History [History.Count - 1];}}

        private delegate TokenSet ParsingStep (TokenSet src, out bool save);

        private static readonly List<ParsingStep> ParsingSteps = new List<ParsingStep> ()
        {
            LookupAlphanumerics,    // Assign a more specific type to an Alphanumeric
            IdentifyParens,         // grouping, function args, sub matrix
            IdentifyBrackets,       // by separator: :, ;, etc.
            ReplaceTransposeOps,    // A' => Transpose (A)
            ReplaceCollapseOps,     // b (:) => Collapse (b)
            CombineTokensIntoPairs, // combine FuncName (FuncArgs) or Matrix (range) into TokenPairs
            CheckSubmatrixArgs,     // b (3 : end) => b (3 : (length (b)))
            IdentifyOperatorType,   // label operators as binary or unary
            BindUnaryOperators,     // -, A => (-1 * A)
            RenameTwoCharOperator,  // rename to BinaryOperator
        };

        //****************************************************************************

        internal TokenSet ParsingPassTwo (TokenSet initial)
        {
            bool saveFlag;

            History.Add (initial);

            foreach (ParsingStep nextStep in ParsingSteps)
            { 
                TokenSet edited = nextStep (Results, out saveFlag); 

                if (saveFlag) 
                    History.Add (edited);
            }

            //edited = 

            return Results;
        }

        //*************************************************************************************************

        // label operators as binary or unary

        private static TokenSet IdentifyOperatorType (TokenSet initial, out bool saveFlag)
        {
            saveFlag = false;

            //
            // find all operator tokens
            //
            List<int> operatorIndices = initial.FindTokenTypeIndices (TokenType.Operator);

            // if none found, just return
            if (operatorIndices.Count == 0)
                return initial;

            TokenSet edited = new TokenSet ("IdentifyOperatorType");
            saveFlag = true;

            //
            // determine whether each operator is unary or binary
            //

            int get = 0;

            foreach (int index in operatorIndices)
            {
                while (get < index)
                    edited.Add (initial [get++]);

                TokenType prevType = index > 0 ? initial [index-1].Type : TokenType.None;

                switch (prevType)
                {
                    case TokenType.None:
                    case TokenType.Operator:
                    case TokenType.BinaryOperator:
                    case TokenType.TwoCharOperator:
                    case TokenType.EqualSign:
                        edited.Add (new Token (TokenType.UnaryOperator, initial [index].AnnotatedText));
                        break;

                    default:
                        edited.Add (new Token (TokenType.BinaryOperator, initial [index].AnnotatedText));
                        break;
                }

                get++;
            }

            while (get < initial.Count)
                edited.Add (initial [get++]);

            return edited;
        }

        //*************************************************************************************************

        // Assign a more specific type to an Alphanumeric

        private static TokenSet LookupAlphanumerics (TokenSet initial, out bool saveFlag)
        {
            TokenSet edited = new TokenSet ("LookupAlphanumerics");
            saveFlag = false;

            for (int i = 0; i<initial.Count; i++)
            {
                if (initial [i].Type == TokenType.Alphanumeric)
                {
                    string str = initial [i].AnnotatedText.Plain;
                    saveFlag = true;

                    SymbolicNameTypes whatIs = Workspace.WhatIs (str);

                    if (whatIs != SymbolicNameTypes.Unknown)
                    {
                        switch (whatIs)
                        {
                            case SymbolicNameTypes.Variable:
                                edited.Add (new Token (TokenType.VariableName, initial [i].AnnotatedText));
                                break;

                            case SymbolicNameTypes.Function:
                                edited.Add (new Token (TokenType.Function, initial [i].AnnotatedText));
                                break;

                            case SymbolicNameTypes.WorkspaceCommand:
                                throw new Exception ("Unexpected Workspace Command: " + str);
                                //break;

                            default:
                                throw new Exception ("Unsupported token type: " + str);
                        }
                    }

                    else if (LibraryManager.WhatIs (str) == SymbolicNameTypes.Function)
                        edited.Add (new Token (TokenType.Function, initial [i].AnnotatedText));

                    else if (FileSystem.IsFunctionFile (initial [i].AnnotatedText.Plain))
                        edited.Add (new Token (TokenType.FunctionFile, initial [i].AnnotatedText));

                    // a script name must be the only token on a line
                    else if (FileSystem.IsScriptFile (initial [i].AnnotatedText.Plain) && initial.Count == 1)
                        edited.Add (new Token (TokenType.ScriptFile, initial [i].AnnotatedText));

                    else
                        edited.Add (new Token (TokenType.Undefined, initial [i].AnnotatedText));
                }

                else // token type not alphanumeric, so just move it to new list
                {
                    edited.Add (initial [i]);
                }
            }

            return edited;
        }

        //*************************************************************************************************

        // Identify parenthesis as:
        //    GroupingParens,  // A * (B + C)
        //    FunctionParens,  // Func1 (P, Q, R, S)
        //    SubmatrixParens, // ZMat (Rs, Cs); % (row select, col select)

        private static TokenSet IdentifyParens (TokenSet tokens, out bool saveFlag)
        {
            TokenSet edited = new TokenSet ("IdentifyParens");
            saveFlag = false;

            for (int i = 0; i<tokens.Count; i++)
            {
                if (tokens [i].Type == TokenType.Parens)
                {
                    saveFlag = true;

                    if (i == 0) // parens with nothing before them
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

                            case TokenType.Function:
                            case TokenType.FunctionFile:
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

        private static TokenSet IdentifyBrackets (TokenSet initial, out bool saveFlag)
        {
            TokenSet edited = new TokenSet ("IdentifyBrackets");
            saveFlag = false;

            for (int i=0; i<initial.Count; i++)
            {
                if (initial [i].Type == TokenType.Brackets)
                {
                    saveFlag = true;

                    AnnotatedString tokenText = initial [i].AnnotatedText;
                    AnnotatedString inside = AnnotatedString.RemoveWrapper (tokenText);

                    List<char> separatorsFound = new List<char> ();
                    int initalNesting = tokenText [0].NestingLevel;

                    for (int j = 0; j<tokenText.CharacterCount; j++)
                    {
                        AnnotatedChar tokenChar = tokenText [j];

                        if (tokenChar.NestingLevel == initalNesting)
                        {
                            if (TokenUtils.bracketSeparators.Contains (tokenChar.Character))
                            {
                                if (separatorsFound.Contains (tokenChar.Character) == false)
                                    separatorsFound.Add (tokenChar.Character);
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

                    edited.Add (new Token (lowestBSP.tokenType, initial [i].AnnotatedText));
                }

                else
                    edited.Add (initial [i]);
            }

            return edited;
        }

        //*************************************************************************************************

        // replace collapse operator by function call

        // b (:) => Collapse (b)

        private static TokenSet ReplaceCollapseOps (TokenSet initial, out bool saveFlag)
        {
            saveFlag = false;

            // look for any SubmatrixParens tokens
            List<int> submatrixParenIndices = initial.FindTokenTypeIndices (TokenType.SubmatrixParens);

            // if none found, return initial
            if (submatrixParenIndices.Count == 0)
                return initial;

            // see if any of those contain just (:)
            List<int> collapseOps = new List<int> ();

            foreach (int i in submatrixParenIndices)
            {
                string text = initial [i].AnnotatedText.Plain.Trim ();
                text = text.Substring (1, text.Length - 2).Trim (); // remove enclosing parens and any extra spaces
                if (text == ":")
                    collapseOps.Add (i);
            }

            if (collapseOps.Count == 0)
                return initial;

            saveFlag = true;

            //*************************************************************

            // same pattern as "transpose"

            TokenSet edited = new TokenSet ("ReplaceCollapseOps");
            int get = 0;

            foreach (int index in collapseOps)
            {
                while (get < index - 1)
                    edited.Add (initial [get++]);

                edited.Add (new Token (TokenType.Function, new AnnotatedString ("collapse")));

                // add parens unless outer level is already parens                
                if (initial [get].Type != TokenType.GroupingParens) edited.Add (new Token (TokenType.FunctionParens, AnnotatedString.AddOuterParens (initial [get].AnnotatedText)));
                else edited.Add (new Token (TokenType.FunctionParens, initial [get].AnnotatedText));

                get += 2;
            }

            // move tokens after last collapse
            while (get < initial.Count)
                edited.Add (initial [get++]);

            return edited;
        }

        //*************************************************************************************************

        // replace transpose operator by function call

        private static TokenSet ReplaceTransposeOps (TokenSet initial, out bool saveFlag)
        {
            TokenSet edited = new TokenSet ("ReplaceTransposeOps");
            saveFlag = false;

            List<int> transposeIndices = initial.FindTokenTypeIndices (TokenType.Transpose);

            // if none found, just return original list
            if (transposeIndices.Count == 0)
                return initial;

            saveFlag = true;
            int get = 0;

            foreach (int index in transposeIndices)
            {
                while (get < index - 1)
                    edited.Add (initial [get++]);

                edited.Add (new Token (TokenType.Function, new AnnotatedString ("transpose")));

                // add parens unless outer level is already parens                
                if (initial [get].Type != TokenType.GroupingParens) edited.Add (new Token (TokenType.FunctionParens, AnnotatedString.AddOuterParens (initial [get].AnnotatedText)));
                else edited.Add (new Token (TokenType.FunctionParens, initial [get].AnnotatedText));

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
        private static TokenSet BindUnaryOperators (TokenSet initial, out bool saveFlag)
        {
            saveFlag = false;
            List<int> unaryOpIndices = initial.FindTokenTypeIndices (TokenType.UnaryOperator);

            // if none found, just return original list
            if (unaryOpIndices.Count == 0)
                return initial;

            TokenSet edited = new TokenSet ();
            saveFlag = true;

            int get = 0; // index used to copy out of initial

            foreach (int index in unaryOpIndices)
            {
                while (get < index)
                    edited.Add (initial [get++]);

                if (initial [index].AnnotatedText.CharacterCount > 1)
                    throw new Exception ("Error in unary operator string: " + initial [index].AnnotatedText.Plain [0]);

                switch (initial [index].AnnotatedText.Plain [0])
                {
                    case '+':
                    case '-':
                        string str = initial [get].AnnotatedText.Plain + "1";

                        Token t1 = new Token (TokenType.Numeric, new AnnotatedString (str));
                        edited.Add (t1);

                        Token t2 = new Token (TokenType.BinaryOperator, new AnnotatedString ("*"));
                        edited.Add (t2);
                        edited.Add (initial [get+1]);
                        get += 2;
                        break;

                    case '~': // "not" function
                        Token t3 = new Token (TokenType.Function, new AnnotatedString ("not"));

                        // add parens unless outer level is already parens                
                        Token t4 = initial [get].Type != TokenType.GroupingParens ?
                                   new Token (TokenType.FunctionParens, AnnotatedString.AddOuterParens (initial [get + 1].AnnotatedText)) :
                                   new Token (TokenType.FunctionParens, initial [get + 1].AnnotatedText);

                        TokenPair funcPair = new TokenPair (TokenPairType.FunctionWithArgs, t3, t4);
                        edited.Add (funcPair);
                        get += 2;
                        break;

                    default:
                        throw new Exception ("TokenParsing found unsupported unary operator: " + initial [index].AnnotatedText.Plain [0]);
                }
            }

            while (get < initial.Count)
                edited.Add (initial [get++]);

            return edited;
        }

        //*************************************************************************************************

        private static TokenSet CombineTokensIntoPairs (TokenSet initial, out bool saveFlag)
        {
            TokenSet edited = new TokenSet ("CombineTokensIntoPairs");
            saveFlag = false;

            for (int i = 0; i<initial.Count-1; i++)
            {
                switch (initial [i].Type)
                {
                    case TokenType.VariableName:
                        if (initial [i+1].Type == TokenType.SubmatrixParens)
                        {
                            TokenPair submatPair = new TokenPair (TokenPairType.Submatrix, initial [i], initial [i+1]);
                            edited.Add (submatPair);
                            i++; // don't look at the parens token a second time
                            saveFlag = true;
                        }
                        else
                            edited.Add (initial [i]);
                        break;

                    case TokenType.FunctionFile: // .m file
                        if (initial [i+1].Type == TokenType.FunctionParens)
                        {
                            initial [i].Type = TokenType.FunctionFile;
                            TokenPair funcPair = new TokenPair (TokenPairType.FunctionWithArgs, initial [i], initial [i+1]);
                            edited.Add (funcPair);
                            i++; // don't look at the function parens token a second time
                            saveFlag = true;
                        }

                        else
                        { 
                            throw new Exception ("Zero-arg .m file functions not allowed");
                            //initial [i].Type = TokenType.ZeroArgFunction;
                            //edited.Add (initial [i]);
                        }

                        break;

                    case TokenType.Function:
                        if (initial [i+1].Type == TokenType.FunctionParens)
                        {
                            initial [i].Type = TokenType.FunctionWithArgs;
                            TokenPair funcPair = new TokenPair (TokenPairType.FunctionWithArgs, initial [i], initial [i+1]);
                            edited.Add (funcPair);
                            i++; // don't look at the function parens token a second time
                            saveFlag = true;
                        }

                        else
                        { 
                            initial [i].Type = TokenType.ZeroArgFunction;
                            edited.Add (initial [i]);
                        }

                        break;

                    default:
                        edited.Add (initial [i]);
                        break;
                }
            }

            // add last initial token if it isn't part of a token pair 
            int last = initial.Count - 1;

            if (initial [last].Type == TokenType.Function) 
                initial [last].Type = TokenType.ZeroArgFunction;

            if ((initial [last].Type != TokenType.SubmatrixParens) && (initial [last].Type != TokenType.FunctionParens))
                edited.Add (initial [last]);

            return edited;
        }

        //*************************************************************************************************

        // Replace any "end" in submatrix args with rows (), cols () or length ()

        // b (3 : end)          => b (3 : (length (b)))
        // c (3 : end, 4 : end) => c (3 : (rows (c)), 4 : (cols (c))

        private static TokenSet CheckSubmatrixArgs (TokenSet initial, out bool saveFlag)
        {
            saveFlag = false;

            // look for any token pairs. they could be a FunctionWithArgs or Submatrix
            List<int> tokenPairs = initial.FindTokenTypeIndices (TokenType.Pair);

            // if none found, return initial
            if (tokenPairs.Count == 0)
                return initial;

            // look for Submatrix pairs among tokenPairs
            List<int> submatrixPairs = initial.FindPairTypeIndices (tokenPairs,  // look at these indices
                                                                    TokenPairType.Submatrix); // for this type
            // if none found, return initial
            if (submatrixPairs.Count == 0)
                return initial;

            //******************************************

            TokenSet edited = new TokenSet ("CheckSubmatrixArgs");

            int get = 0; // index used to copy out of initial

            for (int i = 0; i<submatrixPairs.Count; i++)
            {
                int index = submatrixPairs [i];

                while (get < index)
                    edited.Add (initial [get++]);

                // tokens [index] is a submatrixPair
                TokenPair tp = initial [get] as TokenPair;
                string name = tp.Get1.AnnotatedText.Plain; // name of vector or matrix
                string args = tp.Get2.AnnotatedText.Plain;

                List<string> aset = BreakIntoSubstrings (tp.Get2.AnnotatedText, delegate (AnnotatedChar ac) {return ac.IsComma;});

                if (aset.Count == 1) // working on a vector
                {
                    string initialSelect = args;

                    if (initialSelect.Contains ("end"))
                    { 
                        saveFlag = true;
                        string newSelect = initialSelect.Replace ("end", "(length (" + name + "))");

                        Token     tok1    = new Token     (TokenType.VariableName,    new AnnotatedString (name));
                        Token     tok2    = new Token     (TokenType.SubmatrixParens, new AnnotatedString (newSelect));
                        TokenPair tokPair = new TokenPair (TokenPairType.Submatrix, tok1, tok2);

                        edited.Add (tokPair);
                    }
            
                    else
                        edited.Add (initial [get]);
                }

                else if (aset.Count == 2)
                {
                    string initialRows = aset [0];//.Plain;
                    string initialCols = aset [1];//.Plain;

                    bool rowsJustColon   = initialRows == ":";
                    bool colsJustColon   = initialCols == ":";
                    bool rowsContainsEnd = initialRows.Contains ("end");
                    bool colsContainsEnd = initialCols.Contains ("end");

                    if (rowsContainsEnd == false && rowsJustColon == false && colsContainsEnd == false && colsJustColon == false)
                    {
                        edited.Add (initial [get]);
                    }

                    else
                    {
                        saveFlag = true;
                        string newRows;

                        if (rowsContainsEnd)    newRows = initialRows.Replace ("end", "(rows (" + name + "))");
                        else if (rowsJustColon) newRows = "1 : (rows (" + name + "))";
                        else                    newRows = initialRows;

                        string newCols = "";
                        
                        if (colsContainsEnd)    newCols = initialCols.Replace ("end", "(cols (" + name + "))");
                        else if (colsJustColon) newCols = "1 : (cols (" + name + "))";
                        else                    newCols = initialCols;

                        //Console.WriteLine (initialRows);
                        //Console.WriteLine (newRows);
                        //Console.WriteLine (initialCols);
                        //Console.WriteLine (newCols);

                        string newSelect = "(" + newRows + ", " + newCols + ")";

                        Token     tok1    = new Token     (TokenType.VariableName,    new AnnotatedString (name));
                        Token     tok2    = new Token     (TokenType.SubmatrixParens, new AnnotatedString (newSelect));
                        TokenPair tokPair = new TokenPair (TokenPairType.Submatrix, tok1, tok2);

                        edited.Add (tokPair);
                    }
                }

                else
                    throw new Exception ("Submatrix error, too many dimensions: " + name + " " + initial [i].AnnotatedText.Plain);

                get += 1;
            }

            return edited;
        }

        //*************************************************************************************************

        private static TokenSet RenameTwoCharOperator (TokenSet initial, out bool saveFlag)
        {
            saveFlag = false;
            List<int> twoCharOpIndices = initial.FindTokenTypeIndices (TokenType.TwoCharOperator);

            // if none found, just return original list
            if (twoCharOpIndices.Count == 0)
                return initial;

            TokenSet edited = new TokenSet ();
            saveFlag = true;

            int get = 0; // index used to copy out of initial

            foreach (int index in twoCharOpIndices)
            {
                while (get < index)
                    edited.Add (initial [get++]);

                edited.Add (new Token (TokenType.BinaryOperator, initial [index].AnnotatedText));
                get++;
            }

            while (get < initial.Count)
                edited.Add (initial [get++]);

            return edited;
        }
    }
}
