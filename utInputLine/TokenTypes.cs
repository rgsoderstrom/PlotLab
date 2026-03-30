
/*
    TokenTypes.cs
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    //*************************************************************************************

    public enum TokenType
    {
        // assigned during first pass
        Numeric,         // 123		        
        Alphanumeric,    // used until complete token read and identified
        Parens,          // (a, b)
        Brackets,        // declare a matrix or vector
        String,
        Operator,        // +, -, etc.
        EqualSign,
        Transpose,
        TwoCharOperator,
        SupressPrinting, // trailing semicolon

        // possibly revised to one of these on second pass
        BinaryOperator,
        UnaryOperator,
        VariableName,
        FunctionName,    // built-in or .m file
        ScriptFile,
        Undefined,       // on LHS a new variable, on RHS will be an error

        GroupingParens,  // A * (B + C)
        FunctionParens,  // Func1 (P, Q, R, S)
        SubmatrixParens, // ZMat (Rs, Cs); % (row select, col select)

        BracketsColon,  // [A : B : C] or [A : B]
        BracketsSemi,   // [a ; b ; c ; d] or [1 2 3 ; 4 5 6]
        BracketsComma,  // [1, 2, 3]
        BracketsSpace,  // [1 2 3]

        Pair, // for class TokenPair
        None, // used when stepping through list of tokens. Token before the first or after the last is assigned type "None"
    };

    public enum TokenPairType
    {
        Function,
        Submatrix,
    };
}
