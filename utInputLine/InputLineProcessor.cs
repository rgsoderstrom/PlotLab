
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

                Console.WriteLine ("===========================================\n");
                Console.WriteLine (text);



                // assign "nesting level" to each character
                NestingLevel [] CharNestingLevel = new NestingLevel [text.Length];

                CharNestingLevel [0] = new NestingLevel (text [0]);

                for (int i=1; i<text.Length; i++)
                    CharNestingLevel [i] = new NestingLevel (CharNestingLevel [i-1], text [i]);

                for (int i=0; i<text.Length; i++)
                    Console.WriteLine (CharNestingLevel [i]);

                // look for compound statements. split any into single statements





                TokenParsing parser = new TokenParsing ();

                List<Token> tokens = parser.StringToTokens (text, workspace, library, files);

                Print ("----------------------------------------");

                Print (text);

                foreach (Token tok in tokens)
                    Print (tok.ToString ());
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception in ParseOneInputLine: " + ex.Message);
            }
        }
    }
}
