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
	    public TokenType       type;
	    public AnnotatedString annotatedText;

        public Token (TokenType ty, AnnotatedChar   txt) {type = ty; annotatedText = new AnnotatedString (txt);}
        public Token (TokenType ty, AnnotatedString txt) {type = ty; annotatedText = txt;}

        public override string ToString () {return string.Format ("Token type: {0}, Token Text: {1}", type, annotatedText.Raw);}
    }
}









