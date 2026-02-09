using System;
using System.Collections.Generic;
using System.Windows.Controls;

using PLCommon;

namespace Main
{
    internal partial class ExpressionTreeNode
    {
        public TokenType NodeType;// = TokenType.None;

        public string                   Operator = "";
        public List<ExpressionTreeNode> Operands = new List<ExpressionTreeNode> ();

        // available all nodes
        public static IWorkspace    BaseWorkspace;   // most user created variables
        public static IWorkspace    GlobalWorkspace; // constants (e.g. pi) and variables explcitly marked "global"
        public static ILibrary      Library;
        public static IFileSystem   FileSystem;
        public static PrintFunction Print;





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
        public ExpressionTreeNode (AnnotatedString expr)
        {
            TokenParsing parsing = new TokenParsing ();
            List<IToken> tokens = parsing.StringToTokens (expr, Workspace, Library, FileSystem);
            
            ConstructorCommon (tokens);
        }

        //
        // private ctor
        //
        ExpressionTreeNode (List<IToken> tokens)
        {
            ConstructorCommon (tokens);
        }

        //***************************************************************************************
        //***************************************************************************************
        //***************************************************************************************

        //
        // Invoked by both constructors
        //
        void ConstructorCommon (List<IToken> tokens)
        {
            //if (InstanceCounter++ > 100)
            //{
            //    throw new Exception ("Too many nodes in expression tree");
            //}

            try
            {
                if (tokens.Count == 0) throw new Exception ("Token count == 0");

                //********************************************************************************

                else if (tokens.Count == 1)
                {
                    switch (tokens [0].Type)
                    {
                       case TokenType.GroupingParens:  // e.g. (a * b + c) 
                            BuildNodeFrom_GroupingParens (tokens, Workspace);
                            break;

                        case TokenType.Brackets:
                        case TokenType.BracketsColon:
                        case TokenType.BracketsComma:
                        case TokenType.BracketsSemi:
                        case TokenType.BracketsSpace:
                            BuildNodeFrom_Brackets (tokens, Workspace);                                
                            break;
                       
                        case TokenType.Numeric:  // scalar 
                            BuildNodeFrom_Numeric (tokens, Workspace);
                            break;

                        case TokenType.FunctionName:
                        case TokenType.Undefined:
                        case TokenType.VariableName:  // symbolic name for a variable or constant
                            BuildNodeFrom_VariableName (tokens, Workspace);
                            break;

                        case TokenType.Operator:
                            BuildNodeFrom_Operator (tokens);
                            break;

                        case TokenType.String:
                            BuildNodeFrom_String (tokens);
                            break;

                        case TokenType.Pair:
                        {
                            switch ((tokens [0] as TokenPair).PairType)
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
                            throw new Exception (string.Format ("Token Type {0} not supported", tokens [0].Type));
                    }
                }

                //********************************************************************************

                else if (tokens.Count == 2)
                {
                    string str = "\nToken count == 2. Tokens:\n";
                    str += "  " + tokens [0].ToString () + "\n";
                    str += "  " + tokens [1].ToString () + "\n";

                    throw new Exception (str);
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

    // fails for expressions of the form: 1000 / (10 / 2)
    //  readonly List<string> CanCompact = new List<string> () { "+", ":", "/", "Comma"};
        readonly List<string> CanCompact = new List<string> () { "+", ":", "Comma"};

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

       //     string exp = expression; // temp for debug

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
