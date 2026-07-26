
/*
    BlockManager.cs
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    public static class BlockManager
    {
        // PartialBlocks - incomplete blocks, as they are being read from input
        private static readonly Stack<Block> PartialBlocks = new Stack<Block> ();

        // CompleteBlocks - ready for execution
        private static readonly Dictionary<string, Block> CompleteBlocks = new Dictionary<string, Block> ();


        //*************************************************************************************

        // debug printing
        static PrintFunction Print = null;

        static public void SetPrintFunction (PrintFunction pr)
        {
            Print = pr;
        }

        //*************************************************************************************

        internal static readonly List<string> BlockStartKeywords = new List<string> () {"for", "while", "if"};
        internal static readonly List<string> BlockEndKeywords = new List<string> () { "end", };

        public static SymbolicNameTypes WhatIs (string str)
        {
            if (BlockStartKeywords.Contains (str))
                return SymbolicNameTypes.BlockStart;

            if (BlockEndKeywords.Contains (str))
                return SymbolicNameTypes.BlockEnd;

            return SymbolicNameTypes.Unknown;
        }

        public static bool IsBlockName (string str)
        {
            //Print?.Invoke ("IsBlockName " + CompleteBlocks.ContainsKey (str));

            return CompleteBlocks.ContainsKey (str);
        }

        //*************************************************************************************

        // PartialBlock - a block being built
        private static Block PartialBlock
        {
            get
            {
                return PartialBlocks.Count > 0 ? PartialBlocks.Peek () : null;
            }
        }

        // BlockCollectionInProgress - a Block has been started but not ended
        public static bool BlockCollectionInProgress
        {
            get
            {
                return PartialBlock != null;
            }
        }

        //*************************************************************************************

        public static void Add (AnnotatedString astr)
        {
            StringClassifier classifier = new StringClassifier ();
            InputLineType lineType = classifier.Classify (astr);

            if (lineType == InputLineType.BlockStart)
            {
                StartNewBlock (astr);
            }

            else if (lineType == InputLineType.BlockEnd)
            {
                Block justCompleted = PartialBlocks.Pop ();
                CompleteBlocks.Add (justCompleted.Name, justCompleted);

                if (PartialBlock != null)
                    PartialBlock.Add (new AnnotatedString (justCompleted.Name));

                else
                {

                    //foreach (var kvp in CompleteBlocks)
                    //{
                    //    Block bl = kvp.Value;
                    //    Print (bl.ToString () + "\n");
                    //}

                    justCompleted.Run ();

                    CompleteBlocks.Clear ();
                }
            }

            else
                PartialBlock.Add (astr);
        }

        //*************************************************************************************

        public static void StartNewBlock (AnnotatedString astr)
        {
            string keyword = astr.FirstWord;

            switch (keyword)
            {
                //case "for":
                //    PartialBlocks.Push (new ForBlock (astr));
                //    break;

                //case "while":
                //    PartialBlocks.Push (new WhileBlock (str));
                //    break;

                case "if":
                    PartialBlocks.Push (new IfBlock (astr));
                    break;

                default: throw new Exception ("Unrecognized block type: " + astr.Plain);
            }
        }
    }
}
