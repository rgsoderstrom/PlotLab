
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using Common;
using PLSystem;
using PLCommon;
using PLWorkspace;
using PLLibrary;
using PLFileSystem;

namespace PLMain
{
    public partial class UserConsole : Window
    {
        public static readonly string Prompt = "--> ";

        public UserConsole ()
        {
            InputLineProcessor.Print = Print;
            SystemFunctions.Print = Print;
            ScriptProcessor.Print = Print;
            FileSystem.Print = Print;
            Block.SetPrintFunction (Print);

            FileSystem.Open ();
            InitializeComponent ();

            TextPane.AddHandler (CommandManager.PreviewExecutedEvent, new RoutedEventHandler (CommandPreview), true);

            // get initial state of check boxes
            ExpressionTree.ShowParsingTokens = (bool) ShowParse_Checkbox.IsChecked;
            ExpressionTree.ShowExprTree      = (bool) ShowTree_Checkbox.IsChecked;


            PLSystem.SystemFunctions.UserConsoleRequests = UserConsoleRequests;
        }

        //************************************************************************

        // Requests from services (e.g. PLSystem) for UserConsole to do something

        private bool UserConsoleRequests (string str)
        {
            switch (str)
            {
                case "shutdown":
                    Window_Closed (null, null);
                    break;

                case "ClearConsole":
                    TextPane.Text = "";
                    TextPane.CaretIndex = TextPane.Text.Length;
                    caretLowerLimit     = TextPane.CaretIndex;
                    break;

                case "history":
                    string hstr = CommandLineHistory.ToString ();
                    Print (hstr);
                    break;

                default:
                    Print ("UserConsole received unrecognized request: " + str);
                    break;
            }

            return true;
        }

        //************************************************************************

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            try
            {
                string s1 = FileSystem.LogFileDir;

                EventLog.Open (FileSystem.LogFileDir + "\\Log.txt", true); // false);
                CommandLineHistory.Open ();
                LibraryManager.Print = Print;

                Print ("PlotLab, Ver. 2\n");
                Workspace.Print = Print;
                TextPane.Focus ();

                Print ("Running startup script\n");
                PLVariable ans = new PLNull ();
                InputLineProcessor ip = new InputLineProcessor (Print);
                TerminationReason a = ip.ProcessString (ref ans, "startup");


                //   SystemFunctions.UserConsoleRequests = SystemRequests;



                //        MFileFunctionMgr.CurrentDir = FileSearch.CurrentDirectory;
                //      MFileFunctionMgr.SearchPathCopy = FileSearch.GetPathCopy ();
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Startup error: " + ex.Message + "\n");
                //Print ("Startup error: " + ex.StackTrace + "\n");
            }

            PrintPrompt ();
        }

        //*****************************************************************************************

        private void Window_Closed (object sender, EventArgs e)
        {
            CommandLineHistory.Close ((bool) EditHistory.IsChecked);
            EventLog.Close ();

            Application.Current?.Shutdown ();
        }

        //*****************************************************************************************

        //
        // Handle text pasted into text pane
        //

