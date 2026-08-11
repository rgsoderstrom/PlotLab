using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;

namespace PLMain
{
    public class ExpressionTree
    {
        readonly ExpressionTreeNode tree;
        readonly string originalExpression;

        private bool supressPrinting = false;
        public  bool SupressPrinting {get {return supressPrinting;} private set {supressPrinting = value;}}

        static public bool ShowParsingTokens = false;
        static public bool ShowExprTree = false;
        static private int Counter = 0;

        public ExpressionTree (AnnotatedString expression)
        {
            originalExpression = expression.Plain;

            try
            { 
                expression.CheckForTrailingSemi ();
                SupressPrinting = expression.SupressPrinting;

                ExpressionTreeNode.NodeCounter = 0;
                tree = new ExpressionTreeNode (expression);
                Compact ();

                if (ShowParsingTokens == false && ShowExprTree == false)
                    return;

                Counter++;

                if (ShowParsingTokens)
                {
                    // this runs a copy of the actual parsing code
                    TokenParsing parsing = new TokenParsing ();
                    TextBox tb = new TextBox ();

                    // first pass
                    TokenSet tokens = parsing.ParsingPassOne (expression);
                    tb.Text += "First pass:\n";
                    foreach (IToken tok in tokens) tb.Text += tok.ToString () + "\n";

                    // second pass
                    tokens = parsing.ParsingPassTwo (tokens);
                    tb.Text += "\nSecond pass:\n";
                    foreach (IToken tok in tokens) tb.Text += tok.ToString () + "\n";

                    Window win = new Window
                    {
                        Content = tb,
                        Title   = "Token Parsing " + Counter,
                        Width   = 400,
                        Height  = 300
                    };

                    win.Show ();
                }

                if (ShowExprTree)
                {
                    TreeView tv = new TreeView ();
                    tv.Items.Add (BuildTreeView ());
                    Window win = new Window
                    {
                        Content = tv,
                        Title   = "Expression Tree " + Counter,
                        Width   = 400,
                        Height  = 300
                    };

                    win.Show ();
                }
            }

            catch (Exception ex)
            {
                throw new Exception ("    ExpressionTree build failed for " + originalExpression + " , " + ex.Message + "\n");
            }
        }

        //******************************************************************************

        // this is public so unit test can show tree view

        public TreeViewItem BuildTreeView ()
        {
            TreeViewItem treeView = new TreeViewItem ();

            string headerString = "";

            if (tree.Operator.Length > 0)
                headerString = tree.Operator + ", " + tree.NodeType.ToString ();

            else if (tree.ValueValid)
                headerString = tree.Value.ToString () + ", " + tree.NodeType.ToString ();

            treeView.Header = headerString;

            foreach (ExpressionTreeNode node in tree.Operands)
                node.BuildTreeView (treeView);

            treeView.ExpandSubtree ();

            return treeView;
        }

        //******************************************************************************

        // this is necessary for A:B:C expressions ouside of brackets, streamlines other trees

        void Compact ()
        {
            tree.Compact ();
        }

        //******************************************************************************

        public PLVariable Evaluate ()
        {
            PLVariable answer;

            try
            {
                answer = tree.Evaluate ();
            }

            catch (Exception ex)
            {
                throw new Exception ("    ExpressionTree evaluation failed for " + originalExpression + " , " + ex.Message + "\n");
            }

            return answer;
        }


    }
}
