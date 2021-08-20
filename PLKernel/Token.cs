using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLKernel
{
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
        Decimal,


        // revised on second pass
        VariableName, 
        FunctionName, 
        FunctionFile, 
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

    //***************************************************************************************************

    public class Token
    {
	    public TokenType type;
	    public string    text;

        public Token (TokenType ty, string txt) {type = ty; text = txt;}

        public Token () : this (TokenType.None, null) { }

        public Token (TokenType ty, char txt) : this (ty, new string (txt, 1)) { }

        public override string ToString () {return string.Format ("Type: {0}: {1}", type, text);}
    }

    //***************************************************************************************************

    public class TokenPair : Token
    {
        public TokenPairType pairType;
        public Token t0;
        public Token t1;

        public TokenPair () {type = TokenType.Pair; text = "";}

        public override string ToString ()
        {
            string str = "Pair: [" + t0.ToString () + ", " + t1.ToString () + "]";
            return str;
        }
    }
}









