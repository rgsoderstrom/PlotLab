

// Serializable class containg a list of tokens

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    [Serializable]
    internal class SerializedTokens
    {
        public readonly List<IToken> tokens; 

        public SerializedTokens (List<IToken> src)
        {
            tokens = src;
        }
    }
}
