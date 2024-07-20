using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NUnit.Framework;

namespace MessagingToolkit.Test.Core
{
    [TestFixture]
    public class MyThreadTest
    {
        [Test]
        public void TestThread()
        {
            Thread t1 = new Thread(new ThreadStart(this.Thread1));
            t1.Start();

            Thread t2 = new Thread(new ThreadStart(this.Thread2));
            t2.Start();

            Console.WriteLine("Sleeping for 15 seconds");
            Thread.Sleep(15000);
        }


        public void Thread1()
        {
            MyThread t = new MyThread(1);
            t.MyMethod();
        }

        public void Thread2()
        {
            MyThread t = new MyThread(1);
            t.MyMethod();
        }
    }
}
