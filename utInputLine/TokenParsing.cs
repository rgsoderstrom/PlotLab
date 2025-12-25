using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using PLCommon;
//using PLWorkspace;

namespace Main
{
    public partial class TokenParsing
    {
        private enum ParsingState
        {
            Between,
            InNumber,
            InAlphanumeric,  // a variable or a function
            Decimal,
            InString,
            InBrackets,
            InParenthesis,
            InOperator,
            LookForTranspose,
            Leaving,
            Error
        };

        private class ParsingStatus
        {
            public ParsingState state = ParsingState.Between;
            public char currentChar = '\0';
        }

        //*****************************************************************************************************

        public List<Token> StringToTokens (AnnotatedString expression, IWorkspace workspace, ILibrary lib, IFileSystem files)
        {
            List<Token> tokens = ParsingPassOne (expression);
            tokens = ParsingPassTwo (tokens, workspace, lib, files);
            return tokens;
        }

        //*****************************************************************************************************

        //
        // ParsingPassOne
        //

        internal List<Token> ParsingPassOne (AnnotatedString expression)
        {
            List<Token> tokens = new List<Token> ();
            Token CurrentToken = null;

            //
            // catch some obvious errors
            //
            if (expression == null)
                return tokens; // empty list

            if (expression.Length == 0)
                return tokens; // empty list

            //
            // Parse expression
            //

            ParsingStatus status = new ParsingStatus ();

            bool done = false;
            int get = 0;  // next character index

            while (done == false)
            {
                if (status.currentChar == '\0')
                {
                    if (get < expression.Length)
                    {
                        status.currentChar = expression [get++].Character;
                    }

                    else if (get == expression.Length)
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

                    case ParsingState.Between:
                        BetweenProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InString:
                        StringProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InOperator:
                        OperatorProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.Decimal:
                        DecimalProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InNumber:
                        NumberProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InAlphanumeric:
                        AlphanumericProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InParenthesis:
                        ParenthesisProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.InBrackets:
                        BracketProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.LookForTranspose:
                        LookForTransposeProcessing (tokens, ref CurrentToken, status);
                        break;

                    case ParsingState.Error:
                        if (CurrentToken == null) throw new Exception (string.Format ("Parsing error"));
                        if (CurrentToken.text != null) throw new Exception (string.Format ("Parsing error: {0}, {1}", status.currentChar, CurrentToken.text));
                        else throw new Exception (string.Format ("Parsing error: {0}", status.currentChar));

                    default:
                        throw new Exception ("Parsing state error");
                }
            }

            return tokens;
        }

        //*************************************************************************************************

        static void ExitProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            if (current != null)
            {
                tokens.Add (current);
                current = null;
            }
        }

        //*************************************************************************************************

        static void LookForTransposeProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            if (TokenUtils.IsSingleQuote (status.currentChar))
            {
                Token tok = new Token (TokenType.TransposeOperator, status.currentChar);
                tokens.Add (tok);
                status.currentChar = '\0';
            }

