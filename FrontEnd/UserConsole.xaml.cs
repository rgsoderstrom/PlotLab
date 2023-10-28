using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Input;

using Common;
using PLCommon;
using PLWorkspace;
using static FrontEnd.Utils;
using System.Net.NetworkInformation;
using System.Linq;
using System.Resources;

namespace FrontEnd
{
    public partial class UserConsole : Window
    {
        internal static UserConsole thisConsole = null;
        Workspace userWorkspace = new Workspace ();

        // record any startup error messages generated until the window is ready to display them
        List<string> StartupMessages = new List<string> ();

        public UserConsole ()
        {
            CheckDocumentDirectories ("PlotLabV1", StartupMessages);
            EventLog.Open (UserConsole.LogFileDirectory + "\\Log.txt", true); // false);
            FileSearch.Open ();
            CommandLineHistory.Open ();

            InitializeComponent ();
            thisConsole = this;

            TextPane.AddHandler (CommandManager.PreviewExecutedEvent, new RoutedEventHandler (CommandPreview), true);

            foreach (string str in StartupMessages)
                Print (str + "\n");
        }

        private void Window_Loaded (object sender, RoutedEventArgs e)
        {
            Print ("PlotLab, Ver. 1\n");
            Workspace.Print = Print;
            TextPane.Focus ();

            try
            {
                PLVariable ans = new PLNull ();
                bool fp = false;
                InputLineProcessor ip = new InputLineProcessor (userWorkspace, Print, ResumeScript_Button);
                ip.ProcessOneStatement (ref ans, "startup", ref fp);
            }

            catch (Exception ex)
            {
                Print ("Startup error: " + ex.Message + "\n");
                //Print ("Startup error: " + ex.StackTrace + "\n");
            }

            Print (Utils.Prompt);
        }

        //*****************************************************************************************

        public static string LogFileDirectory = "";
        public static string HistoryFileDirectory = "";
        public static string ScriptsDirectory = "";

        private void CheckDocumentDirectories (string baseDirName, List<string> msgs)
        {
            try
            {
                string myDocs = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);

                if (myDocs == null)
                    throw new Exception ("Can't find user's \"Documents\" folder");

                string fullPathAndName = myDocs + "\\" + baseDirName;
                ScriptsDirectory = fullPathAndName + "\\" + "Scripts";
                string startup = ScriptsDirectory + "\\startup.m";

                if (Directory.Exists (fullPathAndName) == false)
                {
                    msgs.Add ("Creating folder " + baseDirName + " in user's Documents folder");
                    Directory.CreateDirectory (fullPathAndName);
                    Directory.CreateDirectory (ScriptsDirectory);

                    StreamWriter str = File.CreateText (startup);
                    str.Write ("% startup.m - this script runs automatically on start up");
                    str.Close ();
                }

                LogFileDirectory = fullPathAndName;
                HistoryFileDirectory = fullPathAndName;
            }

            catch (Exception ex)
            {
                msgs.Add ("Startup exception: " + ex.Message);
            }
        }

        //*****************************************************************************************

        private void Window_Closed (object sender, EventArgs e)
        {
            CommandLineHistory.Close ();
            EventLog.Close ();
            Application.Current.Shutdown();            
        }

        //*****************************************************************************************

        //
        // Handle text pasted into text pane
        //

