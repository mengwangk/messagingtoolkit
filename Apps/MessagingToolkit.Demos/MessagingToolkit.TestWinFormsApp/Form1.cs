using MessagingToolkit.Core;
using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Collections;
using MessagingToolkit.Core.Mobile.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MessagingToolkit.TestWinFormsApp
{

    public partial class Form1 : Form
    {
        ConcurrentPriorityQueue<Priority, IMessage> items = new ConcurrentPriorityQueue<Priority, IMessage>();

        ConcurrentPriorityQueue<int, IMessage> items2 = new ConcurrentPriorityQueue<int, IMessage>();

        public Form1()
        {
            InitializeComponent();
        }

        private Sms CreateSms(string content, MessageQueuePriority priority)
        {
            Sms sms = Sms.NewInstance();
            sms.Content = content;
            sms.QueuePriority = priority;
            return sms;
        }

        private void Print()
        {


            while (items.Count > 0)
            {
                KeyValuePair<Priority, IMessage> item;
                if (items.TryPeek(out item))
                {
                    Sms sms = (Sms)item.Value;
                    Console.WriteLine("Peek: " + sms.Content);
                    Thread.Sleep(600);
                    if (items.TryDequeue(out item))
                    {
                        //Console.WriteLine("dequeue");
                    }
                    else
                    {
                        Console.WriteLine("unable to dequeue");
                    }
                }
                else
                {
                    Console.WriteLine("unable to peek");
                }

            }

        }

        private void Print2()
        {


            while (items2.Count > 0)
            {
                KeyValuePair<int, IMessage> item;
                if (items2.TryPeek(out item))
                {
                    Sms sms = (Sms)item.Value;
                    Console.WriteLine("Peek: " + sms.Content);
                    Thread.Sleep(600);
                    if (items2.TryDequeue(out item))
                    {
                        //Console.WriteLine("dequeue");
                    }
                    else
                    {
                        Console.WriteLine("unable to dequeue");
                    }
                }
                else
                {
                    Console.WriteLine("unable to peek");
                }

            }

        }


        private void PrintOrder()
        {
            foreach (KeyValuePair<Priority, IMessage> i in items)
            {
                Sms sms = (Sms)i.Value;
                Console.WriteLine("Item: " + sms.Content);
            }


        }
        private void Form1_Load(object sender, EventArgs e)
        {



        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sms sms1 = CreateSms("I1", MessageQueuePriority.High);
            Sms sms2 = CreateSms("N1", MessageQueuePriority.Normal);
            Sms sms3 = CreateSms("N2", MessageQueuePriority.Normal);
            Sms sms4 = CreateSms("N3", MessageQueuePriority.Normal);
            Sms sms5 = CreateSms("I6", MessageQueuePriority.High);
            Sms sms6 = CreateSms("N4", MessageQueuePriority.Normal);
            Sms sms7 = CreateSms("N5", MessageQueuePriority.Normal);



            items.Enqueue(Priority.High, sms1);
            items.Enqueue(Priority.Normal, sms2);
            items.Enqueue(Priority.Normal, sms3);
            items.Enqueue(Priority.Normal, sms4);
            items.Enqueue(Priority.High, sms5);
            items.Enqueue(Priority.Normal, sms6);
            items.Enqueue(Priority.Normal, sms7);

            Thread t = new Thread(new ThreadStart(this.Print));
            t.Start();

            //PrintOrder();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Sms sms1 = CreateSms("I1", MessageQueuePriority.High);
            Sms sms2 = CreateSms("N1", MessageQueuePriority.Normal);
            Sms sms3 = CreateSms("N2", MessageQueuePriority.Normal);
            Sms sms4 = CreateSms("N3", MessageQueuePriority.Normal);
            Sms sms5 = CreateSms("I6", MessageQueuePriority.High);
            Sms sms6 = CreateSms("N4", MessageQueuePriority.Normal);
            Sms sms7 = CreateSms("N5", MessageQueuePriority.Normal);



            items2.Enqueue(1, sms1);
            items2.Enqueue(2, sms2);
            items2.Enqueue(2, sms3);
            items2.Enqueue(2, sms4);
            items2.Enqueue(1, sms5);
            items2.Enqueue(2, sms6);
            items2.Enqueue(2, sms7);

            Thread t = new Thread(new ThreadStart(this.Print2));
            t.Start();

            //PrintOrder();

        }

    }
}
