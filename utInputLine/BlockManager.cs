
/*
    BlockManager.cs
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PLCommon;

namespace PLMain
{
    public static class BlockManager
    {
        //*************************************************************************************

        internal static readonly List<string> BlockStartKeywords = new List<string> () {"for", "while", "if" };
        internal static readonly List<string> BlockEndKeywords   = new List<string> () {"end", };

        public static SymbolicNameTypes WhatIs (string str)
        {
            if (BlockStartKeywords.Contains (str))
                return SymbolicNameTypes.BlockStart;

            if (BlockEndKeywords.Contains (str))
                return SymbolicNameTypes.BlockEnd;

            return SymbolicNameTypes.Unknown;
        }

        //*************************************************************************************

        // ActiveBlocks - a stack of incomplete blocks 
        private static readonly Stack<Block> ActiveBlocks = new Stack<Block> ();

        // PartialBlock - a block being built
        private static Block PartialBlock {get {return ActiveBlocks.Count > 0 
                                                     ? ActiveBlocks.Peek ()
                                                     : null;}}

        // BlockCollectionInProgress - a Block has been started but not ended
        public static bool BlockCollectionInProgress {get {return PartialBlock != null 
                                                               && PartialBlock.Complete == false;}}

        //*************************************************************************************

        public static void Add (string str, InputLineType type)
        {
            if (type == InputLineType.BlockStart)
            {
                StartNewBlock (str);
            }

            else if (type == InputLineType.BlockEnd)
            {
                PartialBlock.Close ();
                PartialBlock.Run ();
                ActiveBlocks.Pop ();
            }

            else
                PartialBlock.Add (str);
        }

        //*************************************************************************************

        public static void StartNewBlock (string str)
        {
            string keyword = StringUtils.FirstWord (str);

            switch (keyword)
            {
                case "for":
                    ActiveBlocks.Push (new ForBlock (str));
                    break;

                case "while":
                    ActiveBlocks.Push (new WhileBlock (str));
                    break;

                case "if":
                    ActiveBlocks.Push (new IfBlock (str));
                    break;

                default: throw new Exception ("Unrecognized block type: " + str);
            }


        }

    }
}
