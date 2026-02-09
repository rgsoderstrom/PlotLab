using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    public class ExpressionTree
    {
        readonly ExpressionTreeNode root;

        public ExpressionTree (AnnotatedString expression) 
        {
            ExpressionTreeNode.InstanceCounter = 0;
            root = new ExpressionTreeNode (expression);
         //   Compact ();
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

        //void Compact ()
        //{
        //    root.Compact ();
        //}

        //******************************************************************************

        //public PLVariable Evaluate (Workspace workspace)
        //{
        //    return root.Evaluate (workspace);
        //}


    }
}
