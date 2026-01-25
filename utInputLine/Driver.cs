using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Main;

using static Main.InputLineProcessor;

namespace utInputLine
{
    class Driver
    {
        static string filePathAndName = @"D:\From_C_Visual Studio 2022\Visual Studio 2022\Projects\PlotLab\Examples\InputLineTests.m";


        static Workspace workspace   = new Workspace ();
        static Library   library     = new Library ();
        static FileSystem fileSystem = new FileSystem ();

        static void Print (string str)
        {
            Console.WriteLine (str);
        }

        static void Main (string [] args)
        {
            //AnnotatedString s1 = new AnnotatedString ("-");
            //AnnotatedString s2 = new AnnotatedString ("123");
            ////AnnotatedChar c2 = new AnnotatedChar ('a');


            //Console.WriteLine ("s1 before");
            //Console.WriteLine ("Text only:");
            //Console.WriteLine (s1.Raw);

            //Console.WriteLine ("Annotations:");
            //Console.WriteLine (s1);
            //Console.WriteLine ("");

            //AnnotatedString s3 = s1 + s2;

            //Console.WriteLine ("s1 after");
            //Console.WriteLine ("Text only:");
            //Console.WriteLine (s1.Raw);

            //Console.WriteLine ("Annotations:");
            //Console.WriteLine (s1);
            //Console.WriteLine ("");

            //Console.WriteLine ("s3");
            //Console.WriteLine ("Text only:");
            //Console.WriteLine (s3.Raw);

            //Console.WriteLine ("Annotations:");
            //Console.WriteLine (s3);
            //Console.WriteLine ("");


            //AnnotatedString s2 = s1.AddOuterParens ();

            //Console.WriteLine ("");
            //Console.WriteLine ("modified:");

            //Console.WriteLine ("Text only:");
            //Console.WriteLine (s2.Raw);

            //Console.WriteLine ("Annotations:");
            //Console.WriteLine (s2);
            //Console.WriteLine ("");

            ////Console.WriteLine ("c2 Text only:");
            ////Console.WriteLine (c2.Character);

            ////Console.WriteLine ("Annotations:");
            ////Console.WriteLine (c2.ToString ());
            ////Console.WriteLine ("");

            //s1.Append (s2);

            //Console.WriteLine (s1.Text);
            //Console.WriteLine (s1);

            //List<AnnotatedString> strList = s1.SplitAtLevel0Semicolon ();
            //Console.WriteLine (strList [0]);
            //Console.WriteLine (strList [1]);




            try
            {
                InputLineProcessor inputProcessor = new InputLineProcessor (workspace, library, fileSystem, Print);

                StreamReader file = new StreamReader (filePathAndName);
                string raw;

                while ((raw = file.ReadLine ()) != null)
                {
                    if (raw.Length > 0)
                    {
                        inputProcessor.ParseOneInputLine (raw);
                    }
                }

                file.Close ();
            }

            catch (Exception ex)
            {
                Print ("Exception: " + ex.Message);
            }
        }
        }
    }