            else
                status.state = ParsingState.Between;
        }

        //*************************************************************************************************

        static void BetweenProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            if (TokenUtils.IsWhitespace (status.currentChar)) status.currentChar = '\0';

            else if (TokenUtils.IsDecimal (status.currentChar)) status.state = ParsingState.Decimal;

            else if (TokenUtils.IsSingleQuote (status.currentChar)) status.state = ParsingState.InString;

            else if (TokenUtils.IsOperator (status.currentChar)) status.state = ParsingState.InOperator;

            else if (TokenUtils.IsAlpha (status.currentChar)) status.state = ParsingState.InAlphanumeric;

            else if (TokenUtils.IsNumber (status.currentChar)) status.state = ParsingState.InNumber;

            else if (TokenUtils.IsOpenParen (status.currentChar)) status.state = ParsingState.InParenthesis;

            else if (TokenUtils.IsOpenBracket (status.currentChar)) status.state = ParsingState.InBrackets;

            else
                status.state = ParsingState.Error;
        }

        //*************************************************************************************************

        static void StringProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // this state's entry function
            if (current == null)
            {
                current = new Token (TokenType.String, status.currentChar);
                status.currentChar = '\0';
                return;
            }

            current.text += status.currentChar;

            bool escaped = current.text.Length > 2 ? (current.text [current.text.Length - 2] == '\\' ? true : false) : false;

            // test exit criteria
            if (TokenUtils.IsSingleQuote (status.currentChar) && escaped == true)
            {
                current.text = current.text.Remove (current.text.Length - 2, 1);
            }
            
            else if (TokenUtils.IsSingleQuote (status.currentChar) && escaped == false)
            {
                status.state = ParsingState.Between;
                tokens.Add (current);
                current = null;
            }

            status.currentChar = '\0';
        }

        //*************************************************************************************************

        static void DecimalProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // this state's entry function
            if (current == null)
            {
                current = new Token (TokenType.Decimal, status.currentChar);
                status.currentChar = '\0';

                status.state = ParsingState.Between;  // immediately exit this state
                tokens.Add (current);
                current = null;
                return;
            }
        }

        //*************************************************************************************************

        static void OperatorProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // every operator char its own token
            current = new Token (TokenType.ArithmeticOperator, status.currentChar);
            status.currentChar = '\0';
            tokens.Add (current);
            current = null;
            status.state = ParsingState.Between;



            // this state's entry function
            //if (current == null)
            //{
            //    current = new Token (TokenType.ArithmeticOperator, status.currentChar);
            //    status.currentChar = '\0';
            //    return;
            //}

            //if (TokenUtils.IsOperator (status.currentChar))
            //{
            //    current.text += status.currentChar;
            //    status.currentChar = '\0';
            //}

            //else
            //{
            //    tokens.Add (current);
            //    current = null;
            //    status.state = ParsingState.Between;
            //}





            //else if (TokenUtils.IsWhitespace (status.currentChar))
            //{
            //    leavingState = true;
            //    status.state = ParsingState.Between;
            //}

            //else if (TokenUtils.IsNumber (status.currentChar))
            //{
            //    leavingState = true;
            //    status.state = ParsingState.Between;
            //}

            //else if (TokenUtils.IsAlpha (status.currentChar))
            //{
            //    leavingState = true;
            //    status.state = ParsingState.Between;
            //}

            //else if (TokenUtils.IsOpenParen (status.currentChar))
            //{
            //    leavingState = true;
            //    status.state = ParsingState.Between;
            //}

            //else if (TokenUtils.IsOpenBracket (status.currentChar))
            //{
            //    leavingState = true;
            //    status.state = ParsingState.Between;
            //}

            //else
            //    status.state = ParsingState.Error;

            //if (leavingState)
            //{
            //    if (current.text.Length > 1)
            //    {
            //        int trailing = 0;
            //        int leading = 1;

            //        while (leading<current.text.Length)
            //        {
            //            char [] twoChars = new char [] { current.text [trailing], current.text [leading] };

            //            if (TokenUtils.IsTwoCharOperator (new string (twoChars)) == true)
            //            {
            //                tokens.Add (new Token (TokenType.ArithmeticOperator, new string (twoChars)));
            //                leading += 2;
            //                trailing += 2;
            //            }
            //            else
            //            {
            //                tokens.Add (new Token (TokenType.ArithmeticOperator, current.text [trailing]));
            //                leading++;
            //                trailing++;
            //            }
            //        }

            //        if (trailing < current.text.Length)
            //            tokens.Add (new Token (TokenType.ArithmeticOperator, current.text [trailing]));

            //        current = null;
            //    }
            //}
        }

        //*************************************************************************************************

        static void NumberProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // this state's entry function
            if (current == null)
            {
                current = new Token (TokenType.Numeric, status.currentChar);
                status.currentChar = '\0';
                return;
            }

            if (TokenUtils.IsNumber (status.currentChar) || TokenUtils.IsDecimal (status.currentChar) || char.ToUpper (status.currentChar) == 'E')
            {
                current.text += status.currentChar;
                status.currentChar = '\0';
            }

            else if (status.currentChar == '-' || status.currentChar == '+')
            {
                if (char.ToUpper (current.text [current.text.Length - 1]) == 'E')
                {
                    current.text += status.currentChar;
                    status.currentChar = '\0';
                }
                else
                {
                    tokens.Add (current);
                    current = null;
                    status.state = ParsingState.Between;
                }
            }

            else
            {
                tokens.Add (current);
                current = null;
                status.state = ParsingState.Between;
            }
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


        static void AlphanumericProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // this state's entry function
            if (current == null)
            {
                current = new Token (TokenType.Alphanumeric, status.currentChar);
                status.currentChar = '\0';
                return;
            }

            if (TokenUtils.IsAlpha (status.currentChar) || TokenUtils.IsNumber (status.currentChar))
            {
                current.text += status.currentChar;
                status.currentChar = '\0';
            }

            else
            {
                tokens.Add (current);
                current = null;
                status.state =  ParsingState.LookForTranspose; // ParsingState.Between;
            }





            //else if (TokenUtils.IsWhitespace (status.currentChar))
            //{
            //    current.type = LookupNameType (current.text, workspace);

            //   // current.type = TokenType.VariableName;


            //    status.currentChar = '\0';
            //    status.state = ParsingState.Between;
            //}

            //else if (TokenUtils.IsOpenParen (status.currentChar))
            //{
            //    TokenType nameType = LookupNameType (current.text, workspace);
            //    current.type = nameType;
            //    tokens.Add (current);

            //    current = new Token ();

            //    if (nameType == TokenType.FunctionName)
            //        current.type = TokenType.FunctionParens;

            //    else if (nameType == TokenType.VariableName)
            //        current.type = TokenType.SubmatrixParens;

            //    current.text += status.currentChar;
            //    status.currentChar = '\0';
            //    status.state = ParsingState.InParenthesis;
            //    status.parenthesisNesting = 1;
            //}

            //else if (TokenUtils.IsOperator (status.currentChar) || TokenUtils.IsTranspose (status.currentChar))
            //{
            //  //  current.type = LookupNameType (current.text, workspace);
            //    status.state = ParsingState.Between;
            //}

            //else
            //    status.state = ParsingState.Error;
        }

        //*************************************************************************************************

        static int parenthesisNesting;

        static void ParenthesisProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // this state's entry function
            if (current == null)
            {
                current = new Token (TokenType.Parens, status.currentChar);
                status.currentChar = '\0';
                parenthesisNesting = 1;
                return;
            }

            current.text += status.currentChar;

            if (TokenUtils.IsOpenParen (status.currentChar))
            {
                parenthesisNesting++;
            }

            if (TokenUtils.IsCloseParen (status.currentChar))
            {
                parenthesisNesting--;

                if (parenthesisNesting == 0)
                {
                    tokens.Add (current);
                    current = null;
                    status.state =  ParsingState.LookForTranspose; // ParsingState.Between;
                }
            }

            status.currentChar = '\0';
        }

        //**************************************************************************************************

        static int bracketNesting;

        static void BracketProcessing (List<Token> tokens, ref Token current, ParsingStatus status)
        {
            // this state's entry function
            if (current == null)
            {
                current = new Token (TokenType.Brackets, status.currentChar);
                bracketNesting = 1;
                status.currentChar = '\0';
                return;
            }

            current.text += status.currentChar;

            if (TokenUtils.IsOpenBracket (status.currentChar))
            {
                bracketNesting++;
            }

            if (TokenUtils.IsCloseBracket (status.currentChar))
            {
                bracketNesting--;

                if (bracketNesting == 0)
                {
                    tokens.Add (current);
                    current = null;
                    status.state = ParsingState.LookForTranspose; // ParsingState.Between;
                }
            }

            status.currentChar = '\0';
        }
    }
}






