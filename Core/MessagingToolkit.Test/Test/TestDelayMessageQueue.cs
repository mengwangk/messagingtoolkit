using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Test.Test
{
    
    [TestFixture]
    public class TestDelayMessageQueue
    {
        [Test]
        public void TestQueue()
        {
            /*
            DelayedMessageQueue delayMessageQueue = new DelayedMessageQueue();
            delayMessageQueue.Persist = true;
         
            Sms sms = Sms.NewInstance();
            sms.ScheduledDeliveryDate = DateTime.Now.AddDays(1);
            sms.Identifier = GatewayHelper.GenerateUniqueIdentifier();

            Mms mms = Mms.NewInstance("abc", "019");
            mms.Identifier = GatewayHelper.GenerateUniqueIdentifier();

            Sms sms1 = Sms.NewInstance();
            sms1.Identifier = GatewayHelper.GenerateUniqueIdentifier();

            delayMessageQueue.QueueMessage(sms);
            delayMessageQueue.QueueMessage(mms);
            delayMessageQueue.QueueMessage(sms1);

                    

            Console.WriteLine(delayMessageQueue.Count());
            Console.WriteLine(delayMessageQueue.CountDue());
                        
            IMessage message = delayMessageQueue.DueMessage;
            Console.WriteLine(message.ScheduledDeliveryDate);

            Console.WriteLine(delayMessageQueue.Count());
            Console.WriteLine(delayMessageQueue.CountDue());

            message = delayMessageQueue.DueMessage;
            Console.WriteLine(message.ScheduledDeliveryDate);

            Console.WriteLine(delayMessageQueue.Count());
            Console.WriteLine(delayMessageQueue.CountDue());

            message = delayMessageQueue.DueMessage;
            if (message != null)
                Console.WriteLine(message.ScheduledDeliveryDate);

            Console.WriteLine(delayMessageQueue.Count());
            Console.WriteLine(delayMessageQueue.CountDue());
            */
            
        }
    }

}
