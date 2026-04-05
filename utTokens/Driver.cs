using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

using PLLibrary;

namespace utTokens
{
    internal class Driver
    {
     // static readonly string InputMFileName = @"..\..\..\TokenUtilsTests.m";
        static readonly string InputMFileName = @"..\..\..\Examples\TokenTests.m";

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        static void Main (string [] args)
        {
            try
            {
                StreamReader inputFile = new StreamReader (InputMFileName);
                string raw;

                while ((raw = inputFile.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        bool ps = false; // print separator
                        ps |= AnnotatedStringTest (raw);
                        //ps |= TokenParsingTest (raw);
                        //pr |= TokenUtilsTest (raw);
                        
                        if (ps) Print ("===========================================");                    }
                }

                inputFile.Close ();
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool AnnotatedStringTest (string str)
        {
            string text = InputLineProcessor.RemovePromptAndComments (str);

            if (text.Length == 0)
                return false;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.Plain.ToString ());
            Print (annotated.ToString ());
     
         //   AnnotatedString a2 = annotated.RemoveWrapper ();
         //   Print (a2.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool TokenParsingTest (string raw)
        {
            string text = InputLineProcessor.RemovePromptAndComments (raw);

            if (text.Length == 0)
                return false;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.Plain.ToString ());

            //
            // pass each annotated string to token processor
            //
            TokenParsing parser = new TokenParsing ();
            List<IToken> statementtokens = parser.StringToTokens (annotated);

            foreach (IToken tok in statementtokens)
                Print (tok.ToString ());

            return true;
        }

        //***********************************************************************
        //***********************************************************************
        //***********************************************************************

        static private bool TokenUtilsTest (string raw)
        {
            TokenParsing parsing = new TokenParsing ();

            string text = InputLineProcessor.RemovePromptAndComments (raw);

            if (text.Length == 0)
                return false;

            text = InputLineProcessor.SqueezeConsecutiveSpaces (text);
            AnnotatedString annotated = new AnnotatedString (text);

            Print (annotated.ToString () + "\n");
            List<AnnotatedString> fargs = parsing.SplitBracketArgs_Space (annotated);

            foreach (AnnotatedString AS in fargs)
                Print (AS.ToString () + "\n");
 
            return true;
        }
    }
}
