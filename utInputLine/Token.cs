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

    //***************************************************************************************************

    public class Token : IToken
    {
        // private storage
	    private          TokenType       type;
	    private readonly AnnotatedString annotatedText;

        // public properties
        public TokenType Type {get {return type;} set {type = value;}}
        public AnnotatedString AnnotatedText {get {return annotatedText;}}

        // constructors
        public Token (TokenType ty, AnnotatedChar   txt) {type = ty; annotatedText = new AnnotatedString (txt);}
        public Token (TokenType ty, AnnotatedString txt) {type = ty; annotatedText = txt;}

        // ToString
        public override string ToString () {return string.Format ("Token type: {0}, Token Text: {1}", type, annotatedText.Raw);}
    }

    //***************************************************************************************************

    public class TokenPair : IToken
    {
        // private storage
	    private          TokenPairType   pairType;  // this will be Submatrix or Function
	    private readonly AnnotatedString annotatedText;

        private readonly IToken t1;
        private readonly IToken t2;

        // public properties
        public TokenType     Type     {get {return TokenType.Pair;} set {;}} // "set" is a nop but is required by interface IToken
        public TokenPairType PairType {get {return pairType;} set {pairType = value;}}

        public IToken Get1 {get {return t1;}}
        public IToken Get2 {get {return t2;}}

        public AnnotatedString AnnotatedText {get {return annotatedText;}}

        // constructors
        public TokenPair (TokenPairType ty, IToken tok1, IToken tok2) 
        {
            pairType = ty; 
            t1 = tok1;
            t2 = tok2;
        }

        // ToString
        public override string ToString () {return string.Format ("Token pair type: {0}, Token Text: {1}", pairType, t1.AnnotatedText.Raw + " " + t2.AnnotatedText.Raw);}
        
    }
}









