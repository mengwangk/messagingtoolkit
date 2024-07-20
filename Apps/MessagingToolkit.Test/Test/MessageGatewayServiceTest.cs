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
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Service;

using NUnit.Framework;

namespace MessagingToolkit.Core.Test
{
    [TestFixture]
    public class MessageGatewayServiceTest
    {
        /// <summary>
        /// Port name
        /// </summary>
        private const string PortName = "COM8";

        private MessageGatewayService messageGatewayService;

       
        [SetUp]
        public void SetUp()
        {
            messageGatewayService = MessageGatewayService.NewInstance();


            // Create the gateway for mobile
            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageGateway =
                MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();

            // Create the mobile gateway configuration
            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            config.PortName = PortName;
            config.Pin = "740731";

            try
            {
                IMobileGateway mobileGateway = messageGateway.Find(config);
                mobileGateway.LogLevel = LogLevel.Verbose;
                mobileGateway.MessageSent += OnMessageSent;
                mobileGateway.MessageStorage = MessageStorage.Phone;
                messageGatewayService.Add(mobileGateway);
            }
            catch (GatewayException gex)
            {
                Assert.Fail(gex.Message);
                Assert.Fail(gex.ToString());
            }        
        }

       
        [TearDown]
        public void TearDown()
        {
            messageGatewayService.RemoveAll();
        }

      
        public void TestSendMessage()
        {
           // Create a new SMS instance
            Sms sms = Sms.NewInstance();
            sms.DestinationAddress = "0192292309";
            sms.Content = "Test message gateway service";
            messageGatewayService.SendMessage(sms);
        }


     
        public void TestSendMessageToGroup()
        {
            // Create a new SMS instance
            Sms sms = Sms.NewInstance();
            sms.DestinationAddress = "bulk";
            sms.Content = "Test message gateway service";

            messageGatewayService.CreateGroup("bulk");
            messageGatewayService.AddToGroup("bulk", "0192292309");
            messageGatewayService.AddToGroup("bulk", "0126868739");
            messageGatewayService.SendMessage(sms);
        }



        private void OnMessageSent(object sender, MessageEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            Console.WriteLine("Sent Content: " + sms.Content);
        }

      
        public void RemovalTest()
        {
            List<string> gateways = new List<string>();
            gateways.Add("1");
            gateways.Add("2");
            gateways.Add("3");
            Console.WriteLine(gateways.Count());
            foreach (string gw in gateways)
            {
                gateways.Remove(gw);
            }
            Console.WriteLine(gateways.Count());
        }

        [Test]
        public void TestReadMessages()
        {
            List<MessageInformation> messageList = messageGatewayService.ReadMessages(MessageStatusType.ReceivedReadMessages);
            Console.WriteLine(messageList.Count());
        }
    }
}
