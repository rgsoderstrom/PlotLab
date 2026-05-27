
/*
    NestedSTring.cs - a "lite" version of AnnotatedChar & AnnotatedString
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLMain
{
    public class NestedChar
    {
        // private members
        private readonly char  character;
        private          sbyte bracketlevel;
        private          sbyte parenlevel;

        // public access properties
        public char  Character       {get {return character;}}
        public sbyte BracketLevel    {get {return bracketlevel;} set {bracketlevel = value;}} 
        public sbyte ParenLevel      {get {return parenlevel;}   set {parenlevel = value;}} 

        public bool IsOpenParen    {get {return character == '(';}}
        public bool IsCloseParen   {get {return character == ')';}}
        public bool IsOpenBracket  {get {return character == '[';}}
        public bool IsCloseBracket {get {return character == ']';}}
    
        //**************************************************************************
        //
        // use this ctor for single char or the first character of a string
        //
        public NestedChar (char c)
        {
            character    = c;
            bracketlevel = 0; // language requires all fields be initialized
            parenlevel   = 0;

            bracketlevel = IsOpenBracket ? (sbyte) 1 : (sbyte) 0;
            parenlevel   = IsOpenParen   ? (sbyte) 1 : (sbyte) 0;

            if (IsCloseParen) throw new Exception ("NestingLevel: paren nesting error");
            if (IsCloseBracket) throw new Exception ("NestingLevel: bracket nesting error");
        }

        //**************************************************************************
        //
        // use this ctor for subsequent characters
        //
        public NestedChar (NestedChar prev, char ch)
        {
            character = ch;

            // start by setting each level equal to previous character, then adjust as necessary
            parenlevel   = prev.parenlevel;
            bracketlevel = prev.bracketlevel;

            //********************************************************

            // check parenthesis level
            if      (IsOpenParen)  parenlevel++;
            if (prev.IsCloseParen) parenlevel--;

            //********************************************************

            // check bracket level
            if      (IsOpenBracket)  bracketlevel++;
            if (prev.IsCloseBracket) bracketlevel--;

            //********************************************************

            // check for errors
            if (bracketlevel < 0) throw new Exception ("nestinglevel: bracket nesting error");
            if (parenlevel   < 0) throw new Exception ("nestinglevel: paren nesting error");
        }
    }

    //*******************************************************************************************
    //*******************************************************************************************
    //*******************************************************************************************

    public class NestedString
    {
        // private members
        private readonly List<NestedChar> nestedChars = new List<NestedChar> ();

        //*************************************************************************
        //
        // ctors
        //
        public NestedString (string text)
        {
            if (text.Length == 0)
                return;

            try
            {
                for (int i=0; i<text.Length; i++)
                {
                    NestedChar nextNC = i > 0 ? new NestedChar (nestedChars [i-1], text [i])
                                              : new NestedChar (text [i]);
                    nestedChars.Add (nextNC);
                }
            }

            catch (Exception ex)
            {
                throw new Exception ("Error in NestedString ctor:\n" + ex.Message);
                //throw new Exception ("Error in NestedString ctor:\n" + ex.StackTrace);
            }

        }
    }
}
