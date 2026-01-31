using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    //***************************************************************************************************

    public interface IToken
    {
        TokenType Type {get ; set ;}
        AnnotatedString AnnotatedText {get ;}
    }

    [Serializable]
    public class Token : IToken
    {
	    private          TokenType       type;
	    private readonly AnnotatedString annotatedText;

        public TokenType Type {get {return type;} set {type = value;}}
        public AnnotatedString AnnotatedText {get {return annotatedText;}}

        public Token (TokenType ty, AnnotatedChar   txt) {type = ty; annotatedText = new AnnotatedString (txt);}
        public Token (TokenType ty, AnnotatedString txt) {type = ty; annotatedText = txt;}

        public override string ToString () {return string.Format ("Token type: {0}, Token Text: {1}", type, annotatedText.Raw);}
    }
}









