using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using PLCommon;
//using PLWorkspace;
using PLLibrary;

namespace Main
{
    public partial class TokenParsing
    {
        private enum ParsingState
        {
            Between,
            InNumber,
            InAlpha,  // a variable or a function
            InString,
            InBrackets,
            InParenthesis,
            InOperator,
            InTwoCharOperator,
            InTranspose,
            SupressOutput,
            Leaving,
            Error
        };

        private class ParsingStatus
        {
            public ParsingState state = ParsingState.Between;
            public AnnotatedChar currentChar;
        }

        //*****************************************************************************************************

        public List<IToken> StringToTokens (AnnotatedString expression, IWorkspace workspace, IFileSystem files)
        {
            List<IToken> tokens = ParsingPassOne (expression);
            tokens = ParsingPassTwo (tokens, workspace, files);
            return tokens;
        }

        //*****************************************************************************************************
        //
        // ParsingPassOne
        //

        internal List<IToken> ParsingPassOne (AnnotatedString expression)
        {
            List<IToken> tokens = new List<IToken> ();
            IToken CurrentToken = null;

            //
            // catch some obvious errors
            //
            if (expression == null)
                return tokens; // empty list

            if (expression.Count == 0)
                return tokens; // empty list

            //
            // Parse expression
            //

            ParsingStatus status = new ParsingStatus ();

            bool done = false;
            int get = 0;  // next character index

            bool getNextChar = true;

            while (done == false)
            {
                if (getNextChar) 
                {
                    if (get < expression.Count)
                    {
                        status.currentChar = expression [get++];
                        getNextChar = false;
                    }

                    else if (get == expression.Count)
                    {
                        done = true;
                        status.state = ParsingState.Leaving;
                    }
                }

                switch (status.state)
                {
                    case ParsingState.Leaving:
                        ExitProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.SupressOutput:
                        getNextChar = SupressOutputProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.Between:
                        getNextChar = BetweenProcessing (status);
                        break;

                    case ParsingState.InString:
                        getNextChar = StringProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InOperator:
                        getNextChar = OperatorProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InTwoCharOperator:
                        getNextChar = TwoCharOperatorProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InTranspose:
                        getNextChar = TransposeProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InNumber:
                        getNextChar = NumberProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InAlpha:
                        getNextChar = AlphanumericProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InParenthesis:
                        getNextChar = ParenthesisProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InBrackets:
                        getNextChar = BracketProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.Error:
                        if      (CurrentToken == null)       throw new Exception (string.Format ("Parsing error, CurrentToken == null"));
                        else if (CurrentToken.AnnotatedText != null) throw new Exception (string.Format ("Parsing error, Text = {0}, char = {1}", CurrentToken.AnnotatedText, status.currentChar));
                        else                                 throw new Exception (string.Format ("Parsing error, atext == null, char = {0}", status.currentChar));

                    default:
                        throw new Exception ("Parsing state error");
                }
            }

            return tokens;
        }

        //*************************************************************************************************

        static bool BetweenProcessing (ParsingStatus status)
        {
          //  Console.WriteLine ("Between, " + status.currentChar);

            bool accepted = false;

            if      (status.currentChar.IsWhitespace)  accepted = true;

            else if (status.currentChar.IsSupress)     status.state = ParsingState.SupressOutput;
            
            else if (status.currentChar.IsTwoCharOp)   status.state = ParsingState.InTwoCharOperator;

            else if (status.currentChar.IsTranspose)   status.state = ParsingState.InTranspose;

            else if (status.currentChar.IsOperator)    status.state = ParsingState.InOperator;

            else if (status.currentChar.IsAlpha)       status.state = ParsingState.InAlpha;

            else if (status.currentChar.IsNumber)      status.state = ParsingState.InNumber;

            else if (status.currentChar.IsOpenParen)   status.state = ParsingState.InParenthesis;

            else if (status.currentChar.IsOpenBracket) status.state = ParsingState.InBrackets;

            else if (status.currentChar.IsOpenQuote)   status.state = ParsingState.InString;

            else
                status.state = ParsingState.Error;

            return accepted;
        }

        //*************************************************************************************************

        static void ExitProcessing (List<IToken> tokens, ref IToken current, ParsingStatus status)
        {
            if (current != null)
            {
                tokens.Add (current);
                current = null;
            }
        }

        //*************************************************************************************************

        static bool AlphanumericProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
            bool accepted = false;

          //  Console.WriteLine ("Alphanumeric, " + status.currentChar);

