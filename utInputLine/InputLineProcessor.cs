
/*
    InputLineProcessor.cs - unit test version
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using PLFileSystem;
using PLLibrary;
using PLWorkspace;

namespace Main
{
    public delegate void PrintFunction (string str);

    public partial class InputLineProcessor
    {
        static PrintFunction Print;

        public InputLineProcessor (PrintFunction pr)
        {
            Print = pr;
        }

        public InputLineProcessor ()
        {
        }

        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************

        // Passed a string entered by user or read from a .m file
        //
        // returns parallel lists of line types and annotated strings
        //
        public void IdentifyInputLine (string str, 
                                       ref List<LineType> statementTypes, 
                                       ref List<AnnotatedString> individualStatements)
        {
            statementTypes.Clear ();
            individualStatements.Clear ();

            // remove prompt, comments and extra spaces
            string cleaned = PreprocessInputLine (str);

            // annotate entire input line
            AnnotatedString annotated = new AnnotatedString (cleaned);

            // split a line containing several statements into serparate lines
            individualStatements = annotated.SplitAtLevel0Semicolon ();

            // initial statement type of Unknown for each statement
            for (int i=0; i<individualStatements.Count; i++)
                statementTypes.Add (LineType.Unknown);

            for (int i=0; i<individualStatements.Count; i++)
            {
                AnnotatedString statement = individualStatements [i];

                if (statement.AlphanumericOnly)
                {
                    string firstWord = statement.FirstWord;
                    string arguments = statement.ArgumentString;

                    if (arguments.Length == 0) // no arguments
                    {
                        if      (FileSystem.WhatIs (firstWord) == FileTypes.ScriptFile) statementTypes [i] = LineType.Script;
                        else if (Workspace.Contains (firstWord)) statementTypes [i] = LineType.VariableName;
                   //     else if ()
                    }

                    else
                    {

                    }

                }

                else
                {

                }

                //if (AS.WordCount == 1)
                //{
                //    if (PLLibrary.LibraryManager.Contains )
                //}
            }



        }

        //public List<List<IToken>> ParseOneInputLine (string inputLine)
        //{
        //    // One List<Token> for each input statement. Since a line can contain
        //    // more than one statement, we may need to return more than one list
        //    List<List<IToken>> TokenLists = new List<List<IToken>> ();

        //    try
        //    { 
        //        string text = RemovePromptAndComments (inputLine);

        //        if (text.Length == 0)
        //            return TokenLists;

        //        text = SqueezeConsecutiveSpaces (text);
        //        AnnotatedString annotated = new AnnotatedString (text);

        //        // Split compound lines like:
        //        // a = 1; b = 2; c = 3;
        //        // into individual statements, while leaving lines like:
        //        // a = [1 ; 2 ; 4];
        //        // as a single statement
        //        List<AnnotatedString> annotated2 = annotated.SplitAtLevel0Semicolon ();

        //        //
        //        // pass each annotated string to token processor
        //        //
        //        TokenParsing parser = new TokenParsing ();

        //        foreach (AnnotatedString annotatedText in annotated2)
        //        {
        //            //Print ("----------------------------------------");

        //            //Console.WriteLine (annotatedText);
        //            //Console.WriteLine ();

        //            List<IToken> statementTokens = parser.StringToTokens (annotatedText);
        //            TokenLists.Add (statementTokens);

        //            //Print (text);

        //            //foreach (Token tok in statementTokens)
        //            //    Print (tok.ToString ());
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        Console.WriteLine ("Exception in ParseOneInputLine: " + ex.Message);
        //    }

        //    return TokenLists;
        //}
    }
}
