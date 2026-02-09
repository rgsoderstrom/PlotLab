
/*
    InputLineProcessor.cs - unit test version
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Main
{
    public delegate void PrintFunction (string str);

    internal partial class InputLineProcessor
    {
        IWorkspace    workspace;
        ILibrary      library;
        IFileSystem   files;
        PrintFunction Print;

        internal InputLineProcessor (IWorkspace ws, ILibrary lib, IFileSystem fs, PrintFunction pr)
        {
            workspace = ws;
            library = lib;
            files = fs;
            Print = pr;
        }

        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************

        public List<List<IToken>> ParseOneInputLine (string inputLine)
        {
            // One List<Token> for each input statement. Since a line can contain
            // more than one statement, we may need to return more than one list
            List<List<IToken>> TokenLists = new List<List<IToken>> ();

            try
            { 
                string text = RemovePromptAndComments (inputLine);

                if (text.Length == 0)
                    return TokenLists;

                text = SqueezeConsecutiveSpaces (text);
                AnnotatedString annotated = new AnnotatedString (text);

                // Split compound lines like:
                // a = 1; b = 2; c = 3;
                // into individual statements, while leaving lines like:
                // a = [1 ; 2 ; 4];
                // as a single statement
                List<AnnotatedString> annotated2 = annotated.SplitAtLevel0Semicolon ();

                //
                // pass each annotated string to token processor
                //
                TokenParsing parser = new TokenParsing ();

                foreach (AnnotatedString annotatedText in annotated2)
                {
                    //Print ("----------------------------------------");

                    //Console.WriteLine (annotatedText);
                    //Console.WriteLine ();

                    List<IToken> statementTokens = parser.StringToTokens (annotatedText, workspace, library, files);
                    TokenLists.Add (statementTokens);

                    //Print (text);

                    //foreach (Token tok in statementTokens)
                    //    Print (tok.ToString ());
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception in ParseOneInputLine: " + ex.Message);
            }

            return TokenLists;
        }
    }
}
