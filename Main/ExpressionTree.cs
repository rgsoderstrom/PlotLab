using System;
using System.Collections.Generic;
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
        readonly ExpressionTreeNode root;

        private bool supressPrinting = false;
        public  bool SupressPrinting {get {return supressPrinting;} private set {supressPrinting = value;}}

        public ExpressionTree (AnnotatedString expression)
        {
            expression.CheckForTrailingSemi ();
            SupressPrinting = expression.SupressPrinting;

            ExpressionTreeNode.NodeCounter = 0;
            root = new ExpressionTreeNode (expression);
            Compact ();
        }

        //******************************************************************************

        public TreeViewItem TreeView ()
        {
            TreeViewItem tree = new TreeViewItem ();

            string headerString = "";

            if (root.Operator.Length > 0)
                headerString = root.Operator + ", " + root.NodeType.ToString ();

            else if (root.ValueValid)
                headerString = root.Value.ToString () + ", " + root.NodeType.ToString ();

            tree.Header = headerString;

            foreach (ExpressionTreeNode node in root.Operands)
                node.BuildTreeView (tree);

            tree.ExpandSubtree ();

            return tree;
        }

        //******************************************************************************

        // this is necessary for A:B:C expressions ouside of brackets, streamlines other trees

        void Compact ()
        {
            root.Compact ();
        }

        //******************************************************************************

        public PLVariable Evaluate ()
        {
            return root.Evaluate ();
        }


    }
}
