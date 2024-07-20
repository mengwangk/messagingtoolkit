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


using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using NUnit.Framework;

namespace MessagingToolkit.Core.Test
{
    [TestFixture]
    internal class PriorityQueueTest
    {
     
        public void testPriority()
        {
            PriorityQueue<Sms, MessageQueuePriority> priorityQueue = new PriorityQueue<Sms, MessageQueuePriority>();

            Sms sms1 = Sms.NewInstance();
            sms1.DestinationAddress = "019";
            sms1.QueuePriority = MessageQueuePriority.Normal;

            Sms sms2 = Sms.NewInstance();
            sms2.DestinationAddress = "012";
            sms2.QueuePriority = MessageQueuePriority.Low;

            vCard v = vCard.NewInstance();
            v.DestinationAddress = "011";
            v.QueuePriority = MessageQueuePriority.High;


            priorityQueue.Enqueue(sms1, sms1.QueuePriority);
            priorityQueue.Enqueue(sms2, sms2.QueuePriority);
            priorityQueue.Enqueue(v, v.QueuePriority);

            PriorityQueueItem<Sms, MessageQueuePriority> item = priorityQueue.Dequeue();
            Sms sms = item.Value;
            Console.WriteLine(sms.DestinationAddress);

            item = priorityQueue.Dequeue();
            sms = item.Value;
            Console.WriteLine(sms.DestinationAddress);

            item = priorityQueue.Dequeue();
            sms = item.Value;
            Console.WriteLine(sms.DestinationAddress);            
        }

        [Test]
        public void testSameObject()
        {
            PriorityQueue<Sms, MessageQueuePriority> priorityQueue = new PriorityQueue<Sms, MessageQueuePriority>();

            Sms sms1 = Sms.NewInstance();
            sms1.DestinationAddress = "019";
            sms1.QueuePriority = MessageQueuePriority.Normal;

            priorityQueue.Enqueue(sms1, sms1.QueuePriority);

            sms1.QueuePriority = MessageQueuePriority.High;
            priorityQueue.Enqueue(sms1, sms1.QueuePriority);

            Console.WriteLine(priorityQueue.Count);

        }
    }

   
}
