using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using FunctionLibrary;

using PLCommon;

namespace PLWorkspace
{
    internal class GlobalWorkspace : WorkspaceBaseClass
    {
        static int InstanceCounter = 0;

        static private readonly Dictionary<string, PLVariable> Constants = new Dictionary<string, PLVariable> ();

        static GlobalWorkspace ()
        {
            PLDouble pi = new PLDouble ("pi", Math.PI);      Constants.Add (pi.Name, pi);
            PLDouble e  = new PLDouble ("e",  Math.Exp (1)); Constants.Add (e.Name, e);

            PLBool TRUE  = new PLBool ("true",  true);       Constants.Add (TRUE.Name, TRUE);
            PLBool FALSE = new PLBool ("false", false);      Constants.Add (FALSE.Name, FALSE);

            PLComplex i = new PLComplex ("i", 0, 1);         Constants.Add (i.Name, i);
            PLComplex j = new PLComplex ("j", 0, 1);         Constants.Add (j.Name, j);

            //Constants.Add ("equal",  new PLString ("equal"));
            //Constants.Add ("tight",  new PLString ("tight"));
            //Constants.Add ("frozen", new PLString ("frozen"));
            //Constants.Add ("auto",   new PLString ("auto"));
            //Constants.Add ("on",     new PLString ("on"));
            //Constants.Add ("off",    new PLString ("off"));
            //Constants.Add ("long",   new PLString ("long"));
            //Constants.Add ("short",  new PLString ("short"));
        }

    //***************************************************************************************************

        internal GlobalWorkspace () : base ("Global")
        {
            if (++InstanceCounter > 1)
                throw new Exception ("Only one GlobalWorkspace allowed");
        }

    //***************************************************************************************************

        //internal override SymbolicNameTypes WhatIs (string str)
        //{
        //    SymbolicNameTypes type = base.WhatIs (str);

        //    if (type == SymbolicNameTypes.Unknown) 
        //        if (Constants.ContainsKey (str))
        //            type = SymbolicNameTypes.Constant;

        //    return type;
        //}
       
        internal override bool Contains (string var)
        {
            return Variables.ContainsKey (var) || Constants.ContainsKey (var);
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

        //internal override bool Exists (string arg)
        //{
        //    if (arg != null)
        //    {
        //        if (Variables.ContainsKey (arg)) return true;
        //        if (Constants.ContainsKey (arg)) return true;
        //    }

        //    return false;
        //}


        //internal PLVariable Get (string name)
        //{
        //    if (Variables.ContainsKey (name))
        //        return Variables [name];

        //    if (Constants.ContainsKey (name))
        //        return Constants [name];

        //    throw new Exception ("Variable " + name + " undefined");
        //}

        internal override bool Get (string name, ref PLVariable var)
        {
            if (Variables.ContainsKey (name))
            {
                var = Variables [name];
                return true;
            }

            if (Constants.ContainsKey (name))
            {
                var = Constants [name];
                return true;
            }

            return false;
        }

    }
}
