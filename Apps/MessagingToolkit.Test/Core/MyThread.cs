using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MessagingToolkit.Test.Core
{
    public class MyThread
    {
        private int id;

        public MyThread(int id)
        {
            this.id = id;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void MyMethod()
        {
            Console.WriteLine(id + ": entering MyMethod");
            Console.WriteLine(id + ":" + DateTime.Now.ToString());
            Thread.Sleep(5000);
            Console.WriteLine(id + " exiting :" + DateTime.Now.ToString());
            Console.WriteLine(id + ": exiting MyMethod");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void MyMethod1()
        {
            Console.WriteLine("entering MyMethod");
            Console.WriteLine("entering " + DateTime.Now.ToString());
            Thread.Sleep(5000);
            Console.WriteLine("exiting " + DateTime.Now.ToString());
            Console.WriteLine("exiting MyMethod");
        }


        
    }
}
