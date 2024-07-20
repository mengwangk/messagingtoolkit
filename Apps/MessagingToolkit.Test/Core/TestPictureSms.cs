//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Drawing;
using System.Threading;

using NUnit.Framework;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Feature;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.PduLibrary;
using MessagingToolkit.Core.Service;
using MessagingToolkit.Core;
using MessagingToolkit.MMS;

namespace MessagingToolkit.Test.Core
{
    [TestFixture]
    public class TestPictureSms
    {
        PriorityQueue<IMessage, MessageQueuePriority> messageQueue = new PriorityQueue<IMessage, MessageQueuePriority>();
        
        public void TestIsBlackAndWhite()
        {
            Console.WriteLine(GatewayHelper.IsBlackAndWhite(@"C:\MySpace\Personal\my.jpg"));
        }

       
        public void TestCreatePictureSms()
        {

            GatewayHelper.ResizeImage(@"C:\MySpace\Personal\new - Copy.jpg", @"C:\MySpace\Personal\new.jpg", 255, 255, true);

            Console.Write("done");
        }

       
        public void TestAsciiArt()
        {

            Console.WriteLine(AsciiArt.ConvertImage(new Bitmap(@"C:\MySpace\Personal\new - Copy.jpg"), AsciiArt.ImageSize.NormalResolution, true)); ;

            Console.Write("done");
        }

        public void AddToQueue(PriorityQueue<IMessage, MessageQueuePriority> messageQueue)
        {
            for (int i = 0; i < 10000; i++)
            {
                Sms sms1 = Sms.NewInstance();
                sms1.Content = "msg " + i;
                sms1.QueuePriority = MessageQueuePriority.Normal;
                messageQueue.Enqueue(sms1, sms1.QueuePriority);
            }
        }

        public void PrintQueue()
        {
            while (messageQueue.Count() > 0)
            {
                PriorityQueueItem<IMessage, MessageQueuePriority> msg = messageQueue.Peek();
                Sms sms = (Sms) msg.Value;
                Console.WriteLine(sms.Content);
                messageQueue.Dequeue();
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void TestPriority()
        {

            Console.WriteLine("Add messages to queue");
            AddToQueue(messageQueue);            
            Console.WriteLine("Done");

            Console.WriteLine("Printing queue");
            Thread t = new Thread(PrintQueue);
            t.IsBackground = true;
            t.Start();

            Thread.Sleep(5000);

            Console.WriteLine("Add high priority msg");
            Sms sms3 = Sms.NewInstance();
            sms3.Content = "High priority msg";
            sms3.QueuePriority = MessageQueuePriority.High;
            messageQueue.Enqueue(sms3, sms3.QueuePriority);
            Console.WriteLine("Done");


            Thread.Sleep(20000);

            try
            {
                t.Abort();
            } catch {}
           
        }
    }
}
