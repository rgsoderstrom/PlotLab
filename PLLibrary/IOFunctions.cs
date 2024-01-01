using System;
using System.Collections.Generic;

using PLCommon;

namespace FunctionLibrary
{
    static public partial class IOFunctions
    {
        //*********************************************************************************************
        //
        // map function name strings to executable functions
        //

        static public Dictionary<string, PLFunction> GetContents ()
        {
            return new Dictionary<string, PLFunction>
            {
                {"sprintf", PrintToString},
                {"disp",    Display},  
            };
        }

        static public void GetZeroArgNames (List<string> funcs)
        {
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        static public PLVariable PrintToString (PLVariable arg)
        {
            if (arg is PLString)
                return arg;

            PLList lst = arg as PLList;
            if (lst == null) throw new Exception ("sprintf error");
            PLString FormatString = lst [0] as PLString;
            if (FormatString == null) throw new Exception ("sprintf error");

            List<string> formatTokens = ParseFormat (FormatString.Data);
            PLString str = new PLString (""); // build return string here

            int valueIndex = 1;

            foreach (string ft in formatTokens)
            {
                if (valueIndex > lst.Count)  // syntax error?
                    break;

                if (ft [0] == '%')
                    str.Add (lst [valueIndex++].ToString (ft));
                else
                    str.Add (ft);
            }

            return str;
        }

        //******************************************************************************

        // format tokens:
        //  %d
        //  %f
        //  %e

        static List<string> ParseFormat (string fmt)
        {
            // remove outer quotes
            int i1 = fmt.IndexOf ('\'');
            int i2 = fmt.LastIndexOf ('\'');

            string format = fmt.Remove (i2, 1);
            format = format.Remove (i1, 1);

            // find all of the % signs in format string
            List<int> formatTokenStartsAt = new List<int> ();
            int i3;
            int startAt = 0;

            while (true)
            {
                i3 = format.IndexOf ('%', startAt);

                if (i3 == -1) break;
                else {formatTokenStartsAt.Add (i3); startAt = i3 + 1;}
            }

            // remove indexes of any 2 consecutive percent signs
            List<int> diffs = new List<int> ();
            for (int i = 1; i<formatTokenStartsAt.Count; i++)
                diffs.Add (formatTokenStartsAt [i] - formatTokenStartsAt [i-1]);

            while ((i2 = diffs.LastIndexOf (1)) != -1)
            {
                formatTokenStartsAt.RemoveAt (i2+1);
                formatTokenStartsAt.RemoveAt (i2);
                diffs.RemoveAt (i2);
            }

            List<int> formatTokenEndsAt = new List<int> ();

            foreach (int s in formatTokenStartsAt)
            {
                int endsAt = s + 1;

                if (endsAt == format.Length)
                    throw new Exception ("sprintf format string syntax error");

                while (char.IsDigit (format [endsAt]) || format [endsAt] == '.')
                {
                    endsAt++;

                    if (endsAt == format.Length)
                        throw new Exception ("sprintf format string syntax error");
                }

                formatTokenEndsAt.Add (endsAt);
            }

            
            // build list of tokens
            List<string> tokens = new List<string> ();

            int current = 0;

            for (int i=0; i<formatTokenStartsAt.Count; i++)
            {
                i1 = formatTokenStartsAt [i];
                i2 = formatTokenEndsAt [i];

                if (i1 > current)
                    tokens.Add (format.Substring (current, i1 - current));

                tokens.Add (format.Substring (i1, i2 - i1 + 1));

                current = i2 + 1;
            }

            if (current < format.Length)
                tokens.Add (format.Substring (current));

            return tokens;
        }

        //****************************************************************************************
        //****************************************************************************************
        //****************************************************************************************

        static public PLVariable Display (PLVariable a)
        {
            PLString  str = a as PLString;
            PLList    lst = a as PLList;
            PLMatrix  mat = a as PLMatrix;
            PLDouble  dbl = a as PLDouble;
            PLInteger igr = a as PLInteger;
            PLBool bl     = a as PLBool;

            if (str != null)
            {
                if (str.Data [str.Data.Length - 1] == '\'') str.Data = str.Data.Substring (0, str.Data.Length - 1);
                if (str.Data [0] == '\'') str.Data = str.Data.Substring (1, str.Data.Length - 1);

                return str;
            }

            if (lst != null)
            {
                return new PLString ("list");
            }

            if (mat != null)
            {
                return new PLString (mat.ToString ()); // new PLString ("mat");
            }

            if (dbl != null)
            {
                return new PLString (dbl.ToString ());
            }

            if (igr != null)
            {
                return new PLString (igr.ToString ());
            }

            if (bl != null)
            {
                return new PLString (bl.ToString ());
            }


            return new PLString (a.GetType ().ToString ()); // PLNull ();
        }

    }
}
