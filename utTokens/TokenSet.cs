
/*
    TokenSet - list of Tokens and any properties common to all
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class TokenSet : IEnumerable
    {
        private List<IToken> tokens = new List<IToken> ();
        public int Count {get {return tokens.Count;}}

        private bool suppessPrinting = false;
        public  bool SuppressPrinting {get {return suppessPrinting;} set {suppessPrinting = value;}}

        //**************************************************************************

        // ctors

        public TokenSet ()
        {
        }

        //**************************************************************************

        public void Add (IToken tok)
        {
            tokens.Add (tok);
        }

        //**************************************************************************

        public int FindIndex (int start, TokenType targetType)
        {
            return tokens.FindIndex (start, delegate (IToken tok) {return tok.Type == targetType;});
        }

        //*******************************************************************
        //
        // Indexer
        //
        public IToken this [int index]
        {
            get
            {
                if (index >= 0 && index < tokens.Count)
                    return tokens [index];

                throw new IndexOutOfRangeException ("Index is out of range in TokenSet indexer get.");
            }

            set
            {
                if (index >= 0 && index < tokens.Count)
                    tokens [index] = value;

                else
                    throw new IndexOutOfRangeException ("Index is out of range in TokenSet indexer set.");
            }
        }

        //*******************************************************************
        //
        // Enumeration
        //

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator ();
        }

        public TokenSetEnum GetEnumerator ()
        {
            return new TokenSetEnum (tokens);
        }

        //*******************************************************************
        //
        // ToString
        //

        public override string ToString ()
        {
            string str = "";

            str += "SuppressPrinting = " + SuppressPrinting.ToString () + "\n";
            str += "\n" + Count + " tokens" + "\n";

            foreach (IToken tok in tokens)
                str += tok.ToString () + "\n";

            return str;
        }

    }

    //**************************************************************************
    //
    // TokenSetEnum - used by TokenSet iterator
    //

    public class TokenSetEnum : IEnumerator
    {
        public List<IToken> _tokens;

        int position = -1;

        public TokenSetEnum (List<IToken> lst)
        {
            _tokens = lst;
        }

        public bool MoveNext ()
        {
            position++;
            return (position < _tokens.Count);
        }

        public void Reset ()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public IToken Current
        {
            get {return _tokens [position];}
        }
    }
}
