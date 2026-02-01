
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
        Numeric,        // 123		        
        Alphanumeric,   // used until complete token read and identified
        Parens,         // (a, b)
        Brackets,       // declare a matrix or vector
        String,
        Operator,       // +, -, etc.
        Transpose,
        TwoCharOperator,
        SupressOutput,  // trailing semicolon

        // possibly revised to one of these on second pass
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
    };

    public enum TokenPairType
    {
        Function,
        Submatrix,
    };
}
