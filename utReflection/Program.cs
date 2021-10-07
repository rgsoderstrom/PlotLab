using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

// investigate use of reflection to run functions in math library

// https://www.c-sharpcorner.com/UploadFile/84c85b/using-reflection-with-C-Sharp-net/

namespace utReflection
{
    class Program
    {
        //static void FieldInvestigation(Type t)  
        //{  
        //    Console.WriteLine("*********Fields*********");  
        //    FieldInfo [] fld= t.GetFields();   
        //    foreach(FieldInfo f in fld)  
        //    {  
        //        Console.WriteLine("-->{0}", f.Name);   
        //    }  
        //}  
  
        //static void MethodInvestigation(Type t)  
        //{  
        //    Console.WriteLine("*********Methods*********");  
        //    MethodInfo [] mth = t.GetMethods();  
        //    foreach (MethodInfo m in mth)  
        //    {  
        //        Console.WriteLine("-->{0}", m.Name);  

        //        if (m.Name == "Sin")
        //        {
        //            Console.WriteLine ("---------------------------");
        //        }
        //    }  
        //}          
        
        static void Main (string [] args)
        {
            Type t = Type.GetType ("System.Math");

            var methodInfoStatic = t.GetMethod ("Sin");

            if (methodInfoStatic == null)
            {
                throw new Exception ("No such static method exists.");
            }

            // Specify parameters for static method: 'public static void MyMethod(int count, float radius)'
            object[] staticParameters = new object [1];
            staticParameters [0] = 3.14159 / 8;

            object results = 0;

            var watch = new System.Diagnostics.Stopwatch();
            
            watch.Start();

            for (int i=0; i<1000000; i++)
            {
                results = methodInfoStatic.Invoke (null, staticParameters);
            }

            watch.Stop();

            Console.WriteLine ($"Execution Time with .Invoke: {watch.ElapsedMilliseconds}");


            watch.Reset ();
            watch.Start();

            for (int i=0; i<1000000; i++)
            {
                results = Math.Sin (3.14/8);
            }

            watch.Stop();

            Console.WriteLine ($"Execution Time with direct call: {watch.ElapsedMilliseconds}");








           // if (results is double)
           //     Console.WriteLine ("results = " +  ((double) results).ToString ());

            //Console.Write("Enter the Name to Explore:");  
            //string typName = "System.Math"; //Console.ReadLine();  
  
            //Type t = Type.GetType(typName);  
            //FieldInvestigation(t);  
            //MethodInvestigation(t);  
   
            //Console.ReadKey();            
        }
    }
}