        private void CommandPreview (object sender, RoutedEventArgs e)
        {
            if ((e as ExecutedRoutedEventArgs).Command == ApplicationCommands.Paste)
            {
                if (sender is TextBox)
                {
                    if (Clipboard.ContainsText ())
                    {
                        try
                        {
                            e.Handled = true;
                            string str = Clipboard.GetText ();
                            string [] lines = str.Split (new string [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                            //
                            // process like typed-in lines
                            //

                            for (int i = 0; i<lines.Length; i++)
                            {
                                TextPane.Text += lines [i];
                                ReturnKeyHandler ();
                            }
                        }

                        catch (Exception ex)
                        {
                            Print ("Error on \"paste\": " + ex.Message);
                        }
                    }

                    PrintPrompt ();
                }
            }
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        private void ReturnKeyHandler ()
        {
            try
            {
                CommandLineHistory.ResetIndices ();
                
                // get the last line in the text pane
                string raw = TextPane.GetLineText (TextPane.LineCount - 1);

                //
                // remove prompt if present
                //
                if (raw.Length >= Prompt.Length)
                    if (raw.Substring (0, Prompt.Length).Contains (Prompt))
                        raw = raw.Remove (0, Prompt.Length);

                if (raw.Length == 0)
                {
                    Print ("\n");
                    return;
                }

                CommandLineHistory.Add (raw);

                if (raw.EndsWith ("\n")) EventLog.Write (raw);
                else                     EventLog.WriteLine (raw);

                TextPane.Text += "\n";
                TextPane.CaretIndex = TextPane.Text.Length;
                caretLowerLimit     = TextPane.CaretIndex;

                InputLineProcessor ip = new InputLineProcessor (Print);

                /* bang
                //
                // Look for bang (i.e. !) followed by a number and maybe the letter 'p'. Number is index of command
                // to recall. It is executed unless followed by :p. If so it is just printed for editting
                //
                //if (raw [0] == '!')
                //{
                //    try
                //    {
                //        string [] tokens = raw.Split (new char [] { '!', ':' }, StringSplitOptions.RemoveEmptyEntries);
                //        int index = Convert.ToInt16 (tokens [0]);
                //        string recalled = CommandLineHistory.History [index - 1];
                //        raw = recalled;
                //        Print (raw + "\n");
                //        CommandLineHistory.Add (raw);
                //        EventLog.WriteLine (raw);

                //        if (tokens.Length > 1)
                //        {
                //            if (tokens [1] [0] == 'p')
                //            {
                //                return;
                //            }
                //            else
                //            {
                //                throw new Exception ("Only history option supported is \'p\', for Print. e.g.: !12:p");
                //            }
                //        }
                //    }

                //    catch (Exception ex)
                //    {
                //        Print ("Exception: " + ex.Message);
                //        return;
                //    }
                //}
                */

                PLVariable ans = new PLNull ();
                ip.ProcessString (ref ans, raw);

                if (ans != null && ans is PLNull == false && ans is PLCanvasObject == false && ans is PLViewportObject == false)
                {
                    ans.Name = "ans";
                    Workspace.Add (ans);

                    if (ip.SupressPrinting == false)
                    {
                        Print (ans.ToString ());
                        //Print ("\n");
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception ("Error in UserConsole ReturnKeyHandler:\n" + "  " + ex.Message);
            }
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        object TextBoxLock = new object ();

        internal void Print (string str)
        {
            lock (TextBoxLock)
            {
                TextPane.Text += str;
                TextPane.ScrollToEnd ();
                TextPane.CaretIndex = TextPane.Text.Length;
                caretLowerLimit     = TextPane.CaretIndex;
            }

            if (str.EndsWith ("\n")) EventLog.Write (str);
            else                     EventLog.WriteLine (str);
        }

        internal void EditablePrint (string str)
        {
            lock (TextBoxLock)
            {
                caretLowerLimit     = TextPane.CaretIndex;

                TextPane.Text += str;
                TextPane.ScrollToEnd ();
                TextPane.CaretIndex = TextPane.Text.Length;
            }

            //if (str.EndsWith ("\n")) EventLog.Write (str);
            //else                     EventLog.WriteLine (str);
        }

        //
        // Used by Tab Completions to append text and still allow previous text to be editted
        //
        private void EditableAppend (string str)
        {
            lock (TextBoxLock)
            {
                TextPane.Text += str;
                TextPane.ScrollToEnd ();
                TextPane.CaretIndex = TextPane.Text.Length;
            }

            if (str.EndsWith ("\n")) EventLog.Write (str);
            else                     EventLog.WriteLine (str);
        }

        //***************************************************************************************
        //

        string typedIn = "";

        private void TextPane_KeyUp (object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) return; 
            if (e.Key == Key.Down) return; 

            // get the last line in the text pane
            typedIn = TextPane.GetLineText (TextPane.LineCount - 1);

            // remove prompt if present
            if (typedIn.Length >= Prompt.Length)
                if (typedIn.Substring (0, Prompt.Length).Contains (Prompt))
                    typedIn = typedIn.Remove (0, Prompt.Length);
        }

        //***************************************************************************************
        //
        // Tab Completions
        //
        
        List<string> FindTabCompletions (string token)
        {
            List<string> lines = new List<string> ();

            if (token.Length < 2) // need at least 2 chars to search
                return lines;

            lines.AddRange (Workspace.PartialMatch       (token));
         // lines.AddRange (PLSystemFunctions.PartialMatch (token));
            lines.AddRange (FileSystem.PartialNameSearch (token));
            lines.AddRange (LibraryManager.PartialMatch  (token));

            return lines;
        }

        List<string> FindTabCompletionsInCWD (string token)
        {
            List<string> lines = new List<string> ();

            if (token.Length < 2) // need at least 2 chars to search
                return lines;

            lines.AddRange (FileSystem.PartialDirectoryNameSearch (token));

            return lines;
        }

        //***************************************************************************************
        //
        // raw input cleaned up and possibly concatenated into inputLine
        //

        int caretLowerLimit = -1;

        private void TextPane_PreviewKeyDown (object sender, KeyEventArgs e)
        {
            try
            {
                //
                // try Tab Completions
                //
                if (e.Key == Key.Tab)
                {
                    if (typedIn.Length > 1) // require at least 2 characters
                    {
                        string[] tokens = typedIn.Split (new char [] {'[', '(', '*', '-', '*', '/', ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
                        int last = tokens.Length - 1;
                        string searchToken = tokens [last];
                        
                        List<string> Completions;
                        
                        if (tokens [0] == "cd")
                            Completions = FindTabCompletionsInCWD (searchToken); // for cd we only want completions in current directory
                        else
                            Completions = FindTabCompletions (searchToken);

                        if (Completions.Count == 1)
                        {
                            // append the completion chars that the user has not already typed
                            string tabAddedChars = Completions [0].Substring (searchToken.Length);
                            EditableAppend (tabAddedChars.TrimEnd ());
                        }

                        else if (Completions.Count > 1)
                        {
                            Print ("\n\n");

                            for (int i = 0; i<Completions.Count; i++)
                                Print (Completions [i]);

                            //
                            // if all completions share any starting letters will add those in common to typedIn
                            //
                            int matchingCharCount = Completions [0].Length;

                            for (int i=1; i<Completions.Count; i++)
                            {
                                int length = Math.Min (matchingCharCount, Completions [i].Length);

                                for (int j=0; j<length; j++)
                                {
                                    if (Completions [0][j] != Completions [i][j])
                                    {
                                        matchingCharCount = j;
                                        break;
                                    }
                                }

                                if (matchingCharCount <= typedIn.Length)
                                    break;
                            }

                            // re-display what was entered plus any completion characters common to all
                            PrintPrompt ();
                            EditablePrint (typedIn);

                            if (matchingCharCount > 0)
                            { 
                                string commonCharacters = Completions [0].Substring (0, matchingCharCount);

                                // and remove the ones the user has already typed
                                string tabAddedChars = commonCharacters.Substring (searchToken.Length);

                                EditableAppend (tabAddedChars);
                            }
                        }
                    }

                    e.Handled = true;
                    return;
                }

                //
                // Ignore control keys
                //
                if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                    return;

                //
                // Up & Down Arrows - command line recall
                //
                if (e.Key == Key.Up) // "up arrow"
                {
                    e.Handled = true;

                    string str = "";
                    bool valid;

                    if (typedIn == "") valid = CommandLineHistory.StepBackward   (out str);
                    else               valid = CommandLineHistory.SearchBackward (out str, typedIn);

                    if (valid)
                    {
                        int index = TextPane.GetLastVisibleLineIndex ();
                        int iChar = TextPane.GetCharacterIndexFromLineIndex (index);

                        TextPane.Text = TextPane.Text.Substring (0, iChar + 4);
                        TextPane.CaretIndex = TextPane.Text.Length;
                        caretLowerLimit =     TextPane.Text.Length;

                        EditablePrint (str);
                    }
                }

                else if (e.Key == Key.Down) // "down" arrow
                {
                    e.Handled = true;

                    int index = TextPane.GetLastVisibleLineIndex ();
                    int iChar = TextPane.GetCharacterIndexFromLineIndex (index);

                    TextPane.Text = TextPane.Text.Substring (0, iChar + 4);
                    TextPane.CaretIndex = TextPane.Text.Length;
                    caretLowerLimit =     TextPane.Text.Length;

                    string str = "";
                    bool valid;

                    if (typedIn == "") valid = CommandLineHistory.StepForward   (out str);
                    else               valid = CommandLineHistory.SearchForward (out str, typedIn);

                    if (valid)
                    {
                        EditablePrint (str);
                    }
                }

                //
                // Left Arrow & Backspace limited to last line
                //
                else if (e.Key == Key.Left || e.Key == Key.Back)
                {
                    if (TextPane.CaretIndex <= caretLowerLimit)
                        e.Handled = true;
                    return;
                }

                //
                // Return Key - try to interpret the line
                //
                else if (e.Key == Key.Return)
                {
                    typedIn = "";
                    CommandLineHistory.ResetIndices (); 
                    e.Handled = true;
                    ReturnKeyHandler ();
                    PrintPrompt ();
                }

                else
                {
                    // allow ctrl-C to function as expected
                    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
                        return;

                    // don't allow text entry above last line
                    if (TextPane.CaretIndex < caretLowerLimit)
                    {
                        TextPane.CaretIndex = TextPane.Text.Length; 
                    }
                }
            }

            catch (Exception ex)
            {
                Print (ex.Message);
                PrintPrompt ();
            }
        }

        private void PrintPrompt ()
        {
            int caretIndex = TextPane.CaretIndex;

            // Get the 0-based line index (Row)
            int lineIndex = TextPane.GetLineIndexFromCharacterIndex (caretIndex);

            // Calculate the 0-based column index
            int lineStartIndex = TextPane.GetCharacterIndexFromLineIndex (lineIndex);
            int columnIndex = caretIndex - lineStartIndex;

            if (columnIndex == 0) Print (Prompt);
            else                  Print ("\n" + Prompt);
        }

        //****************************************************************************************************
        //****************************************************************************************************
        //****************************************************************************************************

        bool textPaneHasFocus = false;

        private void TextPane_LostFocus (object sender, RoutedEventArgs e)
        {
            textPaneHasFocus = false;
        }

        private void TextPane_GotKeyboardFocus (object sender, KeyboardFocusChangedEventArgs e)
        {
            TextPane.CaretIndex = TextPane.Text.Length;
        }

        // this ensures the caret is on the last line when window gets focus
        private void TextPane_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
        {
            if (textPaneHasFocus == false)
            {
                textPaneHasFocus = true;
                //TextPane.Select(TextPane.Text.Length, 0);
                TextPane.CaretIndex = TextPane.Text.Length;
                e.Handled = true;
            }
        }

        //****************************************************************************************************
        //****************************************************************************************************
        //****************************************************************************************************

        private void ClearConsole_Click (object sender, RoutedEventArgs e)
        {
            TextPane.Clear (); 
            PrintPrompt ();
            TextPane.Focus ();
        }

        //****************************************************************************************************

        private void ShowHistory_Click (object sender, RoutedEventArgs e)
        {
            string str = CommandLineHistory.ToString ();

            if (str.Length > 0)
            {
                Print ("\n");
                Print (str);
            }

            PrintPrompt ();
            TextPane.Focus ();
        }

        private void ClearHistory_Click (object sender, RoutedEventArgs e)
        {
            CommandLineHistory.Clear ();
            TextPane.Focus ();
        }

        //****************************************************************************************************

        private void HelpWindow_Click (object sender, RoutedEventArgs e)
        {
            PLHelpWindow.HelpWindowManager.LaunchNewHelpWindow ();
            TextPane.Focus ();
        }

        private void ShowParse_Click (object sender, RoutedEventArgs e)
        {
            ExpressionTree.ShowParsingTokens = (bool) (e.OriginalSource as CheckBox).IsChecked;
            TextPane.Focus ();
        }

        private void ShowTree_Click (object sender, RoutedEventArgs e)
        {
            ExpressionTree.ShowExprTree = (bool) (e.OriginalSource as CheckBox).IsChecked;
            TextPane.Focus ();
        }
    }
}
