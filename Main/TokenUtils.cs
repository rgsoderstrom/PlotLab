using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    internal static partial class TokenUtils
    {

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
                                                                                                 //   new BinaryOperatorPriority ("=",   100),                                                                                                      new BinaryOperatorPriority ("==",   70),

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

        //public class BracketSeparatorPriority
        //{
        //    public readonly char oper;
        //    public readonly int  priority;   // higher number => higher priority
        //    public readonly TokenType tokenType;

        //    public BracketSeparatorPriority (char o, int p, TokenType ty)
        //    {
        //        oper = o;
        //        priority = p;  // high number => high priority
        //        tokenType = ty;
        //    }
        //}

        //public static readonly List<BracketSeparatorPriority> bracketPrioritys = new List<BracketSeparatorPriority> ()
        //{
        //    new BracketSeparatorPriority (' ', 800, TokenType.BracketsSpace),
        //    new BracketSeparatorPriority (',', 600, TokenType.BracketsComma),
        //    new BracketSeparatorPriority (':', 600, TokenType.BracketsColon),
        //    new BracketSeparatorPriority (';', 400, TokenType.BracketsSemi),
        //};

     // public static readonly List<char> bracketSeparators = new List<char> {' ', ',', ':', ';'};
        public static readonly List<char> bracketSeparators = new List<char> {',', ':', ';'};

        //public static BracketSeparatorPriority GetBspForOperator (char op)
        //{
        //    foreach (TokenUtils.BracketSeparatorPriority bsp in TokenUtils.bracketPrioritys)
        //    {
        //        if (op == bsp.oper)
        //        {
        //            return bsp;
        //        }
        //    }

        //    throw new Exception ("Bracket separator not found");
        //}
    }
}