            // if no token in progress, start a new one
            if (token == null)
            {
                token = new Token (TokenType.Alphanumeric, status.currentChar);
                accepted = true;
            }
            else
            { 
                if (status.currentChar.IsAlpha || status.currentChar.IsNumber)
                {
                    token.AnnotatedText.Append (status.currentChar);
                    accepted = true;
                }
                else
                {
                    tokens.Add (token);
                    token = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }


        //*************************************************************************************************

        static bool OperatorProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
          //  Console.WriteLine ("Operator, " + status.currentChar);

            bool accepted = false;

            // if no token in progress, start a new one
            if (token == null)
            {
                token = new Token (TokenType.Operator, status.currentChar);
                accepted = true;
            }
            else 
            {
                if (status.currentChar.IsOperator)
                {
                    token.AnnotatedText.Append (status.currentChar);
                    accepted = true; 
                }

                else
                {
                    tokens.Add (token);
                    token = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }

        static bool TwoCharOperatorProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
          //  Console.WriteLine ("Operator, " + status.currentChar);

            bool accepted = false;

            // if no token in progress, start a new one
            if (token == null)
            {
                token = new Token (TokenType.TwoCharOperator, status.currentChar);
                accepted = true;
            }
            else 
            {
                if (status.currentChar.IsTwoCharOp)
                {
                    token.AnnotatedText.Append (status.currentChar);
                    accepted = true; 
                }

                else
                {
                    tokens.Add (token);
                    token = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }

        //*************************************************************************************************

        static bool TransposeProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
          //  Console.WriteLine ("Transpose, " + status.currentChar);

            token = new Token (TokenType.Transpose, status.currentChar);
            tokens.Add (token);
            token = null;
            status.state = ParsingState.Between;

            return true;
        }

        static bool SupressOutputProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
          //  Console.WriteLine ("SupressOutputProcessing, " + status.currentChar);

            token = new Token (TokenType.SupressPrinting, status.currentChar);
            tokens.Add (token);
            token = null;
            status.state = ParsingState.Between;//.Leaving;

            return true;
        }

        //*************************************************************************************************

        static bool NumberProcessing (List<IToken> tokens, ref IToken current, ParsingStatus status)
        {
           // Console.WriteLine ("Number, " + status.currentChar);

            bool accepted = false;

            // if no token in progress, start a new one
            if (current == null)
            {
                current = new Token (TokenType.Numeric, status.currentChar);
                accepted = true;
            }
            else
            {
                if (status.currentChar.IsNumber)// ||  status.currentChar.IsDecimal || status.currentChar.IsExponential)
                {
                    current.AnnotatedText.Append (status.currentChar);//.Character;
                    accepted = true;
                }
                else
                {
                    tokens.Add (current);
                    current = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }

        //*************************************************************************************************

        static private int quoteNestingLevel = -1;

        static bool StringProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
            //Console.WriteLine ("String, " + status.currentChar);

            bool accepted = false;

            // this state's entry function
            if (token == null)
            {
                token = new Token (TokenType.String, status.currentChar);
                quoteNestingLevel = status.currentChar.QuoteLevel;
                accepted = true;
            }
            else
            { 
                if (status.currentChar.QuoteLevel >= quoteNestingLevel)
                {
                    token.AnnotatedText.Append (status.currentChar);
                    accepted = true;
                }
                else
                {
                    tokens.Add (token);
                    token = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }
             
        //*************************************************************************************************

        //static TokenType LookupNameType (string name, Workspace workspace)
        //{
        //    SymbolicNameTypes ty = workspace.WhatIs (name);

        //    if (ty == SymbolicNameTypes.Unknown) ty = PLLibrary.LibraryManager.WhatIs (name);


        //    if      (ty == SymbolicNameTypes.Function)     return TokenType.FunctionName;
        //    else if (ty == SymbolicNameTypes.Variable)     return TokenType.VariableName;
        //    else if (ty == SymbolicNameTypes.FunctionFile) return TokenType.FunctionFile;
        //    else                                           return TokenType.Undefined; 
        //}


        //*************************************************************************************************

        static int parenthesisNesting; // nesting when token created

        static bool ParenthesisProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
            //Console.WriteLine ("Parenthesis, " + status.currentChar);

            bool accepted = false;

            // this state's entry function
            if (token == null)
            {
                token = new Token (TokenType.Parens, status.currentChar);
                parenthesisNesting = status.currentChar.ParenLevel;
                accepted = true;
            }
            else
            { 
                if (status.currentChar.ParenLevel >= parenthesisNesting)
                {
                    token.AnnotatedText.Append (status.currentChar);
                    accepted = true;
                }
                else
                {
                    tokens.Add (token);
                    token = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }

        //**************************************************************************************************

        static int bracketNesting; // nesting when token created

        static bool BracketProcessing (List<IToken> tokens, ref IToken token, ParsingStatus status)
        {
         //   Console.WriteLine ("Bracket, " + status.currentChar);

            bool accepted = false;

            // this state's entry function
            if (token == null)
            {
                token = new Token (TokenType.Brackets, status.currentChar);
                bracketNesting = status.currentChar.BracketLevel;
                accepted = true;
            }
            else
            { 
                if (status.currentChar.BracketLevel >= bracketNesting)
                {
                    token.AnnotatedText.Append (status.currentChar);
                    accepted = true;
                }
                else
                {
                    tokens.Add (token);
                    token = null;
                    status.state = ParsingState.Between;
                }
            }

            return accepted;
        }
    }
}