        private void CommandPreview (object sender, RoutedEventArgs e)
        {
            if ((e as ExecutedRoutedEventArgs).Command == ApplicationCommands.Paste)
            {
                if (sender is System.Windows.Controls.TextBox)
                {
                    if (Clipboard.ContainsText ())
                    {
                        try
                        {
                            e.Handled = true;
                            string str = Clipboard.GetText ();
                            string [] lines = str.Split (new string [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


                            bool A = str.Contains ("\n");
                            bool B = str.Contains ("\r");
                            int  C = lines.Length;

                            int firstScriptLine = 9999;

                            //
                            // look for if/for/while statements
                            //
                            for (int i = 0; i<lines.Length; i++)
                            {
                                string [] words = lines [i].Split (new char [] { '(', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                if (words.Length > 0)
                                {
                                    if (words [0] == "if" || words [0] == "while" || words [0] == "for")
                                    {
                                        firstScriptLine = i;
                                        break;
                                    }
                                }
                            }

                            //
                            // process like typed-in lines until if/for/while found. pass remainder to script processor
                            //
                            for (int i = 0; i<lines.Length; i++)
                            {
                                if (i == firstScriptLine)
                                    break;

                                TextPane.Text += lines [i];
                                ReturnKeyHandler ();
                            }

                            if (firstScriptLine < lines.Length)
                            {
                                List<string> scriptLines = new List<string> (lines);
                                scriptLines.RemoveRange (0, firstScriptLine);

                                // write lines to text pane w/o processing them
                                foreach (string str2 in scriptLines)
                                    TextPane.Text += str2 + '\n';

                                ScriptProcessor sp = new ScriptProcessor (userWorkspace, Print, ResumeScript_Button);
                                sp.RunScriptLines (scriptLines);
                            }

                            TextPane.CaretIndex = TextPane.Text.Length;
                        }

                        catch (Exception ex)
                        {
                            Print ("Error: " + ex.Message);
                        }
                    }

                    Print (Utils.Prompt);
                }
            }
        }

        //*****************************************************************************************
        //*****************************************************************************************
        //*****************************************************************************************

        List<Utils.InputLine> inputLines = new List<Utils.InputLine> ();
        Utils.NestingLevel nestingLevel = new Utils.NestingLevel ();

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
                if (raw.Length >= Utils.Prompt.Length)
                    if (raw.Substring (0, Utils.Prompt.Length).Contains (Utils.Prompt))
                        raw = raw.Remove (0, Utils.Prompt.Length);

                if (raw.Length == 0)
                {
                    Print ("\n"); // + Utils.Prompt);
                    return;
                }

                CommandLineHistory.Add (raw);
                EventLog.WriteLine (raw);

                TextPane.Text += "\n";
                TextPane.CaretIndex = TextPane.Text.Length;
                caretLowerLimit     = TextPane.CaretIndex;

                InputLineProcessor ip = new InputLineProcessor (userWorkspace, Print, ResumeScript_Button);



                if (raw [0] == '!')
                {
                    try
                    {
                        string [] tokens = raw.Split (new char [] { '!', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        int index = Convert.ToInt16 (tokens [0]);
                        string recalled = CommandLineHistory.History [index - 1];
                        raw = recalled;
                        Print (raw + "\n");
                        CommandLineHistory.Add (raw);
                        EventLog.WriteLine (raw);

                        if (tokens.Length > 1)
                        {
                            if (tokens [1] [0] == 'p')
                            {
                                return;
                            }
                            else
                            {
                                throw new Exception ("Only history option supported is \'p\', for Print. e.g.: !12:p");
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        Print ("Exception: " + ex.Message);
                        return;
                    }
                }





                Utils.CleanupRawInput (raw, inputLines, ref nestingLevel);

                int startIndex = 0;
                int endIndex = -1;

                for (int i=0; i<inputLines.Count; i++)
                {
                    if (inputLines [i].complete)
                    {
                        bool forcePrint = false;

                        endIndex = i;
                        string expr = "";

                        for (int j=startIndex; j<=endIndex; j++)
                            expr += inputLines [j].text;

                        PLVariable ans = new PLNull ();
                        ip.ProcessOneStatement (ref ans, expr, ref forcePrint);

                        if (ans != null && ans is PLNull == false && ans is PLCanvasObject == false && ans is PLViewportObject == false)
                        {
                            ans.Name = "ans";
                            userWorkspace.Add (ans);

                            if (forcePrint || inputLines [endIndex].printFlag)
                            {
                                Print (ans.ToString ());
                                Print ("\n");
                            }
                        }

                        startIndex = endIndex + 1;
                    }
                }

                if (endIndex != -1)
                {
                    inputLines.RemoveRange (0, endIndex + 1);
                }
            }

            catch (Exception ex)
            {
                inputLines.Clear ();
                nestingLevel = new Utils.NestingLevel ();
                throw new Exception (ex.Message);
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

            EventLog.Write (str);
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

            EventLog.Write (str);
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
            if (typedIn.Length >= Utils.Prompt.Length)
                if (typedIn.Substring (0, Utils.Prompt.Length).Contains (Utils.Prompt))
                    typedIn = typedIn.Remove (0, Utils.Prompt.Length);
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
                if (e.Key == Key.Tab)
                {
                    e.Handled = true;
                    return;
                }

                if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                    return;

                //
                // Up & Down Arrows - command line recall
                //
                if (e.Key == Key.Up) // "up arrow"
                {
                    e.Handled = true;

                    string str = "";

                    if (CommandLineHistory.StepBack (ref str, typedIn))
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

                    if (CommandLineHistory.StepForward (ref str, typedIn))
                        EditablePrint (str);
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
                    Print (Utils.Prompt);
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
                Print ("\n");
                Print (Utils.Prompt);
            }
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
            Print (Utils.Prompt);
            ClearInputLine ();
            TextPane.Focus ();
           // textPaneHasFocus = true;
        }

        //****************************************************************************************************

        private void ClearInput_Click (object sender, RoutedEventArgs e)
        {
            Print ('\n' + Utils.Prompt);
            ClearInputLine ();
            TextPane.Focus ();
           // textPaneHasFocus = true;
        }

        internal static void ClearInputLine ()
        {
            thisConsole.inputLines.Clear ();
            thisConsole.nestingLevel = new Utils.NestingLevel ();
        }

        //****************************************************************************************************

        private void ShowHistory_Click (object sender, RoutedEventArgs e)
        {
            PLVariable hist = SystemFunctions.History (new PLNull ());
            PLList lst = hist as PLList;

            Print ("\n");
            foreach (PLString str in lst)
                Print (str.ToString () + '\n');

            Print ('\n' + Utils.Prompt);
            ClearInputLine ();
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

        //****************************************************************************************************

        //bool firstClick = true;

        private void ScriptFile_TextBox_GotFocus (object sender, RoutedEventArgs e)
        {
            //if (firstClick)
            //    ScriptFile_TextBox.Text = "";
            //firstClick = false;
        }

        //****************************************************************************************************

        private void OpenScript_Button_Click (object sender, RoutedEventArgs e)
        {
        }

        //****************************************************************************************************

        private void StepScript_Button_Click (object sender, RoutedEventArgs e)
        {

        }

        //****************************************************************************************************

        private void RunScript_Button_Click (object sender, RoutedEventArgs e)
        {

        }

        //****************************************************************************************************

        private void CloseScript_Button_Click (object sender, RoutedEventArgs e)
        {

        }

        //****************************************************************************************************

        private void AbortScript_Button_Click (object sender, RoutedEventArgs e)
        {

        }

        //****************************************************************************************************

        private void ResumeScript_Button_Click (object sender, RoutedEventArgs e)
        {
            TextPane.Focus ();

            try
            {
                ResumeScript_Button.IsEnabled = ScriptProcessor.ResumePausedScript ();
            }

            catch (Exception ex)
            {
                ResumeScript_Button.IsEnabled = false;
                Print (ex.Message);
            }

            Print ("\n");
            Print (Utils.Prompt);
        }
    }
}
