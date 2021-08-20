using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;
using PLWorkspace;

namespace PLKernel
{
    public class EntryPoint
    {
        public void ProcessArithmeticExpression (ref PLVariable answer, string expression, Workspace workspace)
        {
            try
            {
                TokenParsing parsing = new TokenParsing ();
                List<Token> passOneTokens = parsing.ParsingPassOne (expression);
                List<Token> passTwoTokens = parsing.ParsingPassTwo (passOneTokens, workspace);

                ExpressionTree tree = new ExpressionTree (expression, workspace);
                //window.TreeView1.Items.Clear ();
                //window.TreeView1.Items.Add (tree.TreeView ());

                answer = tree.Evaluate (workspace);
            }

            catch (Exception ex)
            {
                throw new Exception ("expression " + expression + " failed, " + ex.Message);
            }
        }
    }
}
