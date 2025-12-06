using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
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









