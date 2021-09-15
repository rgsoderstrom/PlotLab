using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;

namespace PLKernel
{
    internal partial class ExpressionTreeNode
    {
        public string Operator = "";
        public TokenType NodeType = TokenType.None;

        public List<ExpressionTreeNode> Operands = new List<ExpressionTreeNode> ();


        private Workspace workspace; // copy of passed-in value

        private PLVariable nodeValue = null;
        public bool ValueValid {get {return nodeValue != null;}}

        public PLVariable Value 
        { 
            get
            {
                if (ValueValid) return nodeValue;
                else return Evaluate (workspace);
            }

            set
            {
                nodeValue = value;
            }
        }

        //******************************************************************************************
        //******************************************************************************************
        //******************************************************************************************

        public static PrintFunction PF = null;
        public static int InstanceCounter = 0; // zeroed when new tree started

        //
        // public ctor
        //
        public ExpressionTreeNode (string expr, Workspace ws)
        {
            workspace = ws;
            TokenParsing parsing = new TokenParsing ();
            List<Token> tokens = parsing.StringToTokens (expr, workspace);
            
            ConstructorCommon (tokens);
        }

        //
        // private ctor
        //
        ExpressionTreeNode (List<Token> tokens, Workspace ws)
        {
            workspace = ws;
            ConstructorCommon (tokens);
        }

        //***************************************************************************************
        //***************************************************************************************
        //***************************************************************************************

        //
        // Invoked by both constructors
        //
        void ConstructorCommon (List<Token> tokens)
        {
            if (InstanceCounter++ > 100)
            {
                throw new Exception ("Too many nodes in expression tree");
            }

            try
            {
                if (tokens.Count == 0) throw new Exception ("Token count == 0");

                //********************************************************************************

                else if (tokens.Count == 1)
                {
                    switch (tokens [0].type)
                    {
                       case TokenType.GroupingParens:  // e.g. (a * b + c) 
                            BuildNodeFrom_GroupingParens (tokens, workspace);
                            break;

                        case TokenType.Brackets:
                        case TokenType.BracketsColon:
                        case TokenType.BracketsComma:
                        case TokenType.BracketsSemi:
                        case TokenType.BracketsSpace:
                            BuildNodeFrom_Brackets (tokens, workspace);                                
                            break;
                       
                        case TokenType.Numeric:  // scalar 
                            BuildNodeFrom_Numeric (tokens, workspace);
                            break;

                        case TokenType.FunctionName:
                        case TokenType.Undefined:
                        case TokenType.VariableName:  // symbolic name for a variable or constant
                            BuildNodeFrom_VariableName (tokens, workspace);
                            break;

                        case TokenType.ArithmeticOperator:
                            BuildNodeFrom_Operator (tokens);
                            break;

                        case TokenType.String:
                            BuildNodeFrom_String (tokens);
                            break;

                        case TokenType.Pair:
                        {
                            switch ((tokens [0] as TokenPair).pairType)
                            {
                                case TokenPairType.Function:
                                    BuildNodeFrom_FunctionPair (tokens [0] as TokenPair);
                                    break;

                                case TokenPairType.Submatrix:
                                    BuildNodeFrom_SubmatrixPair (tokens [0] as TokenPair);
                                    break;
                            }
                        }
                        break;

                        default:
                            throw new Exception (string.Format ("Token Type {0} not supported", tokens [0].type));
                    }
                }

                //********************************************************************************

                else if (tokens.Count == 2)
                {
                    throw new Exception ("Token count == 2");
                }

                //********************************************************************************

                // count >= 3

                else
                {
                    BuildNodeFrom_List (tokens);
                }
            }

            catch (Exception ex)
            {
                throw new Exception (string.Format ("{0}", ex.Message));
                //throw new Exception (string.Format ("ExpressionNode ctor exception: {0}", ex.Message));
            }
        }

        //*******************************************************************************************

        public void BuildTreeView (TreeViewItem tree)
        {
            TreeViewItem node = new TreeViewItem ();

            tree.Items.Add (node);

            string headerString = "?";

            if (Operator.Length > 0)
                headerString = Operator + ", " + NodeType.ToString ();

            else if (ValueValid)
                headerString = Value.ToString () + ", " + NodeType.ToString ();

            node.Header = headerString;

            foreach (ExpressionTreeNode etn in Operands)
                etn.BuildTreeView (node);
        }

        //*******************************************************************************************

        readonly List<string> CanCompact = new List<string> () { "+", ":", "/", "Comma"};

        internal void Compact ()
        {
            // list of nodes to be "compacted"
            List<ExpressionTreeNode> cp = new List<ExpressionTreeNode> ();

            foreach (ExpressionTreeNode node in Operands)
            {
                node.Compact ();

                if (CanCompact.Contains (Operator))
                {
                    if (node.Operator == Operator)
                    {
                        cp.Add (node);
                    }
                }
            }

            List<ExpressionTreeNode> newOperands = new List<ExpressionTreeNode> ();

            foreach (ExpressionTreeNode op in Operands)
            {
                // if "op" is in "cp", add its children, else add "op"

                if (cp.Contains (op))
                {
                    newOperands.AddRange (op.Operands);
                }
                else
                {
                    newOperands.Add (op);
                }
            }

            Operands = newOperands;
        }




    }
}
