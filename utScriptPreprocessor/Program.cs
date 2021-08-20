using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptPreprocess
{
    class Program
    {
        static void Main (string [] args)
        {
            try 
            { 
                string scriptFile = @"..\..\s4.m";

                ScriptPreprocessor spp = new ScriptPreprocessor ();
                NumberedScript expanded = spp.Run (scriptFile, true);

                foreach (var s in expanded)
                {
                    Console.WriteLine (s);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine ("Exception: " + ex.Message);
            }        
        }
    }
}
