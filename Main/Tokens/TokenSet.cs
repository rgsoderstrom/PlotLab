
/*
    TokenSet - list of Tokens and any properties common to all
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace PLMain
{
    public class TokenSet : IEnumerable
    {
        private List<IToken> tokens = new List<IToken> ();
        private string name = "";

        public int    Count {get {return tokens.Count;}}
        public string Name  {get {return name;} protected set {name = value;}}

        //**************************************************************************

        // ctors

        public TokenSet (string str)
        {
            name = str;
        }

        public TokenSet () : this ("Unknown")
        {
        }

        //**************************************************************************

        public void Add (IToken tok)
        {
            tokens.Add (tok);
        }

        //**************************************************************************

        //public TokenSet DeepCopy (TokenSet src)
        //{
        //    TokenSet newSet = new TokenSet ();

        //    foreach (IToken tok in src)
        //    {
        //        if (tok is Token)
        //        { 
        //            Token tokCopy = new Token (tok.Type, new AnnotatedString (tok.AnnotatedText.Plain));
        //            newSet.Add (tokCopy);
        //        }

        //        else if (tok is TokenPair)
        //        {
        //            TokenPair origPair = tok as TokenPair;
        //            Token t1 = new Token (origPair.Get1.Type, new AnnotatedString (origPair.Get1.AnnotatedText.Plain));
        //            Token t2 = new Token (origPair.Get2.Type, new AnnotatedString (origPair.Get2.AnnotatedText.Plain));

        //            TokenPair copyPair = new TokenPair (origPair.PairType, t1, t2);
        //            newSet.Add (copyPair);
        //        }
        //    }

        //    return newSet;
        //}

        //**************************************************************************

        //public bool Contains (TokenType targetType)
        //{
        //    foreach (IToken itok in tokens)
        //        if (itok.Type == targetType)
        //            return true;

        //    return false;
        //}

        //**************************************************************************

        private int FindIndex (int start, TokenType targetType)
        {
            return tokens.FindIndex (start, delegate (IToken tok) {return tok.Type == targetType;});
        }

        // search entire list for a given type

        public List<int> FindTokenTypeIndices (TokenType targetType)
        { 
            List<int> indices = new List<int> ();

            int start = 0;

            while (start < Count)
            {
                int index = FindIndex (start, targetType);

                if (index == -1)
                    break;

                indices.Add (index);
                start = index + 1;
            }

            return indices;
        }

        // search a subset of the list for a given type

        public List<int> FindPairTypeIndices (List<int>     tokenPairs, // look at these indices
                                              TokenPairType targetType) // for this type
        {
            List<int> indices = new List<int> ();

            foreach (int i in tokenPairs)
                if ((tokens [i] as TokenPair).PairType == targetType)
                    indices.Add (i);

            return indices;
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

            str += Name + ", " + Count + " tokens " + "\n";

            foreach (IToken tok in tokens)
                str += tok.ToString () + " \n";

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
