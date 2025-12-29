
/*
    InputLineProcessor.cs - unit test version
*/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Main
{
    internal delegate void PrintFunction (string str);

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

        public void ParseOneInputLine (string                   inputLine,
                                       ref List<string>         statements, 
                                       ref List<StatementTypes> stmtTypes)  
        {
            try
            { 
                string text = RemovePromptAndComments (inputLine);
                text = SqueezeConsecutiveSpaces (text);

                //Console.WriteLine ("===========================================\n");

                //Console.WriteLine (text + "\n");

                AnnotatedString annotated = new AnnotatedString (text);
                //Console.WriteLine (annotated.ToString () + "\n");

                // Split lines like:
                // a = 1; b = 2; c = 3;
                // into individual statements, while leaving lines like:
                // a = [1 ; 2 ; 4];
                // as a single statement
                List<AnnotatedString> annotated2 = annotated.SplitAtLevel0Semicolon ();

                //foreach (AnnotatedString AS in annotated2)
                //    Console.WriteLine (AS);

                //
                // pass each annotated string to token processor
                //
                TokenParsing parser = new TokenParsing ();

                foreach (AnnotatedString annotatedText in annotated2)
                {
                    List<Token> tokens = parser.StringToTokens (annotatedText, workspace, library, files);

                    Print ("----------------------------------------");

                    Print (text);

                    foreach (Token tok in tokens)
                        Print (tok.ToString ());
                }



            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception in ParseOneInputLine: " + ex.Message);
            }
        }
    }
}
