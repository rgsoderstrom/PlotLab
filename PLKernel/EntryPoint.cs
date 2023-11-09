using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PLCommon;
using PLWorkspace;

namespace PLKernel
{
    public class EntryPoint
    {
        static public bool ShowParsingTokens = false;
        static public bool ShowExprTree      = false;
        static int Counter = 1;

        public void ProcessArithmeticExpression (ref PLVariable answer, string expression, Workspace workspace)
        {
            try
            {
                if (ShowParsingTokens || ShowExprTree) Counter++;

                if (ShowParsingTokens)
                {
                    TokenParsing parsing = new TokenParsing ();
                    List<Token> passOneTokens = parsing.ParsingPassOne (expression);
                    List<Token> passTwoTokens = parsing.ParsingPassTwo (passOneTokens, workspace);

                    Window win = new Window ();
                    TextBox tb = new TextBox ();
                    tb.Text += "First pass:\n";
                    foreach (Token tok in passOneTokens) tb.Text += tok.ToString () + "\n";
                    tb.Text += "\nSecond pass:\n";
                    foreach (Token tok in passTwoTokens) tb.Text += tok.ToString () + "\n";
                    win.Content = tb;
                    win.Title = "Parsing " + Counter;
                    win.Width = 400;
                    win.Height = 300;
                    win.Show ();
                }

                ExpressionTree tree = new ExpressionTree (expression, workspace);

                if (ShowExprTree)
                {
                    Window win = new Window ();
                    TreeView tv = new TreeView ();
                    tv.Items.Add (tree.TreeView ());
                    win.Content = tv;
                    win.Title = "Tree " + Counter;
                    win.Width = 400;
                    win.Height = 300;
                    win.Show ();
                }

                answer = tree.Evaluate (workspace);
            }

            catch (Exception ex)
            {
                throw new Exception ("expression " + expression + " failed, " + ex.Message);
            }
        }
    }
}
