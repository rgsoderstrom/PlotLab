using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLKernel
{
    internal static partial class TokenUtils
    {
        static readonly List<char>   oneCharBinaryOperators = new List<char> ()   {'^', '*', '/', '+', '-', '=', '&', '|', '>', '<', '~'};
        static readonly List<string> twoCharBinaryOperators = new List<string> () {".*", "./", ".^", "&&", "||", "~=", "==", ">=", "<="};
        static readonly List<char>   unaryOperators         = new List<char> ()   {'+', '-', '~'};

        static public bool IsTwoCharOperator (string str) {return twoCharBinaryOperators.Contains (str);}

        static public bool IsBinaryOp (char c)   {return oneCharBinaryOperators.Contains (c);}
        static public bool IsBinaryOp (string s) {if (s.Length == 1) return IsBinaryOp (s [0]); if (s.Length == 2) return IsTwoCharOperator (s); return false;}  

        static public bool IsDecimal      (char c) {return c == '.';}
        static public bool IsAlpha        (char c) {return Char.IsLetter (c) || c == '_';}
        static public bool IsNumber       (char c) {return Char.IsDigit (c); }
        static public bool IsWhitespace   (char c) {return (c == ' ' || c == '\t');}
        static public bool IsOpenParen    (char c) {return c == '(';}
        static public bool IsCloseParen   (char c) {return c == ')';}
        static public bool IsOpenBracket  (char c) {return c == '[';}
        static public bool IsCloseBracket (char c) {return c == ']';}

        static public bool IsEqualSign    (char c)   {return c == '=';}
        static public bool IsEqualSign    (string c) {return (c.Length == 1) && IsEqualSign (c [0]);}
        


        static public bool IsSingleQuote  (char c)   {return c == '\'';} // string delimiter
        static public bool IsTranspose    (char c)   {return c == '\'';}
        static public bool IsTranspose    (string c) {return (c.Length == 1) && IsTranspose (c [0]);}



        static public bool IsExponent (char c)   {return c == '^'; }
        static public bool IsExponent (string c) {if (c.Length == 1) return IsExponent (c [0]); if (c.Length == 2) return IsDecimal (c [0]) && IsExponent (c [1]); return false;}

        static public bool IsColon (char c) {return c == ':';}
        static public bool IsColon (string c) {return c.Length == 1 && IsColon (c [0]);}

        static public bool IsSemicolon (char c)   {return c == ';';}
        static public bool IsSemicolon (string c) {return c.Length == 1 && IsSemicolon (c [0]);}

        static public bool IsComma        (char c) {return c == ',';}

        static public bool IsOperator (char c) { return IsTranspose (c) || IsColon (c) || IsSemicolon (c) || IsComma (c) || IsDecimal (c) || IsBinaryOp (c); }  

        static public bool IsUnaryOp (char c)   {return unaryOperators.Contains (c);}
        static public bool IsUnaryOp (string s) {if (s.Length > 1) return false; return unaryOperators.Contains (s [0]);}

        //***********************************************************************************************************************************
        //***********************************************************************************************************************************
        //***********************************************************************************************************************************

        class BinaryOperatorPriority
        {
            public readonly string binaryOperator;
            public readonly int    priority;   // higher number => higher priority

            public BinaryOperatorPriority (string o, int p)
            {
                binaryOperator = o;
                priority = p;
            }
        }

        // high number => high priority

        static readonly List<BinaryOperatorPriority> prioritys = new List<BinaryOperatorPriority> () {new BinaryOperatorPriority ("^",   800),
                                                                                                      new BinaryOperatorPriority (".^",  800),

                                                                                                      new BinaryOperatorPriority (",",   600),
                                                                                                      new BinaryOperatorPriority (";",   600),
                                                                                                      new BinaryOperatorPriority ("*",   500),
                                                                                                      new BinaryOperatorPriority (".*",  500),
                                                                                                      new BinaryOperatorPriority ("/",   500),
                                                                                                      new BinaryOperatorPriority ("./",  500),
                                                                                                      new BinaryOperatorPriority ("+",   400),
                                                                                                      new BinaryOperatorPriority ("-",   400),
                                                                                                      new BinaryOperatorPriority (":",   300),
                                                                                                      new BinaryOperatorPriority ("=",   100),                                                                                                      new BinaryOperatorPriority ("==",   70),

                                                                                                      new BinaryOperatorPriority ("~=",   70),
                                                                                                      new BinaryOperatorPriority ("==",   70),
                                                                                                      new BinaryOperatorPriority ("<=",   70),
                                                                                                      new BinaryOperatorPriority (">=",   70),
                                                                                                      new BinaryOperatorPriority ("<",    70),
                                                                                                      new BinaryOperatorPriority (">",    70),

                                                                                                      new BinaryOperatorPriority ("&",    50),
                                                                                                      new BinaryOperatorPriority ("|",    50),
                                                                                                      new BinaryOperatorPriority ("&&",   50),
                                                                                                      new BinaryOperatorPriority ("||",   50),
                                                                                                      };
        static public int GetBinOpPriority (string op)
        {
            int priority = -1;

            foreach (BinaryOperatorPriority bop in prioritys)
            {
                if (op == bop.binaryOperator)
                {
                    priority = bop.priority;
                    break;
                }
            }

            if (priority == -1)
                throw new Exception ("Binary operator priority not found");

            return priority;
        }

        //***********************************************************************************************************************************
        //***********************************************************************************************************************************
        //***********************************************************************************************************************************

        public class BracketSeparatorPriority
        {
            public readonly char oper;
            public readonly int  priority;   // higher number => higher priority
            public readonly TokenType tokenType;

            public BracketSeparatorPriority (char o, int p, TokenType ty)
            {
                oper = o;
                priority = p;
                tokenType = ty;
            }
        }

        // high number => high priority

        public static readonly List<BracketSeparatorPriority> bracketPrioritys = new List<BracketSeparatorPriority> () 
        {
            new BracketSeparatorPriority (' ', 800, TokenType.BracketsSpace),
            new BracketSeparatorPriority (',', 600, TokenType.BracketsComma),
            new BracketSeparatorPriority (':', 600, TokenType.BracketsColon),
            new BracketSeparatorPriority (';', 400, TokenType.BracketsSemi),
        };

        public static readonly List<char> bracketSeparators = new List<char> {' ', ',', ':', ';'};

        public static BracketSeparatorPriority GetBspForOperator (char op)
        {
            foreach (TokenUtils.BracketSeparatorPriority bsp in TokenUtils.bracketPrioritys)
            {
                if (op == bsp.oper)
                {
                    return bsp;
                }
            }

            throw new Exception ("Bracket separator not found");
        }
    }
}
