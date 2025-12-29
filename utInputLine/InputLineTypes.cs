
/*
    InputLineTypes.cs
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    //
    // User input is first classified as one of these
    //
    public enum StatementTypes
    {
        Simple,     // a = b + c * sqrt (d);
        Script,     // .m file name
        Compound,   // a = 3; b = sin (a);
        Function,   // [q, w] = func (x, y);
        Procedure,  // Title ('asd');
        BlockStart, // for, while, if
        BlockEnd,   // end
        PlotCmnd,   // axis frozen  % no parens or semi
        SystemCmnd, // clear a b 
        Bang,       // !
        Partial,    // a = 3 + ...  % statement split over several lines
    };

    public class InputLineType
    {
        string text;
        StatementTypes stamementType;
        List<AnnotatedChar> Nesting; // for each character
    }

    //*************************************************************************************

    public enum TokenType
    {
        // assigned during first pass
        Numeric,        // 123		        
        Alphanumeric,   // used until complete token read and identified
        Parens,         // (a, b)
        Brackets,       // declare a matrix or vector
        String,
        ArithmeticOperator,   // +, -, etc.
        TransposeOperator,
        Decimal,        // number that begins with a leading decimal, .34


        // revised to one of these on second pass
        VariableName, 
        FunctionName, 
        FunctionFile, 
        ScriptFile, 
        Undefined,       // on LHS a new variable, on RHS will be an error

        GroupingParens,  // A * (B + C)
        FunctionParens,  // (P, Q, R, S)
        SubmatrixParens, // (Rs, Cs)

        BracketsColon,  // [A : B : C] or [A : B]
        BracketsSemi,   // [a ; b ; c ; d] or [1 2 3 ; 4 5 6]
        BracketsComma,  // [1, 2, 3]
        BracketsSpace,  // [1 2 3]
    
        Pair, // for derived class TokenPair
        None, // for default ctor
    };

    public enum TokenPairType
    {
        Function,
        Submatrix,
    };

}
