using System;

/*
    Not used
*/


namespace utBlockManager
{
    internal class Driver
    {
        static void Main (string [] _)
        {
            Block.Print = Console.WriteLine;

            ForBlock forBlock = new ForBlock ("for a = 1:9,");
            forBlock.Add (new AnnotatedString ("b = a * 3;"));
            forBlock.Add (new AnnotatedString ("c = b ^ 2;"));
            forBlock.Add ("end");

            forBlock.Run ();
        }
    }
}
