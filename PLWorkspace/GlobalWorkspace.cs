using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using FunctionLibrary;

using PLCommon;

namespace PLWorkspace
{
    internal class GlobalWorkspace : WorkspaceBase
    {
        static int InstanceCounter;

        static private readonly Dictionary<string, PLVariable> Constants = new Dictionary<string, PLVariable> ();

        static GlobalWorkspace ()
        {
            InstanceCounter = 0;

            PLDouble PI = new PLDouble (Math.PI); PI.Name = "PI"; Constants.Add ("PI", PI); Constants.Add ("pi", PI);
            PLDouble e  = new PLDouble (Math.Exp (0)); e.Name = "e";   Constants.Add ("e", e);

            PLBool TRUE  = new PLBool (true);  TRUE.Name = "true";   Constants.Add ("true", TRUE);
            PLBool FALSE = new PLBool (false); FALSE.Name = "false"; Constants.Add ("false", FALSE);

            PLComplex i = new PLComplex (0, 1); i.Name = "i"; Constants.Add ("i", i);
            PLComplex j = new PLComplex (0, 1); j.Name = "j"; Constants.Add ("j", i);

            Constants.Add ("equal",  new PLString ("equal"));
            Constants.Add ("tight",  new PLString ("tight"));
            Constants.Add ("frozen", new PLString ("frozen"));
            Constants.Add ("auto",   new PLString ("auto"));
            Constants.Add ("on",     new PLString ("on"));
            Constants.Add ("off",    new PLString ("off"));
            Constants.Add ("long",   new PLString ("long"));
            Constants.Add ("short",  new PLString ("short"));
        }

    //***************************************************************************************************

        internal GlobalWorkspace () : base ("Global")
        {
            if (++InstanceCounter > 1)
                throw new Exception ("Only one GlobalWorkspace allowed");
        }

    //***************************************************************************************************

        internal override SymbolicNameTypes WhatIs (string str)
        {
            SymbolicNameTypes type = base.WhatIs (str);

            if (type == SymbolicNameTypes.Unknown) 
                if (Constants.ContainsKey (str))
                    type = SymbolicNameTypes.Constant;

            return type;
        }

    //***************************************************************************************************

        internal override List<string> PartialMatch (string str)
        {
            List<string> matches = base.PartialMatch (str);

            foreach (string cmd in Constants.Keys) 
            {
                if (cmd.StartsWith (str)) 
                    matches.Add (cmd + " ");
            }

            return matches;
        }

    //***************************************************************************************************

        internal override PLVariable Exists (PLVariable arg)
        {
            if (arg != null)
            {
                PLString str = arg as PLString;

                if (Variables.ContainsKey (str.Text)) return new PLBool (true);
                if (Constants.ContainsKey (str.Text)) return new PLBool (true);
            }

            return new PLBool (false);
        }

        internal override bool Exists (string str)
        {
            return Variables.ContainsKey (str) || Constants.ContainsKey (str);
        }

        internal override PLVariable Get (string name)
        {
            if (Variables.ContainsKey (name))
                return Variables [name];

            if (Constants.ContainsKey (name))
                return Constants [name];

            throw new Exception ("Variable " + name + " undefined");
        }

    }
}
