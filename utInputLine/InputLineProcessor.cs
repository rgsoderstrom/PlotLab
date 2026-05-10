
/*
    InputLineProcessor.cs - unit test version
*/

using System;
using System.Collections.Generic;
using System.Reflection;

using PLFileSystem;
using PLLibrary;
using PLWorkspace;

namespace PLMain
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

        //
        // Passed a raw string entered by user or read from a .m file
        //  - may contain prompt, a comment and extra spaces
        //

        //public void ClassifyInputLine (string str, ref AnnotatedStringClassifier classifiedStatements)
        //{
        //    // remove prompt, comments and extra spaces
        //    string cleaned = PreprocessInputLine (str);

        //    // comment-only lines
        //    if (cleaned.Length == 0)
        //        return;

        //    // annotate entire input line
        //    classifiedStatements = new ClassifiedStringSet (cleaned);





        //    for (int i=0; i<classifiedStatements.Count; i++)
        //    {
        //        AnnotatedString statement = classifiedStatements [i];

                //if (statement.AlphanumericOnly)
                //{
                //    string firstWord = statement.FirstWord;
                //    string arguments = statement.ArgumentString;

                //    if (arguments.Length == 0) // command only, no arguments
                //    {
                //        if      (FileSystem.WhatIs  (firstWord) == FileTypes.ScriptFile) statementTypes [i] = InputLineType.Script;
                //        else if (Workspace.Contains (firstWord))                         statementTypes [i] = InputLineType.VariableName;
                //   //     else if ()
                //    }

                //    else
                //    {

                //    }

                //}

                //else
                //{

                //}

                //if (AS.WordCount == 1)
                //{
                //    if (PLLibrary.LibraryManager.Contains )
                //}
        //    }



        //}

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
