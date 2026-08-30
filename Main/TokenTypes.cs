
/*
    TokenTypes.cs
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
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

        // possibly revised to one of these on second pass
        BinaryOperator,
        UnaryOperator,
        VariableName,
        ScriptFile,

        FunctionFile,    // .m file
        ZeroArgCommand,  // return a bool
        Undefined,       // on LHS a new variable, on RHS will be an error

        //---------------------------------------

        // any function, after first pass
        Function,        

        // revised to one of these
        ZeroArgFunction, 
        FunctionWithArgs,

        //---------------------------------------

        GroupingParens,  // A * (B + C)
        FunctionParens,  // Func1 (P, Q, R, S)
        SubmatrixParens, // ZMat (Rs, Cs); % (row select, col select)

        BracketsColon,  // [A : B : C] or [A : B]
        BracketsSemi,   // [a ; b ; c ; d] or [1 2 3 ; 4 5 6]
        BracketsComma,  // [1, 2, 3]
        BracketsSpace,  // [1 2 3]

      //SupressPrinting,
        Pair, // for class TokenPair
        None, // used when stepping through list of tokens. Token before the first or after the last is assigned type "None"
    };

    public enum TokenPairType
    {
        FunctionWithArgs,
        Submatrix,
    };
}
