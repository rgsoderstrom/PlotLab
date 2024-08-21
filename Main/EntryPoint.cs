using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PLCommon;
using PLWorkspace;

namespace Main
{
    public class EntryPoint
    {
        static public bool ShowParsingTokens = false;
        static public bool ShowExprTree      = false;
        static int Counter = 0;

        public void ProcessArithmeticExpression (ref PLVariable answer, string expression, Workspace workspace, PrintFunction pf)
        {
            try
            {
                if (ShowParsingTokens || ShowExprTree) Counter++;

                if (ShowParsingTokens)
                {
                    // this runs a copy of the actual parsing code
                    TokenParsing parsing = new TokenParsing ();
                    Window win = new Window ();
                    TextBox tb = new TextBox ();

                    // first pass
                    List<Token> tokens = parsing.ParsingPassOne (expression);
                    tb.Text += "First pass:\n";
                    foreach (Token tok in tokens) tb.Text += tok.ToString () + "\n";

                    // second pass
                    tokens = parsing.ParsingPassTwo (tokens, workspace);
                    tb.Text += "\nSecond pass:\n";
                    foreach (Token tok in tokens) tb.Text += tok.ToString () + "\n";

                    win.Content = tb;
                    win.Title = "Parsing " + Counter;
                    win.Width = 400;
                    win.Height = 300;
                    win.Show ();
                }

                ExpressionTree tree = new ExpressionTree (expression, workspace, pf);

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
