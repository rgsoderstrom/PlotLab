
/*
    TokenSet - list of Tokens and any properties common to all
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class TokenSet
    {
        private List<IToken> tokens = new List<IToken> ();
        public int Count {get {return tokens.Count;}}

        private bool suppessPrinting = false;
        public  bool SuppressPrinting {get {return suppessPrinting;} private set {suppessPrinting = value;}}

        //**************************************************************************

        // ctors

        public TokenSet ()
        {

        }

    }
}
